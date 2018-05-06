using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Control : MonoBehaviour {

	public enum CharacterState {
		Idle = 0, Walk = 1, Run = 2
	}

	private CharacterController characterController;
	private Character character;
	private Transform cameraTransform;
	private Gun sniper;
	private Gun submachineGun;
	private float velocityY = 0.0f;
	private const float gravity = -9.8f;
	private const float maxVelocityY = 30.0f;
	private const float walkScalarVelocity = 2.5f;
	private const float runScalarVelocityMultiple = 2.2f;
	private const float unforwardMultiple = 0.75f;
	private const float jumpVelocity = 4.5f;
	private Gun activeGun;
	private Gun inactiveGun;
	private Sight sniperSight;
	private Recoil recoil = new Recoil ();
	private CharacterSound characterSound;
	private bool allow = false; // can control

	// related to pause
	private Game game;
	private GameUIPanel gameUIPanel;
	private bool isPause = false;

	// 9 directions
	// 0 1 2
	// 3 4 5
	// 6 7 8
	private Vector3[] walkVelocity;

	void Awake() {
		walkVelocity = new Vector3[9];
		walkVelocity [0] = new Vector3 (-1, 0, 1) * walkScalarVelocity * (1 + unforwardMultiple) * 0.5f * 0.707106781187f;
		walkVelocity [1] = new Vector3 (0, 0, 1) * walkScalarVelocity;
		walkVelocity [2] = new Vector3 (1, 0, 1) * walkScalarVelocity * (1 + unforwardMultiple) * 0.5f * 0.707106781187f;
		walkVelocity [3] = new Vector3 (-1, 0, 0) * walkScalarVelocity * unforwardMultiple;
		walkVelocity [4] = Vector3.zero;
		walkVelocity [5] = new Vector3 (1, 0, 0) * walkScalarVelocity * unforwardMultiple;
		walkVelocity [6] = new Vector3 (-1, 0, -1) * walkScalarVelocity * unforwardMultiple * 0.707106781187f;
		walkVelocity [7] = new Vector3 (0, 0, -1) * walkScalarVelocity * unforwardMultiple;
		walkVelocity [8] = new Vector3 (1, 0, -1) * walkScalarVelocity * unforwardMultiple * 0.707106781187f;
	}

	void Start () {
		characterController = GetComponent<CharacterController> ();
		character = GetComponent<Character> ();
		cameraTransform = transform.Find ("Camera");
		GameObject sniperObject = transform.Find ("Camera/sniper").gameObject;
		sniper = sniperObject.GetComponent<Gun> ();
		sniperSight = sniperObject.GetComponent<Sight> ();
		GameObject submachineObject = transform.Find ("Camera/submachinegun").gameObject;
		submachineGun = submachineObject.GetComponent<Gun> ();
		activeGun = sniper;
		inactiveGun = submachineGun;
		characterSound = GetComponent<CharacterSound> ();
		game = GameObject.Find ("Game").GetComponent<Game> ();
		gameUIPanel = GameObject.Find ("GameUI").GetComponent<GameUIPanel> ();
	}

	public void Disallow() {
		allow = false;
	}

	public void Allow() {
		allow = true;
	}

	public void SetDeadCamera() {
		cameraTransform.localRotation = Quaternion.Euler (40, 0, 0);
		sniperSight.SetSight (false);
	}

	public void Reset() {
		if (activeGun != sniper) {
			SwitchGun ();
		}
		cameraTransform.localRotation = Quaternion.Euler (0, 0, 0);
		sniperSight.SetSight (false);
		recoil.Reset ();
		isPause = false;
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
			bool clickSwitchGun = !isPause && Input.GetButtonDown ("SwitchGun");
			bool clickTakeMedicine = !isPause && Input.GetButtonDown ("TakeMedicine");
			bool pressFire = !isPause && Input.GetKey (KeyCode.Mouse0);
			bool pressReload = !isPause && Input.GetKey (KeyCode.R);
			float rotationX = isPause ? 0 : Input.GetAxis ("Mouse X");
			float rotationY = isPause ? 0 : Input.GetAxis ("Mouse Y");
			bool pressJump = !isPause && Input.GetKey (KeyCode.Space);
			bool pressRun = !isPause && Input.GetKey (KeyCode.LeftShift);
			bool pressW = !isPause && Input.GetKey (KeyCode.W);
			bool pressS = !isPause && Input.GetKey (KeyCode.S);
			bool pressA = !isPause && Input.GetKey (KeyCode.A);
			bool pressD = !isPause && Input.GetKey (KeyCode.D);
			bool pressSight = !isPause && Input.GetKey (KeyCode.Mouse1);

			if (clickSwitchGun) {
				SwitchGun ();
			}
			if (clickTakeMedicine) {
				character.TakeMedicine ();
			}
			float recoilResult = 0.0f;
			Gun.GunState gunState = activeGun.Action (pressFire, pressReload, out recoilResult);
			recoil.AddRecoil (recoilResult);
			float recover = recoil.Recover (Time.deltaTime);
			if (sniperSight.GetSight()) {
				rotationX *= 0.45f;
				rotationY *= 0.45f;
			}
			transform.Rotate (0.0f, rotationX, 0.0f);
			cameraTransform.Rotate (-rotationY - recoilResult + recover, 0.0f, 0.0f);
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
				if (pressJump) {
					velocityY = jumpVelocity;
				} else {
					velocityY = 0.0f;
				}
			}
			if (velocityY < -maxVelocityY) {
				velocityY = -maxVelocityY;
			}
			int direction = 4;
			if (pressW && !pressS) {
				if (pressA && !pressD) {
					direction = 0;
				} else if (pressD && !pressA) {
					direction = 2;
				} else {
					direction = 1;
				}
			} else if (pressS && !pressW) {
				if (pressA && !pressD) {
					direction = 6;
				} else if (pressD && !pressA) {
					direction = 8;
				} else {
					direction = 7;
				}
			} else {
				if (pressA && !pressD) {
					direction = 3;
				} else if (pressD && !pressA) {
					direction = 5;
				}
			}
			bool isWalking = direction != 4;
			velocity.x = pressRun ? runScalarVelocityMultiple * walkVelocity [direction].x : walkVelocity [direction].x;
			velocity.z = pressRun ? runScalarVelocityMultiple * walkVelocity [direction].z : walkVelocity [direction].z;
			velocity.y = velocityY;
			characterController.Move (Quaternion.Euler (transform.eulerAngles) * velocity * Time.deltaTime);
			CharacterState characterState;
			if (isWalking) {
				if (pressRun) {
					characterState = CharacterState.Run;
				} else {
					characterState = CharacterState.Walk;
				}
			} else {
				characterState = CharacterState.Idle;
			}
			activeGun.GetAnimator ().SetState (characterState, gunState);
			characterSound.SetWalkingSoundState (characterState);
			sniperSight.SetSight (activeGun == sniper && characterState != CharacterState.Run && (gunState == Gun.GunState.Fire || gunState == Gun.GunState.Idle) && pressSight);
			Cursor.visible = false;
		} else {
			inactiveGun.GetAnimator ().SetState (CharacterState.Idle, Gun.GunState.Hide);
			activeGun.GetAnimator ().SetState (CharacterState.Idle, Gun.GunState.Hide);
			characterSound.SetWalkingSoundState (CharacterState.Idle);
			sniperSight.SetSight (false);
			recoil.Reset ();
			Cursor.visible = true;
		}
		if (Input.GetButtonDown ("Pause")) {
			HandlePauseClick ();
		}
		if (isPause) {
			Cursor.visible = true;
		}
		gameUIPanel.ShowTask (game.IsStart () && Input.GetKey (KeyCode.Tab));
	}

	public void GetBulletInfo(out short bulletNum, out short bulletCapacity, out short bulletOwn) {
		bulletNum = activeGun.GetBulletNum ();
		bulletCapacity = activeGun.bulletCapacity;
		bulletOwn = activeGun.GetBulletOwn ();
	}

	public void HandlePauseClick() {
		if (game.IsStart ()) {
			isPause = !isPause;
			if (isPause) {
				gameUIPanel.Pause ();
			} else {
				gameUIPanel.Continue ();
			}
		}
	}
}
