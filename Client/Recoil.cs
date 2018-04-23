using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Recoil {

	private float recoil = 0.0f;
	public const float maxRecoil = 10.0f;
	public const float recoverSpeed = 20.0f;

	public void Reset() {
		recoil = 0.0f;
	}

	public float AddRecoil(float addRecoil) {
		recoil += addRecoil;
		if (recoil > maxRecoil) {
			recoil = maxRecoil;
		}
		return recoil;
	}

	public float Recover(float dt) {
		float result = recoverSpeed * dt;
		if (result > recoil) {
			result = recoil;
			recoil = 0.0f;
		} else {
			recoil -= result;
		}
		return result;
	}
}
