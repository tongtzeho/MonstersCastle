using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour {

	private GunAnimator gunAnimator;
	public float fireInterval; // assigned in editor
	private float fireCD = 0;
	public float reloadTime; // assigned in editor
	private float reloadTimeLeft = 0;
	private AudioSource fireSound;
	private uint bulletNum;
	public uint bulletCapacity; // assigned in editor
	public uint bulletOwn; // assigned in editor

	void Start () {
		gunAnimator = GetComponent<GunAnimator> ();
		fireSound = GetComponent<AudioSource> ();
		bulletNum = bulletCapacity;
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
	public int Action(bool pressFire, bool pressReload) {
		if (pressReload) {
			if (Reload ()) {
				fireSound.Stop ();
				return 3;
			} else {
				return 1;
			}
		} else {
			if (reloadTimeLeft != 0) {
				if (pressFire && Fire ()) { // press Fire while reloading
					return 2;
				} else {
					return 3;
				}
			} else {
				if (!pressFire) {
					if (fireCD != 0) {
						return 2;
					} else {
						fireSound.Stop ();
						if (bulletNum == 0 && Reload ()) { // when switching to an empty gun
							return 3;
						} else {
							return 1;
						}
					}
				} else {
					if (fireCD != 0 || Fire()) {
						return 2;
					} else { // Fire() return false, means no bullet, need to reload
						fireSound.Stop ();
						if (Reload ()) {
							return 3;
						} else {
							return 1;
						}
					}
				}
			}
		}
	}
}
