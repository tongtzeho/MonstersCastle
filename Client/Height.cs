using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// only use this script to generate height for server while editing
public class Height : MonoBehaviour {

	private float height = 4.5f;
	private float defaultHeight = -5.0f;
	private float minX = -28.0f;
	private float maxX = 28.0f;
	private float minZ = 0.0f;
	private float maxZ = 70.0f;
	private float d = 0.1f;

	[ContextMenu("Generate height.bin")]
	public void Generate () {
		int numX = (int)((maxX - minX) / d);
		int numZ = (int)((maxZ - minZ) / d);
		BinaryWriter bw = new BinaryWriter(new FileStream("height.bin", FileMode.Create));
		bw.Write (minX);
		bw.Write (maxX);
		bw.Write (numX);
		bw.Write (minZ);
		bw.Write (maxZ);
		bw.Write (numZ);
		float x = minX, z;
		for (int i = 0; i < numX; ++i) {
			z = minZ;
			for (int j = 0; j < numZ; ++j) {
				Ray ray = new Ray (new Vector3 (x, height, z), Vector3.down);
				RaycastHit hit;
				if (Physics.Raycast (ray, out hit)) {
					bw.Write (hit.point.y);
				} else {
					bw.Write (defaultHeight);
				}
				z += d;
			}
			x += d;
		}
		bw.Close ();
		Debug.Log ("Complete Generating height.bin");
	}
}
