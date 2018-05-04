using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGM : MonoBehaviour {

	// assigned in editor
	public AudioSource gameBGM;
	public AudioSource hpUpBGM;
	public AudioSource victoryBGM;
	public AudioSource defeatBGM;

	private int gamePlayState = 0;

	public void AllStop() {
		gamePlayState = 0;
		gameBGM.Stop ();
		hpUpBGM.Stop ();
		victoryBGM.Stop ();
		defeatBGM.Stop ();
	}

	public void GamePlay(bool isHpUp) {
		int currGamePlayState = isHpUp ? 2 : 1;
		if (gamePlayState != currGamePlayState) {
			if (!isHpUp) {
				if (hpUpBGM.isPlaying) {
					hpUpBGM.Stop ();
				}
				if (!gameBGM.isPlaying) {
					gameBGM.Play ();
				}
			} else {
				if (!hpUpBGM.isPlaying) {
					hpUpBGM.Play ();
				}
				if (gameBGM.isPlaying) {
					gameBGM.Stop ();
				}
			}
			gamePlayState = currGamePlayState;
		}
	}

	public void Victory() {
		gamePlayState = 0;
		gameBGM.Stop ();
		hpUpBGM.Stop ();
		victoryBGM.Play ();
		defeatBGM.Stop ();
	}

	public void Defeat() {
		gamePlayState = 0;
		gameBGM.Stop ();
		hpUpBGM.Stop ();
		victoryBGM.Stop ();
		defeatBGM.Play ();
	}
}
