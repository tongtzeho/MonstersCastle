using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostAnimator : MonoBehaviour {

	private Animation gunAnimation;
	private ParticleSystem dieParticle;
	private ParticleSystem bombParticle;

	void Start () {
		gunAnimation = GetComponent<Animation> ();
		dieParticle = transform.Find ("DieParticle").gameObject.GetComponent<ParticleSystem> ();
		bombParticle = transform.Find ("BombParticle").gameObject.GetComponent<ParticleSystem> ();
	}

	public void SetState(short action) {
		switch (action) {
		case 1:
			gunAnimation.Play ("move_forward");
			break;
		case 2:
			if (!gunAnimation.IsPlaying ("attack_short_001")) {
				gunAnimation.Play ("idle_normal");
			}
			dieParticle.Stop ();
			bombParticle.Stop ();
			break;
		case 3:
			gunAnimation.Play ("attack_short_001");
			break;
		case 4:
			gunAnimation.Play ("idle_normal");
			if (!dieParticle.isPlaying) {
				dieParticle.Play ();
			}
			break;
		case 5:
			gunAnimation.Play ("idle_normal");
			if (!bombParticle.isPlaying) {
				bombParticle.Play ();
			}
			break;
		}
	}
}
