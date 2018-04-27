using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallPool : MonoBehaviour {

	private Hashtable activeBall = new Hashtable (); // key: BallId in server, value: BallId in pool (0 to 39)
	private Ball[] ballPool = new Ball[40];
	private Queue freeIndex = new Queue (); // BallId in pool (0 to 39)

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
		return activeBall.Contains (serverId);
	}

	public Ball Create(int serverId, Vector3 position, Vector3 velocity) {
		if (freeIndex.Count == 0) {
			return null;
		} else {
			int index = (int)freeIndex.Dequeue ();
			activeBall.Add (serverId, index);
			ballPool [index].Enable (position, velocity);
			return ballPool [index];
		}
	}

	public void RecycleUnusedBalls(HashSet<int> usedBallServerId) {
		IDictionaryEnumerator enumerator = activeBall.GetEnumerator();
		List<int> recycleList = new List<int> ();
		bool next = enumerator.MoveNext ();
		while (next) {
			if (!usedBallServerId.Contains ((int)enumerator.Key)) {
				recycleList.Add ((int)enumerator.Key);
				ballPool [(int)enumerator.Value].Disable ();
				freeIndex.Enqueue((int)enumerator.Value);
			}
			next = enumerator.MoveNext ();
		}
		for (int i = 0; i < recycleList.Count; ++i) {
			activeBall.Remove (recycleList [i]);
		}
	}
}
