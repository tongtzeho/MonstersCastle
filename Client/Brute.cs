using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brute : MonoBehaviour {
	public short isAlive = 0;
	public short level = 0;
	public short hp = 0;
	public short maxHp = 1000;
	public short action = 0;

	private Animator animator;

	void Start() {
		animator = GetComponent<Animator> ();
	}

	public byte[] Serialize() {
		List<byte> result = new List<byte> ();
		result.AddRange (BitConverter.GetBytes (isAlive));
		result.AddRange (BitConverter.GetBytes (level));
		result.AddRange (BitConverter.GetBytes (hp));
		result.AddRange (BitConverter.GetBytes (maxHp));
		result.AddRange (BitConverter.GetBytes (transform.position.x));
		result.AddRange (BitConverter.GetBytes (transform.position.y));
		result.AddRange (BitConverter.GetBytes (transform.position.z));
		result.AddRange (BitConverter.GetBytes (transform.eulerAngles.y));
		result.AddRange (BitConverter.GetBytes (action));
		return result.ToArray ();
	}

	public void UpdateFromServer (bool gameInitializing, byte[] recvData, int beginIndex, int length) {
		short isCurrAlive = BitConverter.ToInt16 (recvData, beginIndex);
		level = BitConverter.ToInt16 (recvData, beginIndex + 2);
		bool isReborn = (isCurrAlive == 1 && isAlive == 0 && !gameInitializing);
		bool isDying = (isCurrAlive == 0 && isAlive == 1 && !gameInitializing);
		SetReborn (isReborn);
		SetDying (isDying);
		isAlive = isCurrAlive;
		if (gameInitializing || isReborn) {
			hp = BitConverter.ToInt16 (recvData, beginIndex + 4);
			maxHp = BitConverter.ToInt16 (recvData, beginIndex + 6);
		}
		if (isAlive == 1) {
			transform.position = new Vector3 (BitConverter.ToSingle (recvData, beginIndex + 8), BitConverter.ToSingle (recvData, beginIndex + 12), BitConverter.ToSingle (recvData, beginIndex + 16));
			transform.rotation = Quaternion.Euler (0, BitConverter.ToSingle (recvData, beginIndex + 20), 0);
		} else {
			transform.position = new Vector3 (0, -20, -30);
		}
		action = BitConverter.ToInt16 (recvData, beginIndex + 24);
		if (isAlive == 1 && action != 1) {
			SetStay (true);
		} else {
			SetStay (false);
		}
	}

	void SetReborn(bool r) {
		animator.SetBool ("Reborn", r);
	}

	void SetStay(bool s) {
		animator.SetBool ("Stay", s);
	}

	void SetDying(bool d) {
		animator.SetBool ("Dying", d);
	}
}
