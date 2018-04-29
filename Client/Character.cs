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

	public void Serialize(byte[] serializedData, ref int offset) {
		int begin = offset;
		Serializer.ToBytes ((short)0, serializedData, ref offset); // character data length
		if (isAlive == 1) {
			Serializer.ToBytes (characterController.transform.position, serializedData, ref offset);
			Serializer.ToBytes (characterController.transform.eulerAngles.y, serializedData, ref offset);
		} else {
			Serializer.ToBytes (defaultPosition, serializedData, ref offset);
			Serializer.ToBytes (0.0f, serializedData, ref offset);
		}
		Serializer.ToBytes (sniper.GetBulletNum(), serializedData, ref offset);
		Serializer.ToBytes (sniper.GetBulletOwn(), serializedData, ref offset);
		Serializer.ToBytes (submachine.GetBulletNum(), serializedData, ref offset);
		Serializer.ToBytes (submachine.GetBulletOwn(), serializedData, ref offset);
		for (int i = 0; i < 4; ++i) {
			Serializer.ToBytes (prop[i], serializedData, ref offset);
		}
		Serializer.ToBytes ((short)(offset - begin - 2), serializedData, ref begin);
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
		float prevRebornTimeLeft = rebornTimeLeft;
		rebornTimeLeft = BitConverter.ToSingle (recvData, beginIndex + 2);
		if (prevRebornTimeLeft > 3.5f && rebornTimeLeft <= 3.5f) {
			System.GC.Collect ();
		}
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
