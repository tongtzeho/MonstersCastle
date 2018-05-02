using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MedicinePool : MonoBehaviour {

	private Character character;
	private AudioSource pickupSound;
	private Transform characterTransform;
	private Transform[] medicines;
	private Queue<int> freeMedicines = new Queue<int>();
	private float enqueueThresholdY = -40;
	private Vector3 resetPosition = new Vector3(0, -50, 0);
	private int poolSize = 5;
	private int queryId = 0;
	private float distSqrThreshold = 1;

	void Awake () {
		medicines = new Transform[poolSize];
		for (int i = 0; i < poolSize; ++i) {
			medicines [i] = transform.Find ("Medicine" + (i + 1).ToString ());
		}
		Reset ();
	}

	void Start() {
		characterTransform = GameObject.Find ("Character").transform;
		character = characterTransform.gameObject.GetComponent<Character> ();
		pickupSound = GetComponent<AudioSource> ();
	}

	public void Reset() {
		freeMedicines.Clear ();
		for (int i = 0; i < medicines.Length; ++i) {
			medicines [i].position = resetPosition;
			freeMedicines.Enqueue (i);
		}
	}

	public bool Occur(Vector3 position) {
		if (freeMedicines.Count > 0) {
			int i = freeMedicines.Dequeue ();
			medicines [i].position = position;
			return true;
		} else {
			return false;
		}
	}
		
	void Update () {
		if ((medicines [queryId].position - characterTransform.position).sqrMagnitude < distSqrThreshold) {
			character.AddMedicine ();
			medicines [queryId].position = resetPosition;
			freeMedicines.Enqueue (queryId);
			pickupSound.Play ();
		}
		++queryId;
		if (queryId >= poolSize) {
			queryId = 0;
		}
	}

	// only updates when firstly connect to a game
	public void UpdateFromServer (byte[] recvData, int beginIndex, int length) {
		int numMedicine = (int)BitConverter.ToInt16 (recvData, beginIndex);
		freeMedicines.Clear ();
		for (int i = 0; i < numMedicine; ++i) {
			medicines [i].position = new Vector3 (BitConverter.ToSingle (recvData, beginIndex + 2 + i * 12), BitConverter.ToSingle (recvData, beginIndex + 6 + i * 12), BitConverter.ToSingle (recvData, beginIndex + 10 + i * 12));
		}
		for (int i = numMedicine; i < poolSize; ++i) {
			medicines [i].position = resetPosition;
			freeMedicines.Enqueue (i);
		}
	}

	public void Serialize(byte[] serializedData, ref int offset) {
		int begin = offset;
		short numMedicines = 0;
		Serializer.ToBytes ((short)0, serializedData, ref offset); // the number of medicines
		for (int i = 0; i < poolSize; ++i) {
			if (medicines [i].position.y > enqueueThresholdY) {
				Serializer.ToBytes (medicines [i].position, serializedData, ref offset);
				++numMedicines;
			}
		}
		Serializer.ToBytes (numMedicines, serializedData, ref begin);
	}
}
