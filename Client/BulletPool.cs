using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletPool : MonoBehaviour {

	// assigned in editor. submachine and sniper, respectively
	public string prefabName;
	public Gun gun;
	public short addBulletOwn;

	private Transform character;
	private Transform[] bullets;
	private Queue<int> freeBullets = new Queue<int>();
	private Vector3 resetPosition = new Vector3(0, -50, 0);
	private int poolSize = 10;
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
			}
			++queryId;
			if (queryId >= poolSize) {
				queryId = 0;
			}
		}
	}
}
