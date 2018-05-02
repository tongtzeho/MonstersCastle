using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterHPUI : MonoBehaviour {

	public Character character; // assigned in editor
	private UnityEngine.UI.Image hpBar;
	private UnityEngine.UI.Image hpUpTo;
	private UnityEngine.UI.Text hpText;
	public RectTransform rebornTextTransform; // assigned in editor
	private Vector3 rebornTextDefaultPos;
	private Vector3 rebornTextDisablePos;
	private UnityEngine.UI.Text rebornTime;

	private short prevHp = -1;
	private short prevMaxHp = -1;

	private string[] rebornTimeString = { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" };

	void Awake () {
		hpBar = transform.Find ("PlayerHPBar").gameObject.GetComponent<UnityEngine.UI.Image> ();
		hpUpTo = transform.Find ("PlayerHPUpTo").gameObject.GetComponent<UnityEngine.UI.Image> ();
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
			if (character.upHpLeft == 0) {
				hpUpTo.fillAmount = 0;
			} else {
				int hpUpToValue = character.hp + character.upHpLeft;
				if (hpUpToValue >= character.maxHp) {
					hpUpTo.fillAmount = 1;
				} else {
					hpUpTo.fillAmount = ((float)hpUpToValue) / character.maxHp;
				}
			}
		}
		if (character.hp != prevHp || character.maxHp != prevMaxHp) {
			hpText.text = string.Concat ("生命值\n", character.hp.ToString (), " / ", character.maxHp.ToString ());
			prevHp = character.hp;
			prevMaxHp = character.maxHp;
		}
		if (character.isAlive == 1) {
			rebornTextTransform.position = rebornTextDisablePos;
		} else {
			rebornTextTransform.position = rebornTextDefaultPos;
			rebornTime.text = rebornTimeString [((int)(character.rebornTimeLeft + 1))];
		}
	}
}
