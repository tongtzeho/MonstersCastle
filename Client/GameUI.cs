using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameUI : MonoBehaviour {

	// assigned in editor
	public GameObject whiteCross;
	public GameObject playerInfo;
	public GameObject victory;
	public Game game;

	void Start () {
		HideAll ();
	}

	public void HideAll() {
		whiteCross.SetActive (false);
		playerInfo.SetActive (false);
		victory.SetActive (false);
	}

	public void StartGame() {
		whiteCross.SetActive (true);
		playerInfo.SetActive (true);
		victory.SetActive (false);
	}

	public void Victory() {
		whiteCross.SetActive (false);
		playerInfo.SetActive (false);
		victory.SetActive (true);
	}

	public void OnAgainClick() {
		game.SendAgain ();
	}

	public void OnLogoutClick() {
		game.SendLogout ();
	}
}
