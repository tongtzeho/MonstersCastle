using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour {

	private Vector3 resetPos = new Vector3 (0, -10, 0);
	private Vector3 velocity = new Vector3();
	private bool isAlive = false;
	private float rotation = 150.0f;

	void Start() {
		transform.position = resetPos;
	}

	public bool isEnabled() {
		return isAlive;
	}

	public void Enable(Vector3 position, Vector3 velocity) {
		isAlive = true;
		transform.position = position;
		this.velocity = velocity;
	}

	public void Disable() {
		isAlive = false;
		transform.position = resetPos;
	}

	void Update () {
		if (isAlive) {
			transform.position += velocity * Time.deltaTime;
			transform.Rotate (0, rotation * Time.deltaTime, 0);
		}
	}
}
