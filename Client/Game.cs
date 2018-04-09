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
	public Brute brute; // assigned in editor

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
			int offset = 0;
			short characterDataLen = BitConverter.ToInt16 (recvDataArray [i], offset);
			character.UpdateFromServer (gameState == GameState.Init, recvDataArray [i], offset + 2, (int)characterDataLen);
			offset += 2 + characterDataLen;
			short bruteDataLen = BitConverter.ToInt16 (recvDataArray [i], offset);
			brute.UpdateFromServer (gameState == GameState.Init, recvDataArray [i], offset + 2, (int)bruteDataLen);
			offset += 2 + bruteDataLen;
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
		byte[] orcResult = brute.Serialize ();
		result.AddRange (BitConverter.GetBytes ((short)orcResult.Length));
		result.AddRange (orcResult);
		return result.ToArray ();
	}
}
