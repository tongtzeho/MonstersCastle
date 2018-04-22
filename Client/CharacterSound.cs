using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSound : MonoBehaviour {

	// assigned in editor
	public AudioSource walk;
	public AudioSource run;
	public AudioSource hurt;
	public AudioSource die;
	public AudioSource reborn;
	
	public void SetWalkingSoundState(Control.CharacterState characterState) {
		switch (characterState) {
		case Control.CharacterState.Idle:
			walk.Stop ();
			run.Stop ();
			break;
		case Control.CharacterState.Walk:
			run.Stop ();
			if (!walk.isPlaying) {
				walk.Play ();
			}
			break;
		case Control.CharacterState.Run:
			walk.Stop ();
			if (!run.isPlaying) {
				run.Play ();
			}
			break;
		}
	}

	public void PlayHurtSound() {
		if (!hurt.isPlaying) {
			hurt.Play ();
		}
	}

	public void PlayDieSound() {
		if (!die.isPlaying) {
			die.Play ();
		}
	}

	public void PlayRebornSound() {
		if (!reborn.isPlaying) {
			reborn.Play ();
		}
	}
}
