using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeImage : MonoBehaviour {

	private UnityEngine.UI.RawImage image;

	// assigned in editor
	public Color zeroAlpha;
	public Color fullAlpha;
	public float fadeAlphaPerSecond;

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
