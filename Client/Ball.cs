using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : IPoolObject {

	private Vector3 resetPos = new Vector3 (0, -10, 0);
	private Vector3 velocity = new Vector3();

	public override void Enable(byte[] recvData, int beginIndex) {
		base.Enable (recvData, beginIndex);
		transform.position = new Vector3 (BitConverter.ToSingle (recvData, beginIndex + 2), BitConverter.ToSingle (recvData, beginIndex + 6), BitConverter.ToSingle (recvData, beginIndex + 10));
		velocity = new Vector3 (BitConverter.ToSingle (recvData, beginIndex + 14), BitConverter.ToSingle (recvData, beginIndex + 18), BitConverter.ToSingle (recvData, beginIndex + 22));
	}

	public override void Disable() {
		base.Disable ();
		transform.position = resetPos;
		velocity = Vector3.zero;
	}

	public override void Serialize (byte[] serializedData, ref int offset) {
		return; // no need
	}

	public override void Synchronize (byte[] recvData, int beginIndex) {
		return; // no need
	}

	public override bool Step() {
		transform.position += velocity * Time.deltaTime;
		return true;
	}
}
