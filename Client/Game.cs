﻿using System;
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
	private short gameResult = 0; // 0 for playing, 1 for win, 2 for lose
	private Queue<byte[]> recvQueue = new Queue<byte[]>();

	public Character character; // assigned in editor
	public Brute brute; // assigned in editor
	public GhostPool ghostPool; // assigned in editor
	private BulletPool submachineBulletPool;
	private BulletPool sniperBulletPool;
	public Control control; // assigned in editor
	public GameUI gameUI; // assigned in editor
	public GameBGM gameBGM; // assigned in editor

	public AsyncClient client; // assigned in editor

	private HashSet<int> usedGhostServerId = new HashSet<int>();

	void Awake() {
		BulletPool[] bulletPools = GameObject.Find ("BulletPool").GetComponents<BulletPool> ();
		if (bulletPools [0].prefabName == "SniperBullet") {
			sniperBulletPool = bulletPools [0];
			submachineBulletPool = bulletPools [1];
		} else {
			sniperBulletPool = bulletPools [1];
			submachineBulletPool = bulletPools [0];
		}
	}

	public bool IsStart() {
		return isStart;
	}

	public bool IsInitialized() {
		return gameState == GameState.Run;
	}

	public bool IsGameOver() {
		return gameResult != 0;
	}

	// called by GameUI.cs
	public void SendAgain() {
		Reset ();
		client.SendCommand (gameResult, 1);
		// when socket received a msg '$si0', Login.cs will start a new game
	}

	// called by GameUI.cs
	public void SendLogout() {
		Reset ();
		client.SendCommand (gameResult, 2);
		// when socket received a msg '$lot', Login.cs will raise the start panel
	}

	// called by Login.cs
	public void StartGame() {
		Reset ();
		control.Allow ();
		gameUI.StartGame ();
		gameBGM.AllStop ();
		gameBGM.GamePlay (false);
		gameResult = 0;
		isStart = true;
	}

	public void Reset() {
		isStart = false;
		gameBGM.AllStop ();
		ghostPool.Reset ();
		control.Reset ();
		control.Disallow ();
		gameState = GameState.Init;
		recvQueue.Clear ();
	}

	void Update () {
		while (recvQueue.Count > 0) {
			byte[] recvData = recvQueue.Dequeue ();
			gameResult = BitConverter.ToInt16 (recvData, 0);
			short level = BitConverter.ToInt16 (recvData, 2);
			short gateHp = BitConverter.ToInt16 (recvData, 4);
			if (gameResult == 1) {
				Reset ();
				gameUI.Victory ();
				gameBGM.Victory ();
			} else {
				int offset = 6;
				short characterDataLen = BitConverter.ToInt16 (recvData, offset);
				character.UpdateFromServer (gameState == GameState.Init, recvData, offset + 2, (int)characterDataLen);
				offset += 2 + characterDataLen;
				short bruteDataLen = BitConverter.ToInt16 (recvData, offset);
				brute.UpdateFromServer (gameState == GameState.Init, recvData, offset + 2, (int)bruteDataLen);
				offset += 2 + bruteDataLen;
				short ghostDataLen = BitConverter.ToInt16 (recvData, offset);
				short ghostDataByte = BitConverter.ToInt16 (recvData, offset + 2);
				offset += 4;
				usedGhostServerId.Clear ();
				for (int j = 0; j < ghostDataLen; ++j) {
					short ghostServerId = BitConverter.ToInt16 (recvData, offset);
					short ghostHp = BitConverter.ToInt16 (recvData, offset + 2);
					Ghost ghost = ghostPool.GetGhostFromServerId ((int)ghostServerId, ghostHp);
					if (ghost != null) {
						ghost.UpdateFromServer (recvData, offset, ghostDataByte);
					}
					offset += ghostDataByte;
					usedGhostServerId.Add (ghostServerId);
				}
				ghostPool.RecycleUnusedGhosts (usedGhostServerId);
				short submachineBulletsNum = BitConverter.ToInt16 (recvData, offset);
				if (gameState == GameState.Init) {
					submachineBulletPool.UpdateFromServer (recvData, offset, 2 + 12 * submachineBulletsNum);
				}
				offset += 2 + 12 * submachineBulletsNum;
				short sniperBulletsNum = BitConverter.ToInt16 (recvData, offset);
				if (gameState == GameState.Init) {
					sniperBulletPool.UpdateFromServer (recvData, offset, 2 + 12 * sniperBulletsNum);
				}
				offset += 2 + 12 * submachineBulletsNum;
				gameState = GameState.Run;
			}
		}
	}

	// called by NetworkThread.Receive
	public void AppendGameStatusFromServer(byte[] recvData) {
		recvQueue.Enqueue (recvData);
	}

	public byte[] GetCurrentGameStatus() {
		List<byte> result = new List<byte> ();
		result.AddRange (BitConverter.GetBytes ((short)0));
		result.AddRange (BitConverter.GetBytes ((short)0));
		byte[] characterResult = character.Serialize ();
		result.AddRange (BitConverter.GetBytes ((short)characterResult.Length));
		result.AddRange (characterResult);
		byte[] bruteResult = brute.Serialize ();
		result.AddRange (BitConverter.GetBytes ((short)bruteResult.Length));
		result.AddRange (bruteResult);
		result.AddRange (ghostPool.Serialize ());
		result.AddRange (submachineBulletPool.Serialize ());
		result.AddRange (sniperBulletPool.Serialize ());
		return result.ToArray ();
	}
}
