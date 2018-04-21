using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterHPUI : MonoBehaviour {

	public Character character; // assigned in editor
	private UnityEngine.UI.Image hpBar;
	private UnityEngine.UI.Text hpText;
	public RectTransform rebornTextTransform; // assigned in editor
	private Vector3 rebornTextDefaultPos;
	private Vector3 rebornTextDisablePos;
	private UnityEngine.UI.Text rebornTime;

	void Awake () {
		hpBar = transform.Find ("PlayerHPBar").gameObject.GetComponent<UnityEngine.UI.Image> ();
		hpText = transform.Find ("PlayerHPText").gameObject.GetComponent<UnityEngine.UI.Text> ();
		rebornTime = transform.Find ("RebornText/RebornTime").gameObject.GetComponent<UnityEngine.UI.Text> ();
		rebornTextDefaultPos = rebornTextTransform.position;
		rebornTextDisablePos = rebornTextDefaultPos + new Vector3 (0, 10000, 0);
	}

	void Start() {
		rebornTextTransform.position = rebornTextDisablePos;
	}

	void Update () {
		if (character.maxHp != 0) {
			hpBar.fillAmount = ((float)character.hp) / character.maxHp;
		}
		hpText.text = string.Concat ("生命值\n", character.hp.ToString (), " / ", character.maxHp.ToString ());
		if (character.isAlive == 1) {
			rebornTextTransform.position = rebornTextDisablePos;
		} else {
			rebornTextTransform.position = rebornTextDefaultPos;
			rebornTime.text = ((int)(character.rebornTimeLeft + 1)).ToString ();
		}
	}
}
