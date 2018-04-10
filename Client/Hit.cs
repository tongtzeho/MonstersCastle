using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hit : MonoBehaviour {

	private Hashtable colliderTable = new Hashtable();

	void Start () {
		GameObject bruteObject = GameObject.Find ("Brute");
		Monster brute = bruteObject.GetComponent<Brute> ().monster;
		Collider[] bruteCollider = bruteObject.GetComponentsInChildren<Collider> ();
		for (int i = 0; i < bruteCollider.Length; ++i) {
			colliderTable.Add (bruteCollider [i], brute);
		}
	}

	public void HitCollider(short atk, Collider collider) {
		if (colliderTable.Contains (collider)) {
			((Monster)colliderTable [collider]).Hit (atk);
		}
	}
}
