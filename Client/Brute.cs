using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brute : MonoBehaviour {
	public short isAlive = 0;
	public short level = 0;
	public MonsterHP monster;
	public short action = 0;
	public Animator animator; // assigned in editor
	private float dieAnimationTotalTime = 1.4f;
	private float dieAnimationCurrTime = 0;
	private float attackParticlePlayTime = 0.48f;
	private float attackCurrTime = 0;
	private ParticleSystem attackParticleSystem;
	private AudioSource attackSound;
	public AudioSource hurtSound; // assigned in editor
	public AudioSource dieSound; // assigned in editor
	private bool setAttackAction = false;
	private MedicinePool medicinePool;

	void Awake() {
		FadeImage skull = GameObject.Find ("Skull").GetComponent<FadeImage> ();
		monster = new MonsterHP (hurtSound, dieSound, skull);
	}

	void Start() {
		GameObject attackPoint = transform.Find ("AttackParticle").gameObject;
		attackParticleSystem = attackPoint.GetComponent<ParticleSystem> ();
		attackSound = attackPoint.GetComponent<AudioSource> ();
		medicinePool = GameObject.Find ("MedicinePool").GetComponent<MedicinePool> ();
	}

	public void Serialize(byte[] serializedData, ref int offset) {
		int begin = offset;
		Serializer.ToBytes ((short)0, serializedData, ref offset); // brute data length
		Serializer.ToBytes (isAlive, serializedData, ref offset);
		Serializer.ToBytes (level, serializedData, ref offset);
		Serializer.ToBytes (monster.hp, serializedData, ref offset);
		Serializer.ToBytes ((short)(offset - begin - 2), serializedData, ref begin);
	}

	public void UpdateFromServer (bool gameInitializing, byte[] recvData, int beginIndex, int length) {
		short isCurrAlive = BitConverter.ToInt16 (recvData, beginIndex);
		level = BitConverter.ToInt16 (recvData, beginIndex + 2);
		bool isReborn = (isCurrAlive == 1 && isAlive == 0 && !gameInitializing);
		isAlive = isCurrAlive;
		if (gameInitializing || isReborn) {
			monster.hp = BitConverter.ToInt16 (recvData, beginIndex + 4);
			monster.maxHp = BitConverter.ToInt16 (recvData, beginIndex + 6);
		}
		if (isAlive == 1) {
			transform.position = new Vector3 (BitConverter.ToSingle (recvData, beginIndex + 8), BitConverter.ToSingle (recvData, beginIndex + 12), BitConverter.ToSingle (recvData, beginIndex + 16));
			transform.rotation = Quaternion.Euler (0, BitConverter.ToSingle (recvData, beginIndex + 20), 0);
			dieAnimationCurrTime = 0;
		} else {
			attackCurrTime = 0;
			if (dieAnimationCurrTime == 0) {
				medicinePool.Occur (transform.position);
			}
			dieAnimationCurrTime += Time.deltaTime;
			if (dieAnimationCurrTime > dieAnimationTotalTime) {
				transform.position = new Vector3 (transform.position.x, -5, transform.position.z);
			}
		}
		action = BitConverter.ToInt16 (recvData, beginIndex + 24);
		SetAnimationAction (action);
	}

	void SetAnimationAction(short action) {
		if ((!setAttackAction) || action == 4) { // avoid in a frame, set attack at first, and set idle at second
			animator.SetInteger ("Action", (int)action);
			if (action == 3) {
				setAttackAction = true;
			}
		}
		if (action == 3 && attackCurrTime == 0) {
			attackCurrTime = 0.001f;
		}
		if (attackCurrTime != 0) {
			attackCurrTime += Time.deltaTime;
			if (attackCurrTime >= attackParticlePlayTime) {
				attackCurrTime = 0;
				attackParticleSystem.Play ();
				attackSound.Play ();
			}
		}
	}

	public void Reset() {
		transform.position = new Vector3 (transform.position.x, -5, transform.position.z);
		SetAnimationAction (2);
	}

	void Update() {
		setAttackAction = false;
	}
}
