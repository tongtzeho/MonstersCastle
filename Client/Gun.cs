﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour {

	public enum GunState {
		Hide = 0, Idle = 1, Fire = 2, Reload = 3
	}

	private GunAnimator gunAnimator;
	public float fireInterval; // assigned in editor
	private float fireCD = 0;
	public float reloadTime; // assigned in editor
	private float reloadTimeLeft = 0;
	private AudioSource fireSound;
	private uint bulletNum;
	public uint bulletCapacity; // assigned in editor
	public uint bulletOwn; // assigned in editor
	private Camera parent;
	private Ray fireRay;
	private RaycastHit hit;

	void Start () {
		gunAnimator = GetComponent<GunAnimator> ();
		fireSound = GetComponent<AudioSource> ();
		bulletNum = bulletCapacity;
		parent = gameObject.transform.parent.gameObject.GetComponent<Camera> ();
	}

	void Update () {
		fireCD -= Time.deltaTime;
		if (fireCD < 0) {
			fireCD = 0;
		}
		if (reloadTimeLeft > 0) {
			reloadTimeLeft -= Time.deltaTime;
			if (reloadTimeLeft <= 0) {
				reloadTimeLeft = 0;
				if (bulletOwn > 0) {
					uint bulletUse = bulletCapacity - bulletNum < bulletOwn ? bulletCapacity - bulletNum : bulletOwn;
					bulletNum += bulletUse;
					bulletOwn -= bulletUse;
				}
			}
		}
	}

	public GunAnimator GetAnimator() {
		return gunAnimator;
	}

	public void ResetTime() {
		fireCD = 0;
		reloadTimeLeft = 0;
	}

	public uint GetBulletNum() {
		return bulletNum;
	}

	private bool Fire() {
		if (fireCD == 0 && bulletNum > 0) {
			reloadTimeLeft = 0;
			fireCD = fireInterval;
			fireSound.Play ();
			bulletNum--;
			fireRay = parent.ScreenPointToRay (new Vector3 (Screen.width / 2.0f, Screen.height / 2.0f, 0));
			if (Physics.Raycast (fireRay, out hit)) {
				
			}
			return true;
		} else {
			return false;
		}
	}

	private bool Reload() {
		if (reloadTimeLeft == 0 && bulletNum < bulletCapacity && bulletOwn > 0) {
			reloadTimeLeft = reloadTime;
			return true;
		} else {
			return false;
		}
	}

	// return gun animator state to FirstPersonalControl.cs
	public GunState Action(bool pressFire, bool pressReload) {
		if (pressReload) {
			if (reloadTimeLeft != 0 || Reload ()) {
				fireSound.Stop ();
				return GunState.Reload;
			} else {
				return GunState.Idle;
			}
		} else {
			if (reloadTimeLeft != 0) {
				if (pressFire && Fire ()) { // press Fire while reloading
					return GunState.Fire;
				} else {
					return GunState.Reload;
				}
			} else {
				if (!pressFire) {
					if (fireCD != 0) {
						return GunState.Fire;
					} else {
						fireSound.Stop ();
						if (bulletNum == 0 && Reload ()) { // when switching to an empty gun
							return GunState.Reload;
						} else {
							return GunState.Idle;
						}
					}
				} else {
					if (fireCD != 0 || Fire()) {
						return GunState.Fire;
					} else { // Fire() return false, means no bullet, need to reload
						fireSound.Stop ();
						if (Reload ()) {
							return GunState.Reload;
						} else {
							return GunState.Idle;
						}
					}
				}
			}
		}
	}
}