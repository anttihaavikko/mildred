using UnityEditor;

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class MeshFromPolyEditor {

	[MenuItem("CONTEXT/MeshFilter/Update Mesh From Collider...")]
	public static void UpdateMeshFromPoly (MenuCommand menuCommand) {
		MeshFilter mf = menuCommand.context as MeshFilter;
		Mesh m = mf.sharedMesh;
		UpdateMesh (mf);
	}

	public static void UpdateMesh(MeshFilter mf) {
		Debug.Log (mf.name);

		Mesh mesh = new Mesh();
		PolygonCollider2D poly = mf.GetComponent<PolygonCollider2D> ();

		// Create the Vector3 vertices
		Vector3[] vertices = new Vector3[poly.points.Length];
		for (int i = 0; i < vertices.Length; i++) {
			vertices[i] = new Vector3(poly.points[i].x, poly.points[i].y, 0);
		}

		Triangulator tr = new Triangulator(poly.points);
		int[] indices = tr.Triangulate();

		mesh.vertices = vertices;
		mesh.triangles = indices;

		mf.mesh = mesh;
	}
}