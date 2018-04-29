using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallPool : MonoBehaviour {

	private Dictionary<int, short> activeBall = new Dictionary<int, short> (); // key: BallId in server, value: BallId in pool (0 to 31)
	private Ball[] ballPool = new Ball[32];
	private Queue freeIndex = new Queue (); // BallId in pool (0 to 31)

	private int[] recycleList = new int[64];
	private int numRecycles = 0;

	void Awake () {
		for (int i = 0; i < ballPool.Length; ++i) {
			GameObject newBall = Instantiate (Resources.Load ("Prefabs/Ball") as GameObject, transform);
			newBall.name = "Ball" + i.ToString ();
			ballPool [i] = newBall.GetComponent<Ball> ();
			ballPool [i].Disable ();
			freeIndex.Enqueue (i);
		}
	}

	public void Reset() {
		RecycleUnusedBalls (new HashSet<int> ());
	}

	public bool Contains(int serverId) {
		return activeBall.ContainsKey (serverId);
	}

	public Ball Create(int serverId, byte[] recvData, int beginIndex) {
		if (freeIndex.Count == 0) {
			return null;
		} else {
			int index = (int)freeIndex.Dequeue ();
			activeBall.Add (serverId, (short)index);
			Vector3 position = new Vector3 (BitConverter.ToSingle (recvData, beginIndex), BitConverter.ToSingle (recvData, beginIndex + 4), BitConverter.ToSingle (recvData, beginIndex + 8));
			Vector3 velocity = new Vector3 (BitConverter.ToSingle (recvData, beginIndex + 12), BitConverter.ToSingle (recvData, beginIndex + 16), BitConverter.ToSingle (recvData, beginIndex + 20));
			ballPool [index].Enable (position, velocity);
			return ballPool [index];
		}
	}

	public void RecycleUnusedBalls(HashSet<int> usedBallServerId) {
		numRecycles = 0;
		foreach (KeyValuePair<int, short> kvp in activeBall) {
			if (!usedBallServerId.Contains (kvp.Key)) {
				recycleList [numRecycles] = kvp.Key;
				++numRecycles;
				ballPool [kvp.Value].Disable ();
				freeIndex.Enqueue ((int)kvp.Value);
			}
		}
		for (int i = 0; i < numRecycles; ++i) {
			activeBall.Remove (recycleList [i]);
		}
	}
}
