using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CriticalHit : MonoBehaviour {

	// assigned in editor
	public AudioSource sound;

	public void Play() {
		sound.Play ();
	}
}
