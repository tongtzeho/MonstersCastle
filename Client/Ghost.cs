using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost : IPoolObject {

	public short serverId;
	public MonsterHP monster;
	public short action;
	private GhostAnimator ghostAnimator;
	private bool isDying = false;
	private ObjectPool submachineBulletPool;
	private ObjectPool sniperBulletPool;
	private float bulletProbability = 0.2f;
	private Vector3 offsetY = new Vector3(0, 50, 0);

	void Awake() {
		AudioSource hurtSound = GetComponent<AudioSource> ();
		FadeImage skull = GameObject.Find ("Skull").GetComponent<FadeImage> ();
		monster = new MonsterHP (hurtSound, hurtSound, skull);
	}

	void Start() {
		ghostAnimator = GetComponent<GhostAnimator> ();
		ObjectPool[] bulletPools = GameObject.Find ("BulletPool").GetComponents<ObjectPool> ();
		sniperBulletPool = bulletPools [0];
		submachineBulletPool = bulletPools [1];
	}

	public override void Enable(byte[] recvData, int beginIndex) {
		base.Enable (recvData, beginIndex);
		serverId = BitConverter.ToInt16 (recvData, beginIndex);
		monster.hp = BitConverter.ToInt16 (recvData, beginIndex + 2);
		isDying = false;
		action = 2;
	}

	public override void Disable() {
		base.Disable ();
		monster.hp = 0;
		isDying = false;
		transform.position = new Vector3 (0, -50, -50);
		action = 2;
	}

	public override void Serialize(byte[] serializedData, ref int offset) {
		Serializer.ToBytes (monster.hp, serializedData, ref offset);
	}

	public override void Synchronize (byte[] recvData, int beginIndex) {
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
						submachineBulletPool.Create (serverId, recvData, beginIndex + 4);
					} else {
						sniperBulletPool.Create (serverId, recvData, beginIndex + 4);
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

	public override bool Step() {
		return true;
	}
}
