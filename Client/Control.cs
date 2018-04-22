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
	private float velocityY = 0.0f;
	private const float gravity = -9.8f;
	private const float maxVelocityY = 30.0f;
	private const float walkVelocity = 2.5f;
	private const float runVelocity = 5.5f;
	private const float jumpVelocity = 4.5f;
	private Gun activeGun;
	private Gun inactiveGun;
	private Sniper sniperSight;
	private CharacterSound characterSound;
	private bool allow = false; // can control

	void Start () {
		characterController = GetComponent<CharacterController> ();
		cameraTransform = transform.Find ("Camera");
		GameObject sniperObject = transform.Find ("Camera/sniper").gameObject;
		sniper = sniperObject.GetComponent<Gun> ();
		sniperSight = sniperObject.GetComponent<Sniper> ();
		GameObject submachineObject = transform.Find ("Camera/submachinegun").gameObject;
		submachineGun = submachineObject.GetComponent<Gun> ();
		activeGun = sniper;
		inactiveGun = submachineGun;
		characterSound = GetComponent<CharacterSound> ();
	}

	public void Disallow() {
		allow = false;
	}

	public void Allow() {
		allow = true;
	}

	public void SetDeadCamera() {
		cameraTransform.localRotation = Quaternion.Euler (40, 0, 0);
		sniperSight.SetADS (false);
	}

	public void Reset() {
		if (activeGun != sniper) {
			SwitchGun ();
		}
		cameraTransform.localRotation = Quaternion.Euler (0, 0, 0);
		sniperSight.SetADS (false);
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
		if (allow) {
			if (Input.GetButtonDown ("SwitchGun")) {
				SwitchGun ();
			}
			bool pressFire = Input.GetKey (KeyCode.Mouse0);
			bool pressReload = Input.GetKey (KeyCode.R);
			Gun.GunState gunState = activeGun.Action (pressFire, pressReload);
			float rotationX = Input.GetAxis ("Mouse X");
			float rotationY = Input.GetAxis ("Mouse Y");
			if (sniperSight.GetADS()) {
				rotationX *= 0.5f;
				rotationY *= 0.5f;
			}
			transform.Rotate (0.0f, rotationX, 0.0f);
			cameraTransform.Rotate (-rotationY, 0.0f, 0.0f);
			if (cameraTransform.localEulerAngles.x <= 180.0f && cameraTransform.localEulerAngles.x > 85.0f) {
				cameraTransform.localRotation = Quaternion.Euler (85.0f, 0, 0);
			} else if (cameraTransform.localEulerAngles.x > 180.0f && cameraTransform.localEulerAngles.x < 275.0f) {
				cameraTransform.localRotation = Quaternion.Euler (275.0f, 0, 0);
			}
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
			characterSound.SetWalkingSoundState (characterState);
			sniperSight.SetADS (activeGun == sniper && characterState != CharacterState.Run && (gunState == Gun.GunState.Fire || gunState == Gun.GunState.Idle) && Input.GetKey (KeyCode.Mouse1));
			Cursor.visible = false;
		} else {
			inactiveGun.GetAnimator ().SetState (CharacterState.Idle, Gun.GunState.Hide);
			activeGun.GetAnimator ().SetState (CharacterState.Idle, Gun.GunState.Hide);
			characterSound.SetWalkingSoundState (CharacterState.Idle);
			sniperSight.SetADS (false);
			Cursor.visible = true;
		}
	}

	public void GetBulletInfo(out short bulletNum, out short bulletCapacity, out short bulletOwn) {
		bulletNum = activeGun.GetBulletNum ();
		bulletCapacity = activeGun.bulletCapacity;
		bulletOwn = activeGun.GetBulletOwn ();
	}
}
