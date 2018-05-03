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
	private short gameResult = 0; // 0 for playing, 1 for win, 2 for lose
	private Queue<int> recvMessageIndex = new Queue<int>();

	public Character character; // assigned in editor
	public Brute brute; // assigned in editor
	public ObjectPool ghostPool; // assigned in editor
	private ObjectPool submachineBulletPool;
	private ObjectPool sniperBulletPool;
	public ObjectPool medicinePool; // assigned in editor
	public ObjectPool ballPool; // assigned in editor
	public Control control; // assigned in editor
	public GateUI gateUI; // assigned in editor
	public GameUIPanel gameUIPanel; // assigned in editor
	public BGM gameBGM; // assigned in editor
	public AsyncClient client; // assigned in editor

	private byte[] serializedData = new byte[1024];

	void Awake() {
		ObjectPool[] bulletPools = GameObject.Find ("BulletPool").GetComponents<ObjectPool> ();
		if (bulletPools [0].prefabName == "Prefabs/SniperBullet") {
			sniperBulletPool = bulletPools [0];
			submachineBulletPool = bulletPools [1];
		} else {
			sniperBulletPool = bulletPools [1];
			submachineBulletPool = bulletPools [0];
		}
		serializedData [0] = 0xed;
		serializedData [1] = 0xcb;
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

	// called by GameUIPanel.cs
	public void SendAgain() {
		Reset ();
		client.SendCommand (gameResult, 1);
		// when socket received a msg '$si0', Login.cs will start a new game
	}

	// called by GameUIPanel.cs
	public void SendLogout() {
		Reset ();
		client.SendCommand (gameResult, 2);
		// when socket received a msg '$lot', Login.cs will raise the start panel
	}

	// called by Login.cs
	public void StartGame() {
		Reset ();
		control.Allow ();
		gameUIPanel.StartGame ();
		gameBGM.AllStop ();
		gameBGM.GamePlay (false);
		gameResult = 0;
		isStart = true;
	}

	public void Reset() {
		isStart = false;
		gameBGM.AllStop ();
		brute.Reset ();
		ghostPool.Reset ();
		ballPool.Reset ();
		control.Reset ();
		control.Disallow ();
		gameState = GameState.Init;
		recvMessageIndex.Clear ();
		System.GC.Collect ();
	}

	void Update () {
		while (recvMessageIndex.Count > 0) {
			byte[] recvData = client.GetMessageContent (recvMessageIndex.Dequeue ());
			gameResult = BitConverter.ToInt16 (recvData, 0);
			//short level = BitConverter.ToInt16 (recvData, 2);
			short gateHp = BitConverter.ToInt16 (recvData, 4);
			short gateMaxHp = BitConverter.ToInt16 (recvData, 6);
			if (gameResult == 1) {
				Reset ();
				gameUIPanel.Victory ();
				gameBGM.Victory ();
			} else {
				gateUI.SetGateCurrentState (gateHp, gateMaxHp);
				int offset = 8;

				short characterDataLen = BitConverter.ToInt16 (recvData, offset);
				character.Synchronize (gameState == GameState.Init, recvData, offset + 2);
				offset += 2 + characterDataLen;

				short bruteDataLen = BitConverter.ToInt16 (recvData, offset);
				brute.Synchronize (gameState == GameState.Init, recvData, offset + 2);
				offset += 2 + bruteDataLen;

				short ghostDataSize = BitConverter.ToInt16 (recvData, offset);
				short ghostDataByte = BitConverter.ToInt16 (recvData, offset + 2);
				ghostPool.Synchronize (recvData, offset);
				offset += 4 + ghostDataSize * ghostDataByte;

				short submachineBulletDataSize = BitConverter.ToInt16 (recvData, offset);
				short submachineBulletDataByte = BitConverter.ToInt16 (recvData, offset + 2);
				if (gameState == GameState.Init) {
					submachineBulletPool.Synchronize (recvData, offset);
				}
				offset += 4 + submachineBulletDataSize * submachineBulletDataByte;

				short sniperBulletDataSize = BitConverter.ToInt16 (recvData, offset);
				short sniperBulletDataByte = BitConverter.ToInt16 (recvData, offset + 2);
				if (gameState == GameState.Init) {
					sniperBulletPool.Synchronize (recvData, offset);
				}
				offset += 4 + sniperBulletDataSize * sniperBulletDataByte;

				short medicineDataSize = BitConverter.ToInt16 (recvData, offset);
				short medicineDataByte = BitConverter.ToInt16 (recvData, offset + 2);
				if (gameState == GameState.Init) {
					medicinePool.Synchronize (recvData, offset);
				}
				offset += 4 + medicineDataSize * medicineDataByte;

				short ballDataSize = BitConverter.ToInt16 (recvData, offset);
				short ballDataByte = BitConverter.ToInt16 (recvData, offset + 2);
				ballPool.Synchronize (recvData, offset);
				offset += 4 + ballDataSize * ballDataByte;

				gameState = GameState.Run;
			}
		}
	}

	// only called by NetworkThread.AsyncReceive
	public void AppendGameStatusFromServer(int index) {
		recvMessageIndex.Enqueue (index);
	}

	public byte[] GetCurrentGameStatus(out int dataSize) {
		dataSize = 4; // reserved for head
		Serializer.ToBytes((short)0, serializedData, ref dataSize); // game result
		Serializer.ToBytes((short)0, serializedData, ref dataSize); // command
		character.Serialize(serializedData, ref dataSize);
		brute.Serialize (serializedData, ref dataSize);
		ghostPool.Serialize (serializedData, ref dataSize);
		submachineBulletPool.Serialize (serializedData, ref dataSize);
		sniperBulletPool.Serialize (serializedData, ref dataSize);
		medicinePool.Serialize (serializedData, ref dataSize);
		return serializedData;
	}
}
