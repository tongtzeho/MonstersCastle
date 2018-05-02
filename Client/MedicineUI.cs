using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MedicineUI : MonoBehaviour {

	private Character character;
	private UnityEngine.UI.Image image;
	private UnityEngine.UI.Text numberText;

	private Color opaqueImage = new Color (1, 1, 1, 0.93f);
	private Color transparentImage = new Color (1, 1, 1, 0.3f);

	private Color opaqueText = new Color (0.45f, 0.15f, 0.16f, 0.93f);
	private Color transparentText = new Color (0.45f, 0.15f, 0.16f, 0.3f);

	private string[] numberString = { "0", "1", "2", "3", "4", "5" };

	void Start () {
		character = GameObject.Find ("Character").GetComponent<Character> ();
		image = transform.Find ("Medicine").GetComponent<UnityEngine.UI.Image> ();
		numberText = transform.Find ("Medicine/Num").GetComponent<UnityEngine.UI.Text> ();
	}

	void Update () {
		numberText.text = numberString [character.medicineNum];
		if (character.medicineNum == 0) {
			image.color = transparentImage;
			numberText.color = transparentText;
		} else {
			image.color = opaqueImage;
			numberText.color = opaqueText;
		}
	}
}
