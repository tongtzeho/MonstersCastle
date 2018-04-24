using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostPool : MonoBehaviour {

	private Hashtable activeGhost = new Hashtable (); // key: GhostId in server, value: GhostId in pool (0 to 29)
	private Ghost[] ghostPool = new Ghost[30];
	private Queue freeIndex = new Queue (); // GhostId in pool (0 to 29)

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

	public byte[] Serialize() {
		List<byte> result = new List<byte> ();
		if (activeGhost.Count == 0) {
			result.AddRange (BitConverter.GetBytes ((short)0));
			result.AddRange (BitConverter.GetBytes ((short)0));
		} else {
			result.AddRange (BitConverter.GetBytes ((short)activeGhost.Count));
			bool addByte = false;
			IDictionaryEnumerator enumerator = activeGhost.GetEnumerator();
			bool next = enumerator.MoveNext ();
			while (next) {
				List<byte> ghostResult = ghostPool [(int)enumerator.Value].Serialize ();
				if (!addByte) {
					addByte = true;
					result.AddRange (BitConverter.GetBytes ((short)ghostResult.Count));
				}
				result.AddRange (ghostResult);
				next = enumerator.MoveNext ();
			}
		}
		return result.ToArray ();
	}

	public Ghost GetGhostFromServerId(int id, short hp) {
		if (activeGhost.Contains (id)) {
			return ghostPool [(int)activeGhost [id]];
		} else {
			if (freeIndex.Count == 0) {
				return null;
			} else {
				int index = (int)freeIndex.Dequeue ();
				activeGhost.Add (id, index);
				ghostPool [index].Enable ((short)id, hp);
				return ghostPool[index];
			}
		}
	}

	public void RecycleUnusedGhosts(HashSet<int> usedGhostServerId) {
		IDictionaryEnumerator enumerator = activeGhost.GetEnumerator();
		List<int> recycleList = new List<int> ();
		bool next = enumerator.MoveNext ();
		while (next) {
			if (!usedGhostServerId.Contains ((int)enumerator.Key)) {
				recycleList.Add ((int)enumerator.Key);
				ghostPool [(int)enumerator.Value].Disable ();
				freeIndex.Enqueue((int)enumerator.Value);
			}
			next = enumerator.MoveNext ();
		}
		for (int i = 0; i < recycleList.Count; ++i) {
			activeGhost.Remove (recycleList [i]);
		}
	}
}
