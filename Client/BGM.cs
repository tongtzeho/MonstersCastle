using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGM : MonoBehaviour {

	// assigned in editor
	public AudioSource gameBGM;
	public AudioSource powerUpBGM;
	public AudioSource victoryBGM;
	public AudioSource defeatBGM;

	public void AllStop() {
		gameBGM.Stop ();
		powerUpBGM.Stop ();
		victoryBGM.Stop ();
		defeatBGM.Stop ();
	}

	public void GamePlay(bool isPowerUp) {
		if (!isPowerUp) {
			if (powerUpBGM.isPlaying) {
				powerUpBGM.Stop ();
			}
			if (!gameBGM.isPlaying) {
				gameBGM.Play ();
			}
		} else {
			if (!powerUpBGM.isPlaying) {
				powerUpBGM.Play ();
			}
			if (gameBGM.isPlaying) {
				gameBGM.Stop ();
			}
		}
	}

	public void Victory() {
		gameBGM.Stop ();
		powerUpBGM.Stop ();
		victoryBGM.Play ();
		defeatBGM.Stop ();
	}
}
