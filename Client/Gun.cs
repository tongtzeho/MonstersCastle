using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour {

	private GunAnimator gunAnimator;

	void Start () {
		gunAnimator = GetComponent<GunAnimator> ();
	}

	void Update () {
		
	}

	public GunAnimator GetAnimator() {
		return gunAnimator;
	}
}
