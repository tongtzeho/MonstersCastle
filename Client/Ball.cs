using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball {

	private Vector3 resetPos = new Vector3 (0, -10, 0);
	private Vector3 velocity = new Vector3();
	private bool isAlive = false;
	private Transform transform = null;

	public Ball(Transform transform) {
		this.transform = transform;
		this.transform.position = resetPos;
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

	public void Move () {
		transform.position += velocity * Time.deltaTime;
	}
}
