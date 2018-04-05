using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunAnimator : MonoBehaviour {

	private int charcterState; // 0 for idle, 1 for walk, 2 for run
	private int gunState; // 0 for hide, 1 for idle, 2 for fire, 3 for reload, 4 for sniper
	private Animation gunAnimation;
	public GameObject gunRendererGameObject;
	private SkinnedMeshRenderer meshRenderer; // assigned in editor

	void Start () {
		gunAnimation = GetComponent<Animation> ();
		meshRenderer = gunRendererGameObject.GetComponent<SkinnedMeshRenderer> ();
	}
	
	public void SetState (int newCharacterState, int newGunState) {
		if (newGunState == 0) {
			meshRenderer.enabled = false;
			gunAnimation.Play ("Idle");
		} else {
			meshRenderer.enabled = true;
			if (newGunState == 1) {
				if (newCharacterState != charcterState) {
					switch (newCharacterState) {
					case 0:
						gunAnimation.Play ("Idle");
						break;
					case 1:
						gunAnimation.Play ("Walk");
						break;
					case 2:
						gunAnimation.Play ("Run");
						break;
					}
				}
			} else {
				if (newGunState != gunState) {
					switch (newGunState) {
					case 2:
						gunAnimation.Play ("Fire");
						break;
					case 3:
						gunAnimation.Play ("Reload");
						break;
					}
				}
			}
		}
		charcterState = newCharacterState;
		gunState = newGunState;
	}
}
