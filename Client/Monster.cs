using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster {

	public short hp;
	public short maxHp;

	public void Hit(short atk) {
		hp -= atk;
		if (hp < 0) {
			hp = 0;
		}
	}
}
