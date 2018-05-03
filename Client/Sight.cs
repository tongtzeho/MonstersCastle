using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sight : MonoBehaviour {

	private SkinnedMeshRenderer meshRenderer;
	private Camera characteCamera;
	public RectTransform defaultSight; // assigned in editor
	public RectTransform sniperSight; // assigned in editor
	private const float sniperSightDefaultHeight = 1080.0f;
	private const float defaultFOV = 80.0f;
	private const float sightFOV = 24.0f;
	private bool useSight = false;
	private Vector3 disablePosition = new Vector3(0, 10000, 0);

	void Start () {
		meshRenderer = transform.Find ("Springfield").gameObject.GetComponent<SkinnedMeshRenderer> ();
		characteCamera = transform.parent.gameObject.GetComponent<Camera> ();
		float scale = Screen.height / sniperSightDefaultHeight;
		sniperSight.localScale = new Vector3 (scale, scale, 1);
	}

	public bool GetSight() {
		return useSight;
	}

	public void SetSight(bool sight) {
		useSight = sight;
		if (!useSight) {
			characteCamera.fieldOfView = defaultFOV;
			defaultSight.localPosition = Vector3.zero;
			sniperSight.localPosition = disablePosition;
		} else {
			characteCamera.fieldOfView = sightFOV;
			meshRenderer.enabled = false;
			sniperSight.localPosition = Vector3.zero;
			defaultSight.localPosition = disablePosition;
		}
	}
}
