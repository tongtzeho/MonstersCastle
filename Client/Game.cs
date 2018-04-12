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
	public GhostPool ghostPool; // assigned in editor

	private HashSet<int> usedGhostServerId = new HashSet<int>();

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
			short ghostDataLen = BitConverter.ToInt16 (recvDataArray [i], offset);
			short ghostDataByte = BitConverter.ToInt16 (recvDataArray [i], offset + 2);
			offset += 4;
			usedGhostServerId.Clear ();
			for (int j = 0; j < ghostDataLen; ++j) {
				short ghostServerId = BitConverter.ToInt16 (recvDataArray [i], offset);
				short ghostHp = BitConverter.ToInt16 (recvDataArray [i], offset + 2);
				Ghost ghost = ghostPool.GetGhostFromServerId ((int)ghostServerId, ghostHp);
				if (ghost != null) {
					ghost.UpdateFromServer (recvDataArray [i], offset, ghostDataByte);
				}
				offset += ghostDataByte;
				usedGhostServerId.Add (ghostServerId);
			}
			ghostPool.RecycleUnusedGhosts (usedGhostServerId);
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
		byte[] bruteResult = brute.Serialize ();
		result.AddRange (BitConverter.GetBytes ((short)bruteResult.Length));
		result.AddRange (bruteResult);
		result.AddRange (ghostPool.Serialize ());
		return result.ToArray ();
	}
}
