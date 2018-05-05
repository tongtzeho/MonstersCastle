using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameUIPanel : MonoBehaviour {

	// assigned in editor
	public GameObject sight;
	public GameObject playerInfo;
	public GameObject victory;
	public GameObject defeat;
	public RectTransform task;
	public RectTransform pause;
	public UnityEngine.UI.Text socketException;
	public Game game;

	private Vector3 disablePosition = new Vector3(0, 10000, 0);

	void Start () {
		HideAll ();
	}

	public void HideAll() {
		sight.SetActive (false);
		playerInfo.SetActive (false);
		victory.SetActive (false);
		defeat.SetActive (false);
		ShowTask (false);
		pause.localPosition = disablePosition;
	}

	public void StartGame() {
		sight.SetActive (true);
		playerInfo.SetActive (true);
		victory.SetActive (false);
		defeat.SetActive (false);
		ShowTask (false);
		pause.localPosition = disablePosition;
	}

	public void Victory() {
		sight.SetActive (false);
		playerInfo.SetActive (false);
		victory.SetActive (true);
		defeat.SetActive (false);
		ShowTask (false);
		pause.localPosition = disablePosition;
	}

	public void Defeat() {
		sight.SetActive (false);
		playerInfo.SetActive (false);
		victory.SetActive (false);
		defeat.SetActive (true);
		ShowTask (false);
		pause.localPosition = disablePosition;
	}

	public void OnAgainClick() {
		game.SendAgain ();
	}

	public void OnLogoutClick() {
		game.SendLogout ();
	}

	void Update() {
		if (socketException.color.a != 0) {
			Color c = socketException.color;
			c.a = Mathf.Max (0, c.a - 0.2f * Time.deltaTime);
			socketException.color = c;
		}
	}

	public void RaiseSocketException() {
		socketException.color = new Color (1.0f, 1.0f, 1.0f, 1.0f);
	}

	public void Pause() {
		pause.localPosition = Vector3.zero;
	}

	public void Continue() {
		pause.localPosition = disablePosition;
	}

	public void ShowTask(bool show) {
		task.localPosition = show ? Vector3.zero : disablePosition;
	}
}
