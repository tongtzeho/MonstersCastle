using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostAnimator : MonoBehaviour {

	private Animation ghostAnimation;
	private ParticleSystem dieParticle;
	private ParticleSystem bombParticle;
	private AudioSource attackSound;
	private AudioSource gateHitSound;

	void Start () {
		ghostAnimation = GetComponent<Animation> ();
		dieParticle = transform.Find ("DieParticle").gameObject.GetComponent<ParticleSystem> ();
		bombParticle = transform.Find ("BombParticle").gameObject.GetComponent<ParticleSystem> ();
		attackSound = transform.Find ("Bip001").gameObject.GetComponent<AudioSource> ();
		gateHitSound = GameObject.Find ("Environment/Gate").GetComponent<AudioSource> ();
	}

	public void SetState(short action) {
		switch (action) {
		case 1:
			ghostAnimation.Play ("move_forward");
			break;
		case 2:
			if (!ghostAnimation.IsPlaying ("attack_short_001")) {
				ghostAnimation.Play ("idle_normal");
			}
			dieParticle.Stop ();
			bombParticle.Stop ();
			break;
		case 3:
			ghostAnimation.Play ("attack_short_001");
			attackSound.Play ();
			break;
		case 4:
			ghostAnimation.Play ("idle_normal");
			if (!dieParticle.isPlaying) {
				dieParticle.Play ();
			}
			attackSound.Stop ();
			break;
		case 5:
			ghostAnimation.Play ("idle_normal");
			if (!bombParticle.isPlaying) {
				bombParticle.Play ();
			}
			attackSound.Stop ();
			if (!gateHitSound.isPlaying) {
				gateHitSound.Play ();
			}
			break;
		}
	}
}
