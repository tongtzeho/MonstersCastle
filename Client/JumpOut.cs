using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpOut : MonoBehaviour {

	private CharacterController characterController;
	private float distSqrThreshold = 1.6f * 1.6f;
	private float jumpOutVelocity = 20.0f;
	private float jumpOutCD = 0.5f;
	private float jumpOutTime = 0;
	private Vector3 yOffset = new Vector3(0, 1, 0);
	private Vector3 forceDir;

	void Start () {
		characterController = GameObject.Find ("Character").GetComponent<CharacterController> ();
	}

	void Update () {
		if (jumpOutTime == 0) {
			Vector3 offset = characterController.transform.position - transform.position;
			forceDir = offset.normalized + yOffset;
			if (offset.sqrMagnitude <= distSqrThreshold) {
				characterController.Move (forceDir * jumpOutVelocity * Time.deltaTime);
				jumpOutTime = jumpOutCD;
			}
		} else {
			characterController.Move (forceDir * jumpOutVelocity * Time.deltaTime * jumpOutTime / jumpOutCD);
			jumpOutTime -= Time.deltaTime;
			if (jumpOutTime < 0) {
				jumpOutTime = 0;
			}
		}
	}
}
