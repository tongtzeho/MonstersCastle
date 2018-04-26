using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CriticalHit : MonoBehaviour {

	// assigned in editor
	public AudioSource sound;

	void Update () {
		
	}

	public void Play() {
		sound.Play ();
	}
}
