using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameUIPanel : MonoBehaviour {

	// assigned in editor
	public GameObject sight;
	public GameObject playerInfo;
	public GameObject victory;
	public Game game;

	void Start () {
		HideAll ();
	}

	public void HideAll() {
		sight.SetActive (false);
		playerInfo.SetActive (false);
		victory.SetActive (false);
	}

	public void StartGame() {
		sight.SetActive (true);
		playerInfo.SetActive (true);
		victory.SetActive (false);
	}

	public void Victory() {
		sight.SetActive (false);
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
