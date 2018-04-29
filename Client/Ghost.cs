﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost : MonoBehaviour {

	public short serverId;
	public MonsterHP monster;
	public short action;
	private GhostAnimator ghostAnimator;
	private bool isDying = false;
	private BulletPool submachineBulletPool;
	private BulletPool sniperBulletPool;
	private Vector3 offsetY = new Vector3(0, 50, 0);
	private float bulletProbability = 0.125f;

	void Awake() {
		AudioSource hurtSound = GetComponent<AudioSource> ();
		FadeImage skull = GameObject.Find ("Skull").GetComponent<FadeImage> ();
		monster = new MonsterHP (hurtSound, hurtSound, skull);
	}

	void Start() {
		ghostAnimator = GetComponent<GhostAnimator> ();
		BulletPool[] bulletPools = GameObject.Find ("BulletPool").GetComponents<BulletPool> ();
		if (bulletPools [0].prefabName == "SniperBullet") {
			sniperBulletPool = bulletPools [0];
			submachineBulletPool = bulletPools [1];
		} else {
			sniperBulletPool = bulletPools [1];
			submachineBulletPool = bulletPools [0];
		}
	}

	public void Serialize(byte[] serializedData, ref int offset) {
		Serializer.ToBytes (serverId, serializedData, ref offset);
		Serializer.ToBytes (monster.hp, serializedData, ref offset);
	}

	public void Enable(short sid, short hp) {
		serverId = sid;
		monster.hp = hp;
		isDying = false;
		action = 2;
	}

	public void Disable() {
		monster.hp = 0;
		isDying = false;
		transform.position = new Vector3 (0, -50, -50);
		action = 2;
	}

	public void UpdateFromServer (byte[] recvData, int beginIndex, int length) {
		monster.maxHp = BitConverter.ToInt16 (recvData, beginIndex + 4);
		action = BitConverter.ToInt16 (recvData, beginIndex + 22);
		if (action == 5) {
			monster.hp = 0;
		}
		if (action == 4) {
			if (!isDying) { // die by hit at the moment
				float rm = UnityEngine.Random.value;
				if (rm < bulletProbability + bulletProbability) {
					if (rm < bulletProbability) {
						submachineBulletPool.Occur (transform.position.y < -40.0f ? transform.position + offsetY : transform.position);
					} else {
						sniperBulletPool.Occur (transform.position.y < -40.0f ? transform.position + offsetY : transform.position);
					}
				}
			}
			isDying = true;
		} else {
			isDying = false;
		}
		if (monster.hp > 0) {
			transform.position = new Vector3 (BitConverter.ToSingle (recvData, beginIndex + 6), BitConverter.ToSingle (recvData, beginIndex + 10), BitConverter.ToSingle (recvData, beginIndex + 14));
			transform.rotation = Quaternion.Euler (0, BitConverter.ToSingle (recvData, beginIndex + 18), 0);
		} else {
			transform.position = new Vector3 (BitConverter.ToSingle (recvData, beginIndex + 6), BitConverter.ToSingle (recvData, beginIndex + 10), BitConverter.ToSingle (recvData, beginIndex + 14)) - offsetY;
		}
		ghostAnimator.SetState (action);
	}
}
