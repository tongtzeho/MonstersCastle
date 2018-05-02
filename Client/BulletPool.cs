using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletPool : MonoBehaviour {

	// assigned in editor. submachine and sniper, respectively
	public string prefabName;
	public Gun gun;
	public short addBulletOwn;

	private AudioSource pickupSound;

	private Transform character;
	private Transform[] bullets;
	private Queue<int> freeBullets = new Queue<int>();
	private float enqueueThresholdY = -40;
	private Vector3 resetPosition = new Vector3(0, -50, 0);
	private int poolSize = 12;
	private int queryId = 0;
	private float distSqrThreshold = 1;

	void Awake () {
		bullets = new Transform[poolSize];
		for (int i = 0; i < poolSize; ++i) {
			GameObject newBullet = Instantiate (Resources.Load ("Prefabs/" + prefabName) as GameObject, transform);
			newBullet.name = prefabName + i.ToString ();
			bullets [i] = newBullet.transform;
		}
		Reset ();
	}

	void Start() {
		character = GameObject.Find ("Character").transform;
		pickupSound = GetComponent<AudioSource> ();
	}

	public void Reset() {
		freeBullets.Clear ();
		for (int i = 0; i < bullets.Length; ++i) {
			bullets [i].position = resetPosition;
			freeBullets.Enqueue (i);
		}
	}

	public bool Occur(Vector3 position) {
		if (freeBullets.Count > 0) {
			int i = freeBullets.Dequeue ();
			bullets [i].position = position;
			return true;
		} else {
			return false;
		}
	}

	void Update () {
		int queryNum = 4;
		for (int i = 0; i < queryNum; ++i) {
			if ((bullets [queryId].position - character.position).sqrMagnitude < distSqrThreshold) {
				gun.AddBulletOwn (addBulletOwn);
				bullets[queryId].position = resetPosition;
				freeBullets.Enqueue (queryId);
				pickupSound.Play ();
			}
			++queryId;
			if (queryId >= poolSize) {
				queryId = 0;
			}
		}
	}

	// only updates when firstly connect to a game
	public void UpdateFromServer (byte[] recvData, int beginIndex, int length) {
		int numBullet = (int)BitConverter.ToInt16 (recvData, beginIndex);
		freeBullets.Clear ();
		for (int i = 0; i < numBullet; ++i) {
			bullets [i].position = new Vector3 (BitConverter.ToSingle (recvData, beginIndex + 2 + i * 12), BitConverter.ToSingle (recvData, beginIndex + 6 + i * 12), BitConverter.ToSingle (recvData, beginIndex + 10 + i * 12));
		}
		for (int i = numBullet; i < poolSize; ++i) {
			bullets [i].position = resetPosition;
			freeBullets.Enqueue (i);
		}
	}

	public void Serialize(byte[] serializedData, ref int offset) {
		int begin = offset;
		short numBullet = 0;
		Serializer.ToBytes ((short)0, serializedData, ref offset); // the number of bullets
		for (int i = 0; i < poolSize; ++i) {
			if (bullets [i].position.y > enqueueThresholdY) {
				Serializer.ToBytes (bullets [i].position, serializedData, ref offset);
				++numBullet;
			}
		}
		Serializer.ToBytes (numBullet, serializedData, ref begin);
	}
}
