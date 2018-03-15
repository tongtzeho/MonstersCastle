using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPersonalControl : MonoBehaviour {

	private CharacterController characterController;
	private Transform cameraTransform;
	private Animation sniperAnimation;
	private float velocityY;
	private const float gravity = -9.8f;
	private const float maxVelocityY = 30.0f;
	private const float walkVelocity = 1.0f;
	private const float runVelocity = 2.0f;
	private const float jumpVelocity = 5.0f;
	private int animationState = -1; // -1 for init, 0 for idle, 1 for walk

	// Use this for initialization
	void Start () {
		characterController = GetComponent<CharacterController> ();
		cameraTransform = transform.Find ("Camera");
		velocityY = 0;
		Cursor.visible = false;
		GameObject sniper = transform.Find ("Camera/sniper").gameObject;
		sniperAnimation = sniper.GetComponent<Animation> ();
		PlayAnimation (0);
	}

	void PlayAnimation(int animState) {
		if (animState != animationState) {
			animationState = animState;
			switch (animationState) {
			case 0:
				sniperAnimation.Play ("Idle");
				break;
			case 1:
				sniperAnimation.Play ("Walk");
				break;
			}
		}
	}

	void Update() {
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
			PlayAnimation (1);
		} else {
			PlayAnimation (0);
		}
	}
}
