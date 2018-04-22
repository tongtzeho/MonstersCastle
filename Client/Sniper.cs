using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sniper : MonoBehaviour {

	private SkinnedMeshRenderer meshRenderer;
	private Camera camera;
	public RectTransform defaultSight; // assigned in editor
	public RectTransform sniperSight; // assigned in editor
	private const float sniperSightDefaultHeight = 1080.0f;
	private const float defaultFOV = 80.0f;
	private const float adsFOV = 24.0f;
	private bool ads = false;
	private Vector3 disablePosition = new Vector3(0, 10000, 0);

	void Start () {
		meshRenderer = transform.Find ("Springfield").gameObject.GetComponent<SkinnedMeshRenderer> ();
		camera = transform.parent.gameObject.GetComponent<Camera> ();
		float scale = Screen.height / sniperSightDefaultHeight;
		sniperSight.localScale = new Vector3 (scale, scale, 1);
	}

	public bool GetADS() {
		return ads;
	}

	public void SetADS(bool a) {
		ads = a;
		if (!ads) {
			camera.fieldOfView = defaultFOV;
			defaultSight.localPosition = Vector3.zero;
			sniperSight.localPosition = disablePosition;
		} else {
			camera.fieldOfView = adsFOV;
			meshRenderer.enabled = false;
			sniperSight.localPosition = Vector3.zero;
			defaultSight.localPosition = disablePosition;
		}
	}
}
