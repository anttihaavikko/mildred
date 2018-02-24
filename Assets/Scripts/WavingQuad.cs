using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WavingQuad : MonoBehaviour {

	public float amount = 1f;
	public float speed = 1f;
	public float midPoint = 0.5f;
	public bool mirrored = false;

	private Mesh mesh;
	private Vector3[] originalVertices, displacedVertices;
	private float offset;

	// Use this for initialization
	void Awake () {

		offset = Random.Range (0f, 1f);

		float width = 0.5f;
		float height = 0.5f;

		// Create Vector2 vertices

		List<Vector2> vertices2D = new List<Vector2> ();

		vertices2D.Add (new Vector2 (-width, -height));
		vertices2D.Add (new Vector2 (-width, -height + height * midPoint * 2));
		vertices2D.Add (new Vector2 (-width, height));
		vertices2D.Add (new Vector2 (width, height));
		vertices2D.Add (new Vector2 (width, -height + height * midPoint * 2));
		vertices2D.Add (new Vector2 (width, -height));

		Vector2[] vertArray = vertices2D.ToArray();

		// Use the triangulator to get indices for creating triangles
		Triangulator tr = new Triangulator(vertArray);
		int[] indices = tr.Triangulate();

		// Create the Vector3 vertices
		Vector3[] vertices = new Vector3[vertArray.Length];
		for (int i = 0; i < vertices.Length; i++) {
			vertices[i] = new Vector3(vertArray[i].x, vertArray[i].y, 0);
		}

		float start = mirrored ? 1f : 0f;
		float end = mirrored ? 0f : 1f;

		Vector2[] uvs = new Vector2[vertices.Length];
		uvs[0] = new Vector2(start, 0f);
		uvs[1] = new Vector2(start, midPoint);
		uvs[2] = new Vector2(start, 1f);
		uvs[3] = new Vector2(end, 1f);
		uvs[4] = new Vector2(end, midPoint);
		uvs[5] = new Vector2(end, 0f);

		mesh = new Mesh();
		mesh.vertices = vertices;
		mesh.triangles = indices;
		mesh.uv = uvs;
		mesh.RecalculateNormals();
		mesh.RecalculateBounds();

		GetComponent<MeshFilter>().mesh = mesh;

		originalVertices = mesh.vertices;

		displacedVertices = new Vector3[originalVertices.Length];
		System.Array.Copy (originalVertices, displacedVertices, originalVertices.Length);
	}
	
	// Update is called once per frame
	void Update () {

		displacedVertices[1] = originalVertices[1] + Vector3.right * Mathf.PerlinNoise (Time.time * speed, offset) * 0.2f * amount;
		displacedVertices[4] = originalVertices[4] + Vector3.right * Mathf.PerlinNoise (Time.time * speed, offset) * 0.2f * amount;

		displacedVertices[2] = originalVertices[2] + Vector3.right * Mathf.PerlinNoise (Time.time * speed, offset) * 0.5f * amount;
		displacedVertices[3] = originalVertices[3] + Vector3.right * Mathf.PerlinNoise (Time.time * speed, offset) * 0.5f * amount;

		mesh.vertices = displacedVertices;
		mesh.RecalculateNormals();
	}
}
