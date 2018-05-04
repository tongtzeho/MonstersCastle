using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeImage : MonoBehaviour {

	private UnityEngine.UI.RawImage image;

	// assigned in editor
	public float fullAlpha;
	public float fadeAlphaPerSecond;

	void Start () {
		image = GetComponent<UnityEngine.UI.RawImage> ();
	}

	void Update () {
		if (image.color.a != 0) {
			Color c = image.color;
			c.a = Mathf.Max (0, c.a - fadeAlphaPerSecond * Time.deltaTime);
			image.color = c;
		}
	}

	public void Activate() {
		image.color = new Color (1.0f, 1.0f, 1.0f, fullAlpha);
	}
}
