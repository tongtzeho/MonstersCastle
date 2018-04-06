using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkingSound : MonoBehaviour {

	private AudioSource walk;
	private AudioSource run;
	private int soundState = 0;

	void Start () {
		AudioSource[] sounds = GetComponents<AudioSource> ();
		if (sounds [0].clip.name == "run") {
			run = sounds [0];
			walk = sounds [1];
		} else {
			walk = sounds [0];
			run = sounds [1];
		}
	}
	
	public void SetSoundState(int newState) {
		if (newState != soundState) {
			switch (newState) {
			case 0:
				walk.Stop ();
				run.Stop ();
				break;
			case 1:
				walk.Play ();
				run.Stop ();
				break;
			case 2:
				walk.Stop ();
				run.Play ();
				break;
			}
		}
		soundState = newState;
	}
}
