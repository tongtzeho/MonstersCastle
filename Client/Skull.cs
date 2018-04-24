using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skull : MonoBehaviour {

	private UnityEngine.UI.RawImage image;
	private Color zeroAlpha = new Color (1.0f, 1.0f, 1.0f, 0.0f);
	private Color fullAlpha = new Color (1.0f, 1.0f, 1.0f, 0.8f);
	private float fadeAlphaPerSecond = 1.3f;

	void Start () {
		image = GetComponent<UnityEngine.UI.RawImage> ();
	}

	void Update () {
		if (image.color.a != 0.0f) {
			float alpha = image.color.a - fadeAlphaPerSecond * Time.deltaTime;
			if (alpha < 0.0f) {
				image.color = zeroAlpha;
			} else {
				image.color = new Color (1.0f, 1.0f, 1.0f, alpha);
			}
		}
	}

	public void Activate() {
		image.color = fullAlpha;
	}
}
