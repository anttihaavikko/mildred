using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeepRotating : MonoBehaviour {

	public float speed = 1f;
	private float angle = 0f;
	
	// Update is called once per frame
	void Update () {
		angle += speed;
		transform.localRotation = Quaternion.Euler (new Vector3 (0, 0, angle));
	}
}
