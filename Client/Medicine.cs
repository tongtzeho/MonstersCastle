using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Medicine : IPoolObject {

	private AudioSource pickupSound;
	private Character character;
	private Transform characterTransform;
	private short queryInterval = 3;
	private short currQuery = 0;
	private float distSqrThreshold = 1;
	private Vector3 resetPos = new Vector3(0, -50, 0);

	void Start () {
		characterTransform = GameObject.Find ("Character").transform;
		character = characterTransform.gameObject.GetComponent<Character> ();
		pickupSound = GameObject.Find ("BulletPool").GetComponent<AudioSource> ();
	}

	public override void Enable(byte[] recvData, int beginIndex) {
		base.Enable (recvData, beginIndex);
		transform.position = new Vector3 (BitConverter.ToSingle (recvData, beginIndex + 2), BitConverter.ToSingle (recvData, beginIndex + 6), BitConverter.ToSingle (recvData, beginIndex + 10));
	}

	public override void Disable() {
		base.Disable ();
		transform.position = resetPos;
	}

	public override void Serialize (byte[] serializedData, ref int offset) {
		Serializer.ToBytes (transform.position, serializedData, ref offset);
	}

	public override void Synchronize (byte[] recvData, int beginIndex) {
		return; // no need
	}

	public override bool Step() {
		bool ret = true;
		if (currQuery == 0) {
			if ((transform.position - characterTransform.position).sqrMagnitude < distSqrThreshold) {
				ret = false;
				character.AddMedicine ();
				pickupSound.Play ();
			}
		}
		++currQuery;
		if (currQuery == queryInterval) {
			currQuery = 0;
		}
		return ret;
	}
}
