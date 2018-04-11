using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BruteHP : MonoBehaviour {

	private GameObject hpSliderObject;
	private UnityEngine.UI.Slider hpSlider;
	private RectTransform hpRect;
	private Transform head;
	private Camera characterCamera;
	private float minDist = 1;
	private float maxDist = 40;
	private Vector3 maxScale = new Vector3(1, 1, 1);
	private Vector3 minScale = new Vector3(0.2f, 0.4f, 1);
	private Vector3 scale = new Vector3(1, 1, 1);
	private Monster monster;

	void Start () {
		hpSliderObject = transform.Find ("HPCanvas/HPSlider").gameObject;
		hpSlider = hpSliderObject.GetComponent<UnityEngine.UI.Slider> ();
		hpRect = hpSliderObject.GetComponent<RectTransform> ();
		head = transform.Find ("hipcontrol/headcontrol/hpbar");
		characterCamera = GameObject.Find ("Character/Camera").GetComponent<Camera> ();
		monster = GetComponent<Brute> ().monster;
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
		Vector3 headScreen = characterCamera.WorldToScreenPoint (head.position);
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
			hpSliderObject.SetActive (UpdateRect ());
		} else {
			hpSliderObject.SetActive (false);
		}
	}
}
