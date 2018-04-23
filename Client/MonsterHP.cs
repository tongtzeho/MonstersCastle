using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterHP {

	public short hp;
	public short maxHp;

	private AudioSource hurtSound;
	private AudioSource dieSound;
	private Skull skull; // kill hint

	public MonsterHP(AudioSource hurt, AudioSource die, Skull skull) {
		hurtSound = hurt;
		dieSound = die;
		this.skull = skull;
	}

	public void Hit(short atk) {
		if (hp > 0 && atk > 0) {
			if (hp - atk > 0) {
				if (!hurtSound.isPlaying) {
					hurtSound.Play ();
				}
			} else {
				if (!dieSound.isPlaying) {
					dieSound.Play ();
				}
				skull.Activate ();
			}
		}
		hp -= atk;
		if (hp < 0) {
			hp = 0;
		}
	}
}
