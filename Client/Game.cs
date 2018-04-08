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
	private List<byte[]> recvDataList = new List<byte[]>();

	public Character character; // assigned in editor

	public bool IsStart() {
		return isStart;
	}

	public bool IsInitialized() {
		return gameState == GameState.Run;
	}

	// called by Login.cs
	public void StartGame() {
		GameObject character = GameObject.Find ("Character");
		character.AddComponent<Control> ();
		GameObject bulletText = GameObject.Find ("BulletText");
		bulletText.AddComponent<BulletInfo> ();
		isStart = true;
	}

	void Update () {
		byte[][] recvDataArray;
		lock (recvDataList) {
			recvDataArray = recvDataList.ToArray ();
			recvDataList.Clear ();
		}
		for (int i = 0; i < recvDataArray.Length; ++i) {
			short characterDataLen = BitConverter.ToInt16 (recvDataArray [i], 0);
			character.UpdateFromServer (gameState == GameState.Init, recvDataArray [i], 2, (int)characterDataLen);
			gameState = GameState.Run;
		}
	}

	// called by NetworkThread.Receive
	public void AppendGameStatusFromServer(byte[] recvData) {
		lock (recvDataList) {
			recvDataList.Add (recvData);
		}
	}

	public byte[] GetCurrentGameStatus() {
		List<byte> result = new List<byte> ();
		byte[] characterResult = character.Serialize ();
		result.AddRange (BitConverter.GetBytes ((short)characterResult.Length));
		result.AddRange (characterResult);
		return result.ToArray ();
	}
}
