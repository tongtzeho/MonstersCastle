using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Game : MonoBehaviour {

	public enum GameState {
		Init = 1, Run = 2
	}

	private bool isStart = false;
	private GameState gameState = GameState.Init;

	private class GameStatus {
		public Vector3 characterPos;
		public float characterRotation;
	}

	private List<GameStatus> gameStatusList = new List<GameStatus>();

	public bool IsStart() {
		return isStart;
	}

	public void StartGame() {
		GameObject character = GameObject.Find ("Character");
		character.AddComponent<FirstPersonalControl> ();
		GameObject bulletText = GameObject.Find ("BulletText");
		bulletText.AddComponent<BulletInfo> ();
	}

	void Update () {

	}

	// called by NetworkThread.Receive
	public void AppendGameStatusFromServer(byte[] recvData) {
		float posX = BitConverter.ToSingle (recvData, 0);
		float posY = BitConverter.ToSingle (recvData, 4);
		float posZ = BitConverter.ToSingle (recvData, 8);
		float r = BitConverter.ToSingle (recvData, 12);
		Debug.Log (posX + " " + posY + " " + posZ + " " + r);
	}
}
