﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateUI : MonoBehaviour {

	private UnityEngine.UI.Image bar;
	private Color green = new Color(0, 0.8f, 0, 1);
	private Color yellow = new Color(0.9f, 0.75f, 0, 1);
	private Color red = new Color(1, 0, 0, 1);

	void Start () {
		bar = transform.Find ("GateHPBar").GetComponent<UnityEngine.UI.Image> ();
	}
	
	public void SetGateCurrentState(short hp, short maxHp) {
		if (maxHp != 0) {
			float rate = ((float)hp) / maxHp;
			bar.fillAmount = rate;
			if (rate >= 0.5f) {
				bar.color = green;
			} else if (rate >= 0.2f) {
				bar.color = yellow;
			} else {
				bar.color = red;
			}
		}
	}
}
