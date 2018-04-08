using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class Login : MonoBehaviour {

	// assigned in editor
	public GameObject startPanel;
	public GameObject loginPanel;
	public GameObject registerPanel;
	public UnityEngine.UI.InputField loginUsername;
	public UnityEngine.UI.InputField loginPassword;
	public UnityEngine.UI.Text loginHint;
	public UnityEngine.UI.InputField registerUsername;
	public UnityEngine.UI.InputField registerPassword;
	public UnityEngine.UI.Text registerHint;
	public NetworkThread networkThread;

	private int loginResult = -1;
	private int registerResult = -1;

	void Start () {
		startPanel.SetActive (true);
		loginPanel.SetActive (false);
		registerPanel.SetActive (false);
	}

	public void GetResultFromServer(string decode) {
		if (decode [0] == '$' && decode [1] == 's') {
			if (decode [2] == 'i') {
				loginResult = decode [3] - '0';
			} else if (decode [2] == 'u') {
				registerResult = decode [3] - '0';
			}
		}
	}

	void Update () {
		if (loginResult == 0) {
			StartGame ();
		} else {
			if (loginResult == 1) {
				loginHint.text = "此用户不存在";
			} else if (loginResult == 2) {
				loginHint.text = "用户名与密码不匹配";
			} else if (loginResult == 3) {
				loginHint.text = "此用户已在别处登录";
			}
			if (registerResult == 1) {
				registerHint.text = "此用户已存在";
			}
		}
		loginResult = registerResult = -1;
	}

	void StartGame() {
		loginPanel.SetActive (false);
		registerPanel.SetActive (false);
		GameObject.Find ("Game").GetComponent<Game> ().StartGame ();
	}

	public void OnStartPanelLoginClick() {
		loginUsername.text = "Netease";
		loginPassword.text = "163";
		loginHint.text = "";
		loginResult = -1;
		startPanel.SetActive (false);
		loginPanel.SetActive (true);
	}

	public void OnStartPanelRegisterClick() {
		registerUsername.text = "";
		registerPassword.text = "";
		registerHint.text = "";
		registerResult = -1;
		startPanel.SetActive (false);
		registerPanel.SetActive (true);
	}

	bool IsValidUsername(string username) {
		return username.Length >= 3 && username.Length <= 16 && Regex.IsMatch (username, "[0-9a-zA-Z]+");
	}

	bool IsValidPassword(string password) {
		return password.Length >= 3 && password.Length <= 16 && Regex.IsMatch (password, "[0-9a-zA-Z]+");
	}

	public void OnLoginClick() {
		if (!IsValidUsername (loginUsername.text)) {
			loginHint.text = "无效的用户名";
		} else if (!IsValidPassword (loginPassword.text)) {
			loginHint.text = "无效的密码";
		} else {
			networkThread.SendString ("$si " + loginUsername.text + " " + loginPassword.text);
		}
	}

	public void OnRegisterClick() {
		if (!IsValidUsername (registerUsername.text)) {
			registerHint.text = "无效的用户名";
		} else if (!IsValidPassword (registerPassword.text)) {
			registerHint.text = "无效的密码";
		} else {
			networkThread.SendString ("$su " + registerUsername.text + " " + registerPassword.text);
		}
	}

	public void OnBackClick() {
		startPanel.SetActive (true);
		loginPanel.SetActive (false);
		registerPanel.SetActive (false);
	}
}
