using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IPoolObject : MonoBehaviour {

	private bool isEnabled = false;

	public bool IsEnabled() {
		return isEnabled;
	}

	public virtual void Enable(byte[] data, int beginIndex) {
		isEnabled = true;
	}

	public virtual void Disable() {
		isEnabled = false;
	}

	public abstract void Serialize (byte[] serializedData, ref int offset); // called by ObjectPool.cs Serialize

	public abstract void Synchronize (byte[] recvData, int beginIndex); // called by ObjectPool.cs Synchronize

	public abstract bool Step(); // called by ObjectPool.cs Update
}
