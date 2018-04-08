using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunAnimator : MonoBehaviour {

	private Animation gunAnimation;
	public GameObject gunRendererGameObject; // assigned in editor
	private SkinnedMeshRenderer meshRenderer;
	private AudioSource fireSound;

	void Start () {
		gunAnimation = GetComponent<Animation> ();
		meshRenderer = gunRendererGameObject.GetComponent<SkinnedMeshRenderer> ();
		fireSound = GetComponent<AudioSource> ();
	}
	
	public void SetState (Control.CharacterState characterState, Gun.GunState gunState) {
		if (gunState == Gun.GunState.Hide) {
			meshRenderer.enabled = false;
			gunAnimation.Play ("Idle");
			fireSound.Stop ();
		} else {
			meshRenderer.enabled = true;
			if (gunState == Gun.GunState.Idle) {
				switch (characterState) {
				case Control.CharacterState.Idle:
					if (!gunAnimation.IsPlaying ("Idle")) {
						gunAnimation.Play ("Idle");
					}
					break;
				case Control.CharacterState.Walk:
					if (!gunAnimation.IsPlaying ("Walk")) {
						gunAnimation.Play ("Walk");
					}
					break;
				case Control.CharacterState.Run:
					if (!gunAnimation.IsPlaying ("Run")) {
						gunAnimation.Play ("Run");
					}
					break;
				}
			} else {
				switch (gunState) {
				case Gun.GunState.Fire:
					if (!gunAnimation.IsPlaying ("Fire")) {
						gunAnimation.Play ("Fire");
					}
					break;
				case Gun.GunState.Reload:
					if (!gunAnimation.IsPlaying ("Reload")) {
						gunAnimation.Play ("Reload");
					}
					break;
				}
			}
		}
	}
}
