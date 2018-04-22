using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour {
	public short isAlive = 1;
	public float rebornTimeLeft = 0;
	public short hp = 100;
	public short maxHp = 100;
	public CharacterController characterController; // assigned in editor
	public Gun sniper; // assigned in editor
	public Gun submachine; // assigned in editor
	public short[] prop = new short[4];
	public float[] buffTimeLeft = new float[3];
	private Control control;
	private CharacterSound characterSound;
	private Vector3 defaultPosition;

	void Awake() {
		defaultPosition = characterController.transform.position;
	}

	void Start() {
		control = GetComponent<Control> ();
		characterSound = GetComponent<CharacterSound> ();
	}

	public byte[] Serialize() {
		List<byte> result = new List<byte> ();
		result.AddRange (BitConverter.GetBytes (isAlive));
		result.AddRange (BitConverter.GetBytes (rebornTimeLeft));
		result.AddRange (BitConverter.GetBytes (hp));
		result.AddRange (BitConverter.GetBytes (maxHp));
		if (isAlive == 1) {
			result.AddRange (BitConverter.GetBytes (characterController.transform.position.x));
			result.AddRange (BitConverter.GetBytes (characterController.transform.position.y));
			result.AddRange (BitConverter.GetBytes (characterController.transform.position.z));
			result.AddRange (BitConverter.GetBytes (characterController.transform.eulerAngles.y));
		} else {
			result.AddRange (BitConverter.GetBytes (defaultPosition.x));
			result.AddRange (BitConverter.GetBytes (defaultPosition.y));
			result.AddRange (BitConverter.GetBytes (defaultPosition.z));
			result.AddRange (BitConverter.GetBytes (0.0f));
		}
		result.AddRange (BitConverter.GetBytes (sniper.GetBulletNum()));
		result.AddRange (BitConverter.GetBytes (sniper.GetBulletOwn()));
		result.AddRange (BitConverter.GetBytes (submachine.GetBulletNum()));
		result.AddRange (BitConverter.GetBytes (submachine.GetBulletOwn()));
		for (int i = 0; i < 4; ++i) {
			result.AddRange (BitConverter.GetBytes (prop[i]));
		}
		for (int i = 0; i < 3; ++i) {
			result.AddRange (BitConverter.GetBytes (buffTimeLeft[i]));
		}
		return result.ToArray ();
	}

	public void UpdateFromServer(bool gameInitializing, byte[] recvData, int beginIndex, int length) {
		if (control == null) {
			control = GetComponent<Control> ();
		}
		short isCurrAlive = BitConverter.ToInt16 (recvData, beginIndex);
		if (isCurrAlive == 1 && (isAlive == 0 || gameInitializing)) { // reborn
			control.Reset ();
			control.Allow ();
			characterController.transform.position = new Vector3(BitConverter.ToSingle (recvData, beginIndex + 10), BitConverter.ToSingle (recvData, beginIndex + 14), BitConverter.ToSingle (recvData, beginIndex + 18));
			characterController.transform.rotation = Quaternion.Euler (0, BitConverter.ToSingle (recvData, beginIndex + 22), 0);
		} else if (isCurrAlive == 0 && (isAlive == 1 || gameInitializing)) {
			control.Disallow ();
			characterController.transform.position = new Vector3 (0, 20, 55);
			characterController.transform.rotation = Quaternion.Euler (0, 180, 0);
			control.SetDeadCamera ();
		}
		if (isCurrAlive == 0 && isAlive == 1 && !gameInitializing) {
			characterSound.PlayDieSound ();
		} else if (isCurrAlive == 1 && (isAlive == 0 || gameInitializing)) {
			characterSound.PlayRebornSound ();
		}
		isAlive = isCurrAlive;
		rebornTimeLeft = BitConverter.ToSingle (recvData, beginIndex + 2);
		short currHp = BitConverter.ToInt16 (recvData, beginIndex + 6);
		if (currHp < hp && !gameInitializing) {
			characterSound.PlayHurtSound ();
		}
		hp = currHp;
		if (gameInitializing) {
			maxHp = BitConverter.ToInt16 (recvData, beginIndex + 8);
			sniper.SetBulletNum (BitConverter.ToInt16 (recvData, beginIndex + 26));
			sniper.SetBulletOwn (BitConverter.ToInt16 (recvData, beginIndex + 28));
			submachine.SetBulletNum (BitConverter.ToInt16 (recvData, beginIndex + 30));
			submachine.SetBulletOwn (BitConverter.ToInt16 (recvData, beginIndex + 32));
			for (int i = 0; i < 4; ++i) {
				prop [i] = BitConverter.ToInt16 (recvData, beginIndex + 34 + i * 2);
			}
		}
	}
}
