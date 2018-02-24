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

	private Vector3 spawnPoint = Vector3.zero;

	private int points = 0;

	// Use this for initialization
	void Start () {
		body = GetComponent<Rigidbody2D> ();
		spawnPoint = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		transform.localScale = Vector3.MoveTowards(transform.localScale, targetSize, Time.deltaTime);

		grounded = false;

		for (int i = -1; i < 2; i++) {
			Vector3 spot = transform.position + i * Vector3.left * 0.5f;
			bool spotGrounded = Physics2D.Raycast (spot, Vector3.down, 1f, groundLayer);

			Color c = spotGrounded ? Color.green : Color.red;
			Debug.DrawRay (spot, Vector3.down, c);

			if (spotGrounded) {
				grounded = true;
				break;
			}
		}
			
	}

	void FixedUpdate() {
		float dir = InputMagic.Instance.GetAxis (InputMagic.STICK_OR_DPAD_X);
		body.AddTorque (-dir * rollForce);

		targetSize = (InputMagic.Instance.GetButton (InputMagic.A)) ? new Vector3 (1.1f, 0.9f, 1f) : new Vector3 (1f, 1f, 1f);

		if(Physics2D.OverlapCircle(transform.position, 0.2f, groundLayer)) {
			Die();
		}
	}

	void LateUpdate() {
		
		if (grounded && InputMagic.Instance.GetButtonDown (InputMagic.A)) {
			body.velocity = new Vector2 (body.velocity.x, 0);
			body.AddForce (Vector2.up * jumpForce, ForceMode2D.Impulse);
		}

		body.angularVelocity = Mathf.Clamp (body.angularVelocity, -maxRotationSpeed, maxRotationSpeed);

		if (Application.isEditor && Input.GetKeyDown (KeyCode.R)) {
			Die ();
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
			Die ();
		}

		if (coll.gameObject.tag == "Spikes") {
			Die ();
		}
	}

	void OnTriggerEnter2D(Collider2D coll) {
		
		if (coll.tag == "Checkpoint") {
			spawnPoint = coll.transform.position;
		}

		if (coll.tag == "Leaf") {
			Destroy (coll.gameObject);
			points++;
			Debug.Log (points + " points");
		}
	}

	void Die() {
		gameObject.SetActive (false);
		Invoke ("Respawn", 2f);
	}

	void Respawn() {
		body.velocity = Vector2.zero;
		body.angularVelocity = 0f;

		tailBody.velocity = Vector2.zero;
		tailBody.angularVelocity = 0f;

		transform.position = spawnPoint;
		tailBody.transform.localPosition = new Vector3 (0, -1.783f, 0f);

		gameObject.SetActive (true);
	}
}
