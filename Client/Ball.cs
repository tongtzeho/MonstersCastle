using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour {

	Vector3 resetPos = new Vector3 (0, -10, 0);
	Vector3 velocity = new Vector3();
	bool isAlive = false;

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
		}
	}
}
