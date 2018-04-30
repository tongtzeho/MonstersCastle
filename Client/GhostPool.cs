using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostPool : MonoBehaviour {

	private Dictionary<int, short> activeGhost = new Dictionary<int, short> (); // key: GhostId in server, value: GhostId in pool (0 to 31)
	private Ghost[] ghostPool = new Ghost[32];
	private Queue<int> freeIndex = new Queue<int> (); // GhostId in pool (0 to 31)

	private int[] recycleList = new int[64];
	private int numRecycles = 0;

	void Awake () {
		for (int i = 0; i < ghostPool.Length; ++i) {
			GameObject newGhost = Instantiate (Resources.Load ("Prefabs/Ghost") as GameObject, transform);
			newGhost.name = "Ghost" + i.ToString ();
			ghostPool [i] = newGhost.GetComponent<Ghost> ();
			ghostPool [i].Disable ();
			freeIndex.Enqueue (i);
		}
	}

	public void Reset() {
		RecycleUnusedGhosts (new HashSet<int> ());
	}

	public void Serialize(byte[] serializedData, ref int offset) {
		if (activeGhost.Count == 0) {
			Serializer.ToBytes ((short)0, serializedData, ref offset);
			Serializer.ToBytes ((short)0, serializedData, ref offset);
		} else {
			Serializer.ToBytes ((short)activeGhost.Count, serializedData, ref offset);
			bool addByte = false;
			Dictionary<int, short>.ValueCollection values = activeGhost.Values;
			foreach (short i in values) {
				if (!addByte) { // first ghost
					addByte = true;
					int begin = offset;
					Serializer.ToBytes ((short)0, serializedData, ref offset); // data length of a ghost
					ghostPool [i].Serialize (serializedData, ref offset);
					Serializer.ToBytes ((short)(offset - begin - 2), serializedData, ref begin);
				} else {
					ghostPool [i].Serialize (serializedData, ref offset);
				}
			}
		}
	}

	public Ghost GetGhostFromServerId(int id, short hp) {
		if (activeGhost.ContainsKey (id)) {
			return ghostPool [(int)activeGhost [id]];
		} else {
			if (freeIndex.Count == 0) {
				return null;
			} else {
				int index = freeIndex.Dequeue ();
				activeGhost.Add (id, (short)index);
				ghostPool [index].Enable ((short)id, hp);
				return ghostPool[index];
			}
		}
	}

	public void RecycleUnusedGhosts(HashSet<int> usedGhostServerId) {
		numRecycles = 0;
		foreach (KeyValuePair<int, short> kvp in activeGhost) {
			if (!usedGhostServerId.Contains (kvp.Key)) {
				recycleList [numRecycles] = kvp.Key;
				++numRecycles;
				ghostPool [kvp.Value].Disable ();
				freeIndex.Enqueue ((int)kvp.Value);
			}
		}
		for (int i = 0; i < numRecycles; ++i) {
			activeGhost.Remove (recycleList [i]);
		}
	}
}
