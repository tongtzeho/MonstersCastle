using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkingSound : MonoBehaviour {

	private AudioSource walk;
	private AudioSource run;

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
	
	public void SetSoundState(FirstPersonalControl.CharacterState characterState) {
		switch (characterState) {
		case FirstPersonalControl.CharacterState.Idle:
			walk.Stop ();
			run.Stop ();
			break;
		case FirstPersonalControl.CharacterState.Walk:
			run.Stop ();
			if (!walk.isPlaying) {
				walk.Play ();
			}
			break;
		case FirstPersonalControl.CharacterState.Run:
			walk.Stop ();
			if (!run.isPlaying) {
				run.Play ();
			}
			break;
		}
	}
}
