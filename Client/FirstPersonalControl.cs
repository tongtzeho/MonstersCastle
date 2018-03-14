using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPersonalControl : MonoBehaviour {

	private CharacterController characterController;
	private Transform cameraTransform;
	private float velocityY;
	private const float gravity = -9.8f;
	private const float maxVelocityY = 30.0f;
	private const float walkVelocity = 1.0f;
	private const float runVelocity = 2.0f;
	private const float jumpVelocity = 5.0f;

	// Use this for initialization
	void Start () {
		characterController = GetComponent<CharacterController> ();
		cameraTransform = transform.Find ("Camera");
		velocityY = 0;
		Cursor.visible = false;
	}

	void Update() {
		float rotationX = Input.GetAxis ("Mouse X");
		float rotationY = Input.GetAxis ("Mouse Y");
		transform.Rotate (0.0f, rotationX, 0.0f);
		cameraTransform.Rotate (-rotationY, 0.0f, 0.0f);
		Vector3 velocity;
		if (!characterController.isGrounded) {
			velocityY += gravity * Time.fixedDeltaTime;
		} else {
			if (Input.GetKey (KeyCode.Space)) {
				velocityY = jumpVelocity;
			} else {
				velocityY = 0.0f;
			}
		}
		if (velocityY < -maxVelocityY) {
			velocityY = -maxVelocityY;
		}
		bool isRunning = Input.GetKey (KeyCode.LeftShift);
		if (Input.GetKey (KeyCode.W)) {
			velocity.z = isRunning ? runVelocity : walkVelocity;
		} else if (Input.GetKey (KeyCode.S)) {
			velocity.z = -(isRunning ? runVelocity : walkVelocity);
		} else {
			velocity.z = 0.0f;
		}
		if (Input.GetKey (KeyCode.A)) {
			velocity.x = -(isRunning ? runVelocity : walkVelocity);
		} else if (Input.GetKey (KeyCode.D)) {
			velocity.x = isRunning ? runVelocity : walkVelocity;
		} else {
			velocity.x = 0.0f;
		}
		velocity.y = velocityY;
		characterController.Move (Quaternion.Euler (transform.eulerAngles) * velocity * Time.fixedDeltaTime);
	}
}
