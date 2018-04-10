using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brute : MonoBehaviour {
	public short isAlive = 0;
	public short level = 0;
	public Monster monster = new Monster ();
	public short action = 0;
	public Animator animator; // assigned in editor
	private float dieAnimationTotalTime = 1.3f;
	private float dieAnimationCurrTime = 0;

	public byte[] Serialize() {
		List<byte> result = new List<byte> ();
		result.AddRange (BitConverter.GetBytes (isAlive));
		result.AddRange (BitConverter.GetBytes (level));
		result.AddRange (BitConverter.GetBytes (monster.hp));
		result.AddRange (BitConverter.GetBytes (monster.maxHp));
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
		isAlive = isCurrAlive;
		if (gameInitializing || isReborn) {
			monster.hp = BitConverter.ToInt16 (recvData, beginIndex + 4);
			monster.maxHp = BitConverter.ToInt16 (recvData, beginIndex + 6);
		}
		if (isAlive == 1) {
			transform.position = new Vector3 (BitConverter.ToSingle (recvData, beginIndex + 8), BitConverter.ToSingle (recvData, beginIndex + 12), BitConverter.ToSingle (recvData, beginIndex + 16));
			transform.rotation = Quaternion.Euler (0, BitConverter.ToSingle (recvData, beginIndex + 20), 0);
			dieAnimationCurrTime = 0;
		} else {
			dieAnimationCurrTime += Time.deltaTime;
			if (dieAnimationCurrTime > dieAnimationTotalTime) {
				transform.position = new Vector3 (0, -20, -30);
			}
		}
		action = BitConverter.ToInt16 (recvData, beginIndex + 24);
		SetAnimationAction (action);
	}

	void SetAnimationAction(short action) {
		animator.SetInteger ("Action", (int)action);
	}
}
