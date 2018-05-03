using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : IPoolObject {

	private Gun gun;
	private short addBulletOwn;
	private AudioSource pickupSound;
	private Transform character;
	private short queryInterval = 3;
	private short currQuery = 0;
	private float distSqrThreshold = 1;
	private Vector3 resetPos = new Vector3(0, -50, 0);

	void Start () {
		character = GameObject.Find ("Character").transform;
		pickupSound = GameObject.Find ("BulletPool").GetComponent<AudioSource> ();
		if (gameObject.name.StartsWith ("Sniper")) {
			gun = character.Find ("Camera/sniper").gameObject.GetComponent<Gun> ();
			addBulletOwn = 10;
		} else {
			gun = character.Find ("Camera/submachinegun").gameObject.GetComponent<Gun> ();
			addBulletOwn = 60;
		}
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
			if ((transform.position - character.position).sqrMagnitude < distSqrThreshold) {
				ret = false;
				gun.AddBulletOwn (addBulletOwn);
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
