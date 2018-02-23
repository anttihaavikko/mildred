using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Panda : MonoBehaviour {

	private Rigidbody2D body;

	private float rollForce = 5f;
	private float jumpForce = 10f;
	private float maxRotationSpeed = 500f;

	// Use this for initialization
	void Start () {
		body = GetComponent<Rigidbody2D> ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void FixedUpdate() {
		float dir = InputMagic.Instance.GetAxis (InputMagic.STICK_OR_DPAD_X);
		body.AddTorque (-dir * rollForce);

		if (InputMagic.Instance.GetButtonDown (InputMagic.A)) {
			body.velocity = new Vector2 (body.velocity.x, 0);
			body.AddForce (Vector2.up * jumpForce, ForceMode2D.Impulse);
		}

		body.angularVelocity = Mathf.Clamp (body.angularVelocity, -maxRotationSpeed, maxRotationSpeed);

		if (Application.isEditor && Input.GetKeyDown (KeyCode.R)) {
			body.MovePosition (Vector2.zero);
		}
	}
}
