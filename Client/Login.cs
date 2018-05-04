using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class Login : MonoBehaviour {

	// assigned in editor
	public GameObject startPanel;
	public GameObject registerPanel;
	public GameObject postRegisterPanel;
	public GameObject introductionPanel;
	public UnityEngine.UI.InputField loginUsername;
	public UnityEngine.UI.InputField loginPassword;
	public UnityEngine.UI.InputField registerUsername;
	public UnityEngine.UI.InputField registerPassword;
	public UnityEngine.UI.Text startHint;
	public UnityEngine.UI.Text registerHint;
	public UnityEngine.UI.Text postRegisterSuccess;
	public UnityEngine.UI.Text postRegisterHint;
	public AsyncClient client;

	private enum State {
		Start = 0, Register = 1, PostRegister = 2, Introduction = 3
	};

	private State state = State.Start;

	private int loginResult = -1;
	private int registerResult = -1;
	private bool logoutResult = false;

	void Start() {
		state = State.Start;
		loginUsername.text = "";
		loginPassword.text = "";
		startHint.text = "";
		startPanel.SetActive (true);
		registerPanel.SetActive (false);
		postRegisterPanel.SetActive (false);
		introductionPanel.SetActive (false);
	}

	public void Synchronize(string decode) {
		if (decode [0] == '$' && decode [1] == 's') {
			if (decode [2] == 'i') {
				loginResult = decode [3] - '0';
			} else if (decode [2] == 'u') {
				registerResult = decode [3] - '0';
			}
		} else if (decode [0] == '$' && decode [1] == 'l' && decode [2] == 'o' && decode [3] == 't') {
			logoutResult = true;
		}
	}

	public void RaiseSocketException() {
		switch (state) {
		case State.Start:
			startHint.text = "网络连接错误";
			break;
		case State.Register:
			registerHint.text = "网络连接错误";
			break;
		case State.PostRegister:
			postRegisterHint.text = "网络连接错误";
			break;
		}
	}

	void Update () {
		if (logoutResult) {
			Start ();
			logoutResult = false;
		} else if (loginResult == 0) {
			StartGame ();
		} else if (registerResult == 0) {
			registerPanel.SetActive (false);
			postRegisterPanel.SetActive (true);
			postRegisterSuccess.text = registerUsername.text + "注册成功！";
			state = State.PostRegister;
		} else {
			UnityEngine.UI.Text hint = state == State.PostRegister ? postRegisterHint : startHint;
			if (loginResult == 1) {
				hint.text = "该用户名不存在";
			} else if (loginResult == 2) {
				hint.text = "用户名与密码不匹配";
			} else if (loginResult == 3) {
				hint.text = "该用户已在别处登录";
			}
			if (registerResult == 1) {
				registerHint.text = "该用户名已存在";
			}
		}
		loginResult = registerResult = -1;
	}

	void StartGame() {
		state = State.Start;
		startPanel.SetActive (false);
		postRegisterPanel.SetActive (false);
		GameObject.Find ("Game").GetComponent<Game> ().StartGame ();
	}

	bool IsValidUsername(string username) {
		return username.Length >= 3 && username.Length <= 16 && Regex.IsMatch (username, "[0-9a-zA-Z]+");
	}

	bool IsValidPassword(string password) {
		return password.Length >= 3 && password.Length <= 16 && Regex.IsMatch (password, "[0-9a-zA-Z]+");
	}

	public void OnStartPanelLoginClick() {
		if (!IsValidUsername (loginUsername.text)) {
			startHint.text = "无效的用户名";
		} else if (!IsValidPassword (loginPassword.text)) {
			startHint.text = "无效的密码";
		} else {
			client.SendString ("$si " + loginUsername.text + " " + loginPassword.text);
		}
	}

	public void OnStartPanelRegisterClick() {
		registerUsername.text = "";
		registerPassword.text = "";
		registerHint.text = "";
		registerResult = -1;
		startPanel.SetActive (false);
		registerPanel.SetActive (true);
		state = State.Register;
	}

	public void OnStartPanelIntroductionClick() {
		startPanel.SetActive (false);
		introductionPanel.SetActive (true);
		state = State.Introduction;
	}

	public void OnExitClick() {
		Application.Quit ();
	}

	public void OnRegisterPanelRegisterClick() {
		if (!IsValidUsername (registerUsername.text)) {
			registerHint.text = "无效的用户名";
		} else if (!IsValidPassword (registerPassword.text)) {
			registerHint.text = "无效的密码";
		} else {
			client.SendString ("$su " + registerUsername.text + " " + registerPassword.text);
		}
	}

	public void OnPostRegisterPanelEnterClick() {
		client.SendString ("$si " + registerUsername.text + " " + registerPassword.text);
	}

	public void OnBackClick() {
		Start ();
	}
		
	/*
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
	public AsyncClient client;

	private int loginResult = -1;
	private int registerResult = -1;
	private bool logoutResult = false;

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
		} else if (decode [0] == '$' && decode [1] == 'l' && decode [2] == 'o' && decode [3] == 't') {
			logoutResult = true;
		}
	}

	void Update () {
		if (logoutResult) {
			Start ();
			logoutResult = false;
		} else if (loginResult == 0) {
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
			client.SendString ("$si " + loginUsername.text + " " + loginPassword.text);
		}
	}

	public void OnRegisterClick() {
		if (!IsValidUsername (registerUsername.text)) {
			registerHint.text = "无效的用户名";
		} else if (!IsValidPassword (registerPassword.text)) {
			registerHint.text = "无效的密码";
		} else {
			client.SendString ("$su " + registerUsername.text + " " + registerPassword.text);
		}
	}

	public void OnBackClick() {
		startPanel.SetActive (true);
		loginPanel.SetActive (false);
		registerPanel.SetActive (false);
	}*/
}
