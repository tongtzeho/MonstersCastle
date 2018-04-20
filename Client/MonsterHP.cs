using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterHP {

	public short hp;
	public short maxHp;

	private AudioSource hurtSound;
	private AudioSource dieSound;

	public MonsterHP(AudioSource hurt, AudioSource die) {
		hurtSound = hurt;
		dieSound = die;
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
			}
		}
		hp -= atk;
		if (hp < 0) {
			hp = 0;
		}
	}
}
