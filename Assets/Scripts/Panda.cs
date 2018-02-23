using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Panda : MonoBehaviour {

	private Rigidbody2D body;
	public Rigidbody2D tailBody;

	private float rollForce = 5f;
	private float jumpForce = 7.5f;
	private float maxRotationSpeed = 500f;

	private Vector3 targetSize = Vector3.one;

	private int currentDirection = 1;

	private bool grounded = false;
	public LayerMask groundLayer;

	// Use this for initialization
	void Start () {
		body = GetComponent<Rigidbody2D> ();
	}
	
	// Update is called once per frame
	void Update () {
		transform.localScale = Vector3.MoveTowards(transform.localScale, targetSize, Time.deltaTime);

		grounded = Physics2D.Raycast (transform.position, Vector2.down, 1f, groundLayer);
		Color c = grounded ? Color.green : Color.red;
		Debug.DrawRay (transform.position, Vector2.down, c);
	}

	void FixedUpdate() {
		float dir = InputMagic.Instance.GetAxis (InputMagic.STICK_OR_DPAD_X);
		body.AddTorque (-dir * rollForce);
		targetSize = (InputMagic.Instance.GetButton (InputMagic.A)) ? new Vector3 (1.1f, 0.9f, 1f) : new Vector3 (1f, 1f, 1f);

		if(Physics2D.OverlapCircle(transform.position, 0.2f, groundLayer)) {
			Reset();
		}
	}

	void LateUpdate() {
		if (grounded && InputMagic.Instance.GetButtonUp (InputMagic.A)) {
			body.velocity = new Vector2 (body.velocity.x, 0);
			body.AddForce (Vector2.up * jumpForce, ForceMode2D.Impulse);
		}

		body.angularVelocity = Mathf.Clamp (body.angularVelocity, -maxRotationSpeed, maxRotationSpeed);

		if (Application.isEditor && Input.GetKeyDown (KeyCode.R)) {
			Reset ();
		}

		int rollDir = 0;
		float rollLimit = 50f;

		if (body.angularVelocity > rollLimit) {
			rollDir = 1;
		}

		if (body.angularVelocity < -rollLimit) {
			rollDir = -1;
		}

		if (rollDir != 0 && rollDir != currentDirection) {
			WallHolder.Instance.UpdateWalls (rollDir);
			currentDirection = rollDir;
		}
	}

	void OnCollisionEnter2D(Collision2D coll) {
		if (coll.relativeVelocity.magnitude > 50f) {
			Reset ();
		}
	}

	void Reset() {
		body.velocity = Vector2.zero;
		body.angularVelocity = 0f;

		tailBody.velocity = Vector2.zero;
		tailBody.angularVelocity = 0f;

		transform.position = Vector3.zero;
		tailBody.transform.localPosition = new Vector3 (0, -1.783f, 0f);
	}
}
