using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterHP {

	public short hp;
	public short maxHp;

	private AudioSource hurtSound;
	private AudioSource dieSound;
	private FadeImage skull; // kill hint

	public MonsterHP(AudioSource hurt, AudioSource die, FadeImage skull) {
		hurtSound = hurt;
		dieSound = die;
		this.skull = skull;
	}

	public void Hit(short damage) {
		if (hp > 0 && damage > 0) {
			if (hp - damage > 0) {
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
		hp -= damage;
		if (hp < 0) {
			hp = 0;
		}
	}
}
