using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour {

	private IPoolObject[] pool;
	private Dictionary<short, short> activeObjects = new Dictionary<short, short> (); // Key: Unique Id (Server Id), Value: Index in Pool Array
	private Queue<short> freeObjects = new Queue<short>(); // Unused Index in Pool Array

	private short[] recycleObjects;
	private int numRecycle = 0;
	private HashSet<short> usedObjects = new HashSet<short>();

	// assigned in editor
	public int poolSize;
	public string prefabName;
	public string componentName;

	void Awake () {
		pool = new IPoolObject[poolSize];
		recycleObjects = new short[poolSize];
		for (int i = 0; i < poolSize; ++i) {
			GameObject newObject = Instantiate (Resources.Load (prefabName) as GameObject, transform);
			pool [i] = (IPoolObject)newObject.GetComponent (componentName);
		}
		Reset ();
	}

	public void Reset() {
		activeObjects.Clear ();
		freeObjects.Clear ();
		for (int i = 0; i < poolSize; ++i) {
			pool [i].Disable ();
			freeObjects.Enqueue ((short)i);
		}
	}

	public short Create(short uniqueId, byte[] data, int beginIndex) {
		if (freeObjects.Count == 0) {
			return -1;
		} else {
			short id = freeObjects.Dequeue ();
			activeObjects.Add (uniqueId, id);
			pool [id].Enable (data, beginIndex);
			return id;
		}
	}

	private void Release(short uniqueId) {
		short id = activeObjects [uniqueId];
		activeObjects.Remove (uniqueId);
		freeObjects.Enqueue (id);
		pool [id].Disable ();
	}

	public void Serialize(byte[] serializedData, ref int offset) {
		if (activeObjects.Count == 0) {
			Serializer.ToBytes ((short)0, serializedData, ref offset);
			Serializer.ToBytes ((short)0, serializedData, ref offset);
		} else {
			Serializer.ToBytes ((short)activeObjects.Count, serializedData, ref offset);
			bool addByte = false;
			foreach (KeyValuePair<short, short> kvp in activeObjects) {
				if (!addByte) { // first ghost
					addByte = true;
					int begin = offset;
					Serializer.ToBytes ((short)0, serializedData, ref offset); // data length of a ghost
					Serializer.ToBytes (kvp.Key, serializedData, ref offset);
					pool [kvp.Value].Serialize (serializedData, ref offset);
					Serializer.ToBytes ((short)(offset - begin - 2), serializedData, ref begin);
				} else {
					Serializer.ToBytes (kvp.Key, serializedData, ref offset);
					pool [kvp.Value].Serialize (serializedData, ref offset);
				}
			}
		}
	}

	public void Synchronize (byte[] recvData, int beginIndex) {
		int offset = beginIndex;
		short dataSize = BitConverter.ToInt16 (recvData, offset);
		short dataByte = BitConverter.ToInt16 (recvData, offset + 2);
		offset += 4;
		usedObjects.Clear ();
		for (int i = 0; i < dataSize; ++i) {
			short uniqueId = BitConverter.ToInt16 (recvData, offset);
			if (activeObjects.ContainsKey (uniqueId)) {
				pool [activeObjects [uniqueId]].Synchronize (recvData, offset);
			} else {
				short id = Create (uniqueId, recvData, offset);
				if (id != -1) {
					pool [id].Synchronize (recvData, offset);
				}
			}
			offset += dataByte;
			usedObjects.Add (uniqueId);
		}
		Recycle (usedObjects);
	}

	private void Recycle (HashSet<short> usedObjects) {
		numRecycle = 0;
		foreach (KeyValuePair<short, short> kvp in activeObjects) {
			if (!usedObjects.Contains (kvp.Key)) {
				recycleObjects [numRecycle] = kvp.Key;
				++numRecycle;
			}
		}
		for (int i = 0; i < numRecycle; ++i) {
			Release (recycleObjects [i]);
		}
	}

	void Update () {
		numRecycle = 0;
		foreach (KeyValuePair<short, short> kvp in activeObjects) {
			if (!pool [kvp.Value].Step ()) {
				recycleObjects [numRecycle] = kvp.Key;
				++numRecycle;
			}
		}
		for (int i = 0; i < numRecycle; ++i) {
			Release (recycleObjects [i]);
		}
	}
}
