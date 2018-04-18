using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterHPBar : MonoBehaviour {

	private GameObject hpSliderObject;
	private UnityEngine.UI.Slider hpSlider;
	private Monster monster;
	private Transform hpBar;

	void Start () {
		hpSliderObject = transform.Find ("HPCanvas/HPSlider").gameObject;
		hpSlider = hpSliderObject.GetComponent<UnityEngine.UI.Slider> ();
		if (gameObject.name == "Brute") {
			monster = GetComponent<Brute> ().monster;
			hpBar = transform.Find ("hipcontrol/hpbar");
		} else { // Ghost
			monster = GetComponent<Ghost> ().monster;
			hpBar = transform.Find ("HpBar");
		}
		Camera characterCamera = GameObject.Find ("Character/Camera").GetComponent<Camera> ();
		Canvas canvas = transform.Find ("HPCanvas").gameObject.GetComponent<Canvas> ();
		canvas.worldCamera = characterCamera;
		canvas.planeDistance = 1.0f;
	}
		
	void Update () {
		if (monster.maxHp != 0) {
			hpSlider.value = ((float)monster.hp) / monster.maxHp;
		}
		hpSliderObject.transform.position = hpBar.position;
	}
}
