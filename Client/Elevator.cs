using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elevator : MonoBehaviour {

	private float minY = 0;
	private float maxY = 4.0f;
	private float velocity = 1.5f;
	private Vector3 downVelocity;
	private Vector3 upVelocity;
	private bool downing = true;

	private Transform rigidbodyTransform;

	void Start() {
		downVelocity = new Vector3(0, -velocity, 0);
		upVelocity = new Vector3 (0, velocity, 0);
		rigidbodyTransform = GetComponent<Rigidbody> ().transform;
	}

	void Update() {
		if (downing && transform.position.y <= minY) {
			downing = false;
		} else if (!downing && transform.position.y >= maxY) {
			downing = true;
		}
		if (downing) {
			rigidbodyTransform.Translate (downVelocity * Time.deltaTime);
		} else {
			rigidbodyTransform.Translate (upVelocity * Time.deltaTime);
		}
	}
}
