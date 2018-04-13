using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost : MonoBehaviour {

	public short serverId;
	public Monster monster = new Monster();
	public short action;
	private GhostAnimator ghostAnimator;

	void Start() {
		ghostAnimator = GetComponent<GhostAnimator> ();
	}

	public List<byte> Serialize() {
		List<byte> result = new List<byte> ();
		result.AddRange (BitConverter.GetBytes (serverId));
		result.AddRange (BitConverter.GetBytes (monster.hp));
		result.AddRange (BitConverter.GetBytes (monster.maxHp));
		result.AddRange (BitConverter.GetBytes (transform.position.x));
		result.AddRange (BitConverter.GetBytes (transform.position.y));
		result.AddRange (BitConverter.GetBytes (transform.position.z));
		result.AddRange (BitConverter.GetBytes (transform.eulerAngles.y));
		result.AddRange (BitConverter.GetBytes (action));
		return result;
	}

	public void Enable(short sid, short hp) {
		serverId = sid;
		monster.hp = hp;
		action = 2;
	}

	public void Disable() {
		monster.hp = 0;
		transform.position = new Vector3 (0, -50, -50);
		action = 2;
	}

	public void UpdateFromServer (byte[] recvData, int beginIndex, int length) {
		monster.maxHp = BitConverter.ToInt16 (recvData, beginIndex + 4);
		action = BitConverter.ToInt16 (recvData, beginIndex + 22);
		if (action == 5) {
			monster.hp = 0;
		}
		if (monster.hp > 0) {
			transform.position = new Vector3 (BitConverter.ToSingle (recvData, beginIndex + 6), BitConverter.ToSingle (recvData, beginIndex + 10), BitConverter.ToSingle (recvData, beginIndex + 14));
			transform.rotation = Quaternion.Euler (0, BitConverter.ToSingle (recvData, beginIndex + 18), 0);
		} else {
			transform.position = new Vector3 (BitConverter.ToSingle (recvData, beginIndex + 6), BitConverter.ToSingle (recvData, beginIndex + 10) - 50.0f, BitConverter.ToSingle (recvData, beginIndex + 14));
		}
		ghostAnimator.SetState (action);
	}
}
