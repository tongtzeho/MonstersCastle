using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Control : MonoBehaviour {

	public enum CharacterState {
		Idle = 0, Walk = 1, Run = 2
	}

	private CharacterController characterController;
	private Transform cameraTransform;
	private Gun sniper;
	private Gun submachineGun;
	private float velocityY;
	private const float gravity = -9.8f;
	private const float maxVelocityY = 30.0f;
	private const float walkVelocity = 2.5f;
	private const float runVelocity = 5.5f;
	private const float jumpVelocity = 4.5f;
	private Gun activeGun;
	private Gun inactiveGun;
	private WalkingSound walkingSound;

	void Start () {
		characterController = GetComponent<CharacterController> ();
		cameraTransform = transform.Find ("Camera");
		velocityY = 0;
		Cursor.visible = false;
		GameObject sniperObject = transform.Find ("Camera/sniper").gameObject;
		sniper = sniperObject.GetComponent<Gun> ();
		GameObject submachineObject = transform.Find ("Camera/submachinegun").gameObject;
		submachineGun = submachineObject.GetComponent<Gun> ();
		activeGun = sniper;
		inactiveGun = submachineGun;
		walkingSound = GetComponent<WalkingSound> ();
	}

	public void SwitchGun() {
		if (activeGun == sniper) {
			activeGun = submachineGun;
			inactiveGun = sniper;
		} else {
			activeGun = sniper;
			inactiveGun = submachineGun;
		}
		inactiveGun.GetAnimator ().SetState (CharacterState.Idle, Gun.GunState.Hide);
		inactiveGun.ResetTime ();
	}

	public Gun GetActiveGun() {
		return activeGun;
	}

	void Update() {
		if (Input.GetButtonDown ("SwitchGun")) {
			SwitchGun ();
		}
		bool pressFire = Input.GetKey (KeyCode.Mouse0);
		bool pressReload = Input.GetKey (KeyCode.R);
		Gun.GunState gunState = activeGun.Action (pressFire, pressReload);
		float rotationX = Input.GetAxis ("Mouse X");
		float rotationY = Input.GetAxis ("Mouse Y");
		transform.Rotate (0.0f, rotationX, 0.0f);
		cameraTransform.Rotate (-rotationY, 0.0f, 0.0f);
		Vector3 velocity;
		bool isOnGround = characterController.isGrounded;
		if (!isOnGround) {
			velocityY += gravity * Time.deltaTime;
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
		characterController.Move (Quaternion.Euler (transform.eulerAngles) * velocity * Time.deltaTime);
		CharacterState characterState;
		if (isWalking) {
			if (isRunning) {
				characterState = CharacterState.Run;
			} else {
				characterState = CharacterState.Walk;
			}
		} else {
			characterState = CharacterState.Idle;
		}
		activeGun.GetAnimator ().SetState (characterState, gunState);
		walkingSound.SetSoundState (characterState);
	}

	public void GetBulletInfo(out short bulletNum, out short bulletCapacity, out short bulletOwn) {
		bulletNum = activeGun.GetBulletNum ();
		bulletCapacity = activeGun.bulletCapacity;
		bulletOwn = activeGun.GetBulletOwn ();
	}
}
