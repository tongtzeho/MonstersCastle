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
	private int poolSize = 8;
	private int queryId = 0;
	private float distSqrThreshold = 0.9f*0.9f;

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
		int queryNum = 3;
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
		int num = BitConverter.ToInt16 (recvData, beginIndex);
		if (num == poolSize) {
			freeBullets.Clear ();
			for (int i = 0; i < poolSize; ++i) {
				bullets [i].position = new Vector3 (BitConverter.ToSingle (recvData, beginIndex + 2 + i * 12), BitConverter.ToSingle (recvData, beginIndex + 6 + i * 12), BitConverter.ToSingle (recvData, beginIndex + 10 + i * 12));
				if (bullets [i].position.y < enqueueThresholdY) {
					freeBullets.Enqueue (i);
				}
			}
		}
	}

	public byte[] Serialize() {
		List<byte> result = new List<byte> ();
		result.AddRange (BitConverter.GetBytes ((short)(poolSize)));
		for (int i = 0; i < poolSize; ++i) {
			result.AddRange (BitConverter.GetBytes (bullets [i].position.x));
			result.AddRange (BitConverter.GetBytes (bullets [i].position.y));
			result.AddRange (BitConverter.GetBytes (bullets [i].position.z));
		}
		return result.ToArray ();
	}
}
