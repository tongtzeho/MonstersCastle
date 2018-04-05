using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPersonalControl : MonoBehaviour {

	private CharacterController characterController;
	private Transform cameraTransform;
	private Gun sniper;
	private Gun submachineGun;
	private float velocityY;
	private const float gravity = -9.8f;
	private const float maxVelocityY = 30.0f;
	private const float walkVelocity = 1.0f;
	private const float runVelocity = 2.0f;
	private const float jumpVelocity = 5.0f;
	private Gun activeGun;
	private Gun inactiveGun;

	void Start () {
		characterController = GetComponent<CharacterController> ();
		cameraTransform = transform.Find ("Camera");
		velocityY = 0;
		Cursor.visible = false;
		sniper = transform.Find ("Camera/sniper").gameObject.GetComponent<Gun> ();
		submachineGun = transform.Find ("Camera/submachinegun").gameObject.GetComponent<Gun> ();
		activeGun = sniper;
		inactiveGun = submachineGun;
		activeGun.GetAnimator ().SetState (0, 1);
		inactiveGun.GetAnimator ().SetState (0, 0);
	}

	void SwitchGun() {
		if (activeGun == sniper) {
			activeGun = submachineGun;
			inactiveGun = sniper;
		} else {
			activeGun = sniper;
			inactiveGun = submachineGun;
		}
		inactiveGun.GetAnimator ().SetState (0, 0);
	}

	void Update() {
		if (Input.GetButtonDown ("SwitchGun")) {
			SwitchGun ();
		}
		float rotationX = Input.GetAxis ("Mouse X");
		float rotationY = Input.GetAxis ("Mouse Y");
		transform.Rotate (0.0f, rotationX, 0.0f);
		cameraTransform.Rotate (-rotationY, 0.0f, 0.0f);
		Vector3 velocity;
		bool isOnGround = characterController.isGrounded;
		if (!isOnGround) {
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
		bool isWalking = false;
		if (Input.GetKey (KeyCode.W)) {
			velocity.z = isRunning ? runVelocity : walkVelocity;
			isWalking = true;
		} else if (Input.GetKey (KeyCode.S)) {
			velocity.z = -(isRunning ? runVelocity : walkVelocity);
			isWalking = true;
		} else {
			velocity.z = 0.0f;
		}
		if (Input.GetKey (KeyCode.A)) {
			velocity.x = -(isRunning ? runVelocity : walkVelocity);
			isWalking = true;
		} else if (Input.GetKey (KeyCode.D)) {
			velocity.x = isRunning ? runVelocity : walkVelocity;
			isWalking = true;
		} else {
			velocity.x = 0.0f;
		}
		velocity.y = velocityY;
		characterController.Move (Quaternion.Euler (transform.eulerAngles) * velocity * Time.fixedDeltaTime);
		if (isWalking) {
			if (isRunning) {
				activeGun.GetAnimator ().SetState (2, 1);
			} else {
				activeGun.GetAnimator ().SetState (1, 1);
			}
		} else {
			activeGun.GetAnimator ().SetState (0, 1);
		}
	}
}
