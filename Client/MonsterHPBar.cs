using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterHPBar : MonoBehaviour {

	private GameObject hpSliderObject;
	private UnityEngine.UI.Slider hpSlider;
	private bool isHpSliderActive = true;
	private RectTransform hpRect;
	private Monster monster;
	private Transform hpBar;
	private float minDist;
	private float maxDist;
	private Vector3 maxScale;
	private Vector3 minScale;
	private Vector3 scale;
	private Camera characterCamera;

	void Start () {
		hpSliderObject = transform.Find ("HPCanvas/HPSlider").gameObject;
		hpSlider = hpSliderObject.GetComponent<UnityEngine.UI.Slider> ();
		hpRect = hpSliderObject.GetComponent<RectTransform> ();
		if (gameObject.name == "Brute") {
			monster = GetComponent<Brute> ().monster;
			hpBar = transform.Find ("hipcontrol/hpbar");
		} else { // Ghost
			monster = GetComponent<Ghost> ().monster;
			hpBar = transform.Find ("HpBar");
		}
		minDist = 1;
		maxDist = 40;
		minScale = new Vector3 (0.2f, 0.3f, 1);
		maxScale = new Vector3 (1, 1, 1);
		scale = new Vector3 (1, 1, 1);
		characterCamera = GameObject.Find ("Character/Camera").GetComponent<Camera> ();

	}

	private bool UpdateValue() {
		if (monster.hp == 0 || monster.maxHp == 0) {
			return false;
		} else {
			hpSlider.value = ((float)monster.hp) / monster.maxHp;
			return true;
		}
	}

	private bool UpdateRect() {
		Vector3 headScreen = characterCamera.WorldToScreenPoint (hpBar.position);
		if (headScreen.z <= 0) {
			return false;
		}
		hpRect.position = headScreen;
		float dist = headScreen.z;
		if (dist < minDist) {
			scale = maxScale;
		} else if (dist > maxDist) {
			scale = minScale;
		} else {
			float rate = (maxDist - dist) / (maxDist - minDist);
			scale = minScale + rate * (maxScale - minScale);
		}
		hpRect.localScale = scale;
		return true;
	}

	void Update () {
		if (UpdateValue ()) {
			bool updateRectRet = UpdateRect ();
			if (isHpSliderActive != updateRectRet) {
				hpSliderObject.SetActive (updateRectRet);
				isHpSliderActive = updateRectRet;
			}
		} else {
			if (isHpSliderActive) {
				hpSliderObject.SetActive (false);
				isHpSliderActive = false;
			}
		}
	}
}
