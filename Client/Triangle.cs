using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// only use this script to generate scene's triangles for server while editing. remove this script before building
public class Triangle : MonoBehaviour {

	void Start () {
		List<Vector3> vertices = new List<Vector3> ();
		List<uint> indices = new List<uint> ();
		GameObject environment = GameObject.Find ("Environment");
		MeshCollider[] meshColliders = environment.GetComponentsInChildren<MeshCollider> ();
		for (int i = 0; i < meshColliders.Length; ++i) {
			uint numIndices = (uint)vertices.Count;
			for (int j = 0; j < meshColliders [i].sharedMesh.vertices.Length; ++j) {
				Vector3 v = meshColliders [i].transform.TransformPoint (meshColliders [i].sharedMesh.vertices [j]);
				vertices.Add (v);
			}
			for (int j = 0; j < meshColliders [i].sharedMesh.subMeshCount; ++j) {
				for (int k = 0; k < meshColliders [i].sharedMesh.GetIndices (j).Length; ++k) {
					indices.Add (numIndices + (uint)meshColliders [i].sharedMesh.GetIndices (j) [k]);
				}
			}
		}
		BinaryWriter bw = new BinaryWriter(new FileStream("triangle.bin", FileMode.Create));
		bw.Write (vertices.Count);
		for (int i = 0; i < vertices.Count; ++i) {
			bw.Write (vertices [i].x);
			bw.Write (vertices [i].y);
			bw.Write (vertices [i].z);
		}
		bw.Write (indices.Count);
		for (int i = 0; i < indices.Count; ++i) {
			bw.Write (indices [i]);
		}
		bw.Close ();
		Debug.Log ("Complete Generating triangle.bin");
	}
}