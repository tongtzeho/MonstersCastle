using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour {

	public enum GunState {
		Hide = 0, Idle = 1, Fire = 2, Reload = 3
	}

	private GunAnimator gunAnimator;
	public float fireInterval; // assigned in editor
	private float fireAnimationTimeLeft = 0;
	private float fireCD = 0; // when swtiching gun, fireAnimTimeLeft reset to 0, but fireCD remains
	public float reloadTime; // assigned in editor
	private float reloadTimeLeft = 0;
	private AudioSource fireSound;
	private short bulletNum = 0;
	public short bulletCapacity; // assigned in editor
	private short bulletOwn = 0;
	public short bulletLimit; // assigned in editor
	private Camera parent;
	private Ray fireRay;
	public float diffuse; // assigned in editor
	private RaycastHit hitResult;
	private Hit hitSystem;
	public short attack; // assigned in editor
	public short criticalAttack; // assigned in editor
	public float recoil; // assigned in editor

	void Start () {
		gunAnimator = GetComponent<GunAnimator> ();
		fireSound = GetComponent<AudioSource> ();
		bulletNum = bulletCapacity;
		parent = gameObject.transform.parent.gameObject.GetComponent<Camera> ();
		hitSystem = GameObject.Find ("Game").GetComponent<Hit> ();
	}

	void Update () {
		fireAnimationTimeLeft -= Time.deltaTime;
		if (fireAnimationTimeLeft < 0) {
			fireAnimationTimeLeft = 0;
		}
		fireCD -= Time.deltaTime;
		if (fireCD < 0) {
			fireCD = 0;
		}
		if (reloadTimeLeft > 0) {
			reloadTimeLeft -= Time.deltaTime;
			if (reloadTimeLeft <= 0) {
				reloadTimeLeft = 0;
				if (bulletOwn > 0) {
					short bulletUse = (short)(bulletCapacity - bulletNum < bulletOwn ? bulletCapacity - bulletNum : bulletOwn);
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
		fireAnimationTimeLeft = 0;
		reloadTimeLeft = 0;
	}

	public short GetBulletNum() {
		return bulletNum;
	}

	public void SetBulletNum(short bn) {
		bulletNum = bn;
	}

	public short GetBulletOwn() {
		return bulletOwn;
	}

	public void SetBulletOwn(short bo) {
		bulletOwn = bo;
	}

	public void AddBulletOwn(short abo) {
		bulletOwn += abo;
		if (bulletOwn > bulletLimit) {
			bulletOwn = bulletLimit;
		}
	}

	private bool Fire(out float recoilResult) {
		if (fireCD == 0 && bulletNum > 0) {
			reloadTimeLeft = 0;
			fireAnimationTimeLeft = fireInterval;
			fireCD = fireInterval;
			fireSound.Play ();
			bulletNum--;
			float randomX = UnityEngine.Random.value * 2.0f * diffuse - diffuse;
			float randomY = UnityEngine.Random.value * 2.0f * diffuse - diffuse;
			fireRay = parent.ScreenPointToRay (new Vector3 (Screen.width / 2.0f + randomX, Screen.height / 2.0f + randomY, 0));
			if (Physics.Raycast (fireRay, out hitResult)) {
				hitSystem.HitCollider (attack, criticalAttack, hitResult.collider);
			}
			recoilResult = recoil;
			return true;
		} else {
			recoilResult = 0.0f;
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

	// return gun animator state to Control.cs
	public GunState Action(bool pressFire, bool pressReload, out float recoilResult) {
		recoilResult = 0.0f;
		if (pressReload) {
			if (reloadTimeLeft != 0 || Reload ()) {
				fireSound.Stop ();
				return GunState.Reload;
			} else {
				return GunState.Idle;
			}
		} else {
			if (reloadTimeLeft != 0) {
				if (pressFire && Fire (out recoilResult)) { // press Fire while reloading
					return GunState.Fire;
				} else {
					return GunState.Reload;
				}
			} else {
				if (!pressFire) {
					if (fireAnimationTimeLeft != 0) {
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
					if (fireAnimationTimeLeft != 0 || Fire(out recoilResult)) {
						return GunState.Fire;
					} else {
						fireSound.Stop ();
						if (bulletNum == 0 && Reload ()) { // when pressing Fire with empty bullet
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
