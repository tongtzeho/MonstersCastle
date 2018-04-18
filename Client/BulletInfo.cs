using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletInfo : MonoBehaviour {

	private UnityEngine.UI.Text text;
	private Control controlScript;
	public Gun sniper; // assigned in editor
	private GameObject sniperImage;
	private GameObject submachineImage;
	private UnityEngine.UI.Image sniperBulletsImage;
	private UnityEngine.UI.Image submachineBulletsImageL;
	private UnityEngine.UI.Image submachineBulletsImageR;
	private bool isSniperImageActive = true;

	void Start () {
		text = transform.Find ("BulletText").gameObject.GetComponent<UnityEngine.UI.Text> ();
		controlScript = GameObject.Find ("Character").GetComponent<Control> ();
		sniperImage = transform.Find ("SniperImage").gameObject;
		submachineImage = transform.Find ("SubmachineImage").gameObject;
		submachineImage.SetActive (false);
		sniperBulletsImage = transform.Find ("SniperBulletsImage").gameObject.GetComponent<UnityEngine.UI.Image> ();
		submachineBulletsImageL = transform.Find ("SubmachineBulletsImageL").gameObject.GetComponent<UnityEngine.UI.Image> ();
		submachineBulletsImageR = transform.Find ("SubmachineBulletsImageR").gameObject.GetComponent<UnityEngine.UI.Image> ();
	}

	void Update () {
		short bullet;
		short bulletCapacity;
		short bulletOwn;
		controlScript.GetBulletInfo (out bullet, out bulletCapacity, out bulletOwn);
		text.text = bulletCapacity.ToString () + " | " + bulletOwn.ToString ();
		if (controlScript.GetActiveGun () == sniper) {
			if (!isSniperImageActive) {
				sniperImage.SetActive (true);
				submachineImage.SetActive (false);
				isSniperImageActive = true;
			}
			sniperBulletsImage.fillAmount = ((float)bullet) / bulletCapacity;
			submachineBulletsImageL.fillAmount = 0.0f;
			submachineBulletsImageR.fillAmount = 0.0f;
		} else {
			if (isSniperImageActive) {
				sniperImage.SetActive (false);
				submachineImage.SetActive (true);
				isSniperImageActive = false;
			}
			sniperBulletsImage.fillAmount = 0.0f;
			if ((bullet & 1) != 0) {
				submachineBulletsImageR.fillAmount = ((float)(bullet >> 1)) / (bulletCapacity >> 1);
				submachineBulletsImageL.fillAmount = ((float)(bullet >> 1) + 1) / (bulletCapacity >> 1);
			} else {
				submachineBulletsImageL.fillAmount = submachineBulletsImageR.fillAmount = ((float)(bullet >> 1)) / (bulletCapacity >> 1);
			}
		}
	}
}
