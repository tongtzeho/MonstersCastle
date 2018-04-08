using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletInfo : MonoBehaviour {

	private UnityEngine.UI.Text text;
	private Control controlScript;

	// Use this for initialization
	void Start () {
		text = GetComponent<UnityEngine.UI.Text> ();
		controlScript = GameObject.Find ("Character").GetComponent<Control> ();
	}
	
	// Update is called once per frame
	void Update () {
		short bullet;
		short bulletCapacity;
		short bulletOwn;
		controlScript.GetBulletInfo (out bullet, out bulletCapacity, out bulletOwn);
		text.text = bullet.ToString () + "/" + bulletCapacity.ToString () + "/" + bulletOwn.ToString ();
	}
}
