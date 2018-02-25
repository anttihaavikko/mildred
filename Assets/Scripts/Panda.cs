using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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

	private EffectCamera cam;

	public Face face;

	public InfoText info;

	private bool hasMoved = false;
	private Vector3 startPos;

	private bool canJump = false;
	private bool canDoubleJump = false;
	private bool hasDoubleJumped = false;
	private bool thickSkin = false;
	private bool highJump = false;
	private bool hasEnded = false;
	private bool canMove = false;

	public Dimmer dimmer;

	public GameObject mate;

	public ParticleSystem sleepParticles;

	private bool sleeping = true;
	private float rollCooldown = 0f;

	// Use this for initialization
	void Start () {
		body = GetComponent<Rigidbody2D> ();
		spawnPoint = transform.position;
		cam = Camera.main.GetComponent<EffectCamera> ();

		startPos = transform.position;

		Invoke ("ShowWake", 2f);

		if (!Application.isEditor) {
			Cursor.visible = false;
		}
	}

	void ShowWake() {
		Invoke ("ShowWakeAgain", 7f);
		info.ShowText ("MILLIE", "WOKE UP");
		MessageSound ();
		canMove = true;
	}

	void ShowWakeAgain() {
		if (!hasMoved) {
			info.ShowText ("NO REALLY,", "MILLIE WOKE UP!", 6f, 0);
			MessageSound ();
		}
	}
	
	// Update is called once per frame
	void Update () {

		if (!hasMoved && (transform.position - startPos).magnitude > 2f) {
			hasMoved = true;
		}

		if (Input.GetKeyUp (KeyCode.Escape)) {
			BackToStart (true);
		}
			
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

		RollCheck ();
			
	}

	void FixedUpdate() {
		float dir = InputMagic.Instance.GetAxis (InputMagic.STICK_OR_DPAD_X);

		if (canMove) {
			body.AddTorque (-dir * rollForce);
		}

		if (sleeping) {
			float pos = Mathf.Abs(Mathf.Sin (Time.time)) * 0.05f;
			targetSize = new Vector3 (1f + pos, 1f - pos, 1f);
		} else {
			targetSize = (InputMagic.Instance.GetButton (InputMagic.A)) ? new Vector3 (1.1f, 0.9f, 1f) : new Vector3 (1f, 1f, 1f);
		}

		if(Physics2D.OverlapCircle(transform.position, 0.2f, groundLayer)) {
			Die();
		}
	}

	void LateUpdate() {
		
		if (canJump && (grounded || (canDoubleJump && !hasDoubleJumped)) && InputMagic.Instance.GetButtonDown (InputMagic.A)) {
			body.velocity = new Vector2 (body.velocity.x, 0);
			float jump = highJump ? jumpForce * 1.2f : jumpForce;
			body.AddForce (Vector2.up * jump, ForceMode2D.Impulse);
			hasDoubleJumped = !grounded;

			if (grounded) {
				EffectManager.Instance.AddEffect (6, transform.position + Vector3.down * 0.75f);
			}

			EffectManager.Instance.AddEffectToParent(4, transform.position, transform);

			AudioManager.Instance.PlayEffectAt (12, transform.position, 0.25f);
			AudioManager.Instance.PlayEffectAt (10, transform.position, 0.25f);
		}

		body.angularVelocity = Mathf.Clamp (body.angularVelocity, -maxRotationSpeed, maxRotationSpeed);

		if (Application.isEditor && Input.GetKeyDown (KeyCode.R)) {
			Die ();
		}

		if (Application.isEditor && Input.GetKeyDown (KeyCode.KeypadPlus)) {
			points++;
			SkillInfo ();
		}

		int rollDir = 0;
		float rollLimit = 50f;

		if (body.angularVelocity > rollLimit) {
			rollDir = 1;
			sleeping = false;
			face.WakeUp ();
			sleepParticles.Stop ();
		}

		if (body.angularVelocity < -rollLimit) {
			rollDir = -1;
			sleeping = false;
			face.WakeUp ();
			sleepParticles.Stop ();
		}

		if (rollDir != 0 && rollDir != currentDirection) {
			face.Emote (Face.Emotion.Sneaky, Face.Emotion.Default, 2f);
			WallHolder.Instance.UpdateWalls (rollDir);
			currentDirection = rollDir;
			AudioManager.Instance.PlayEffectAt (11, transform.position, 0.3f);
		}
	}

	void OnCollisionEnter2D(Collision2D coll) {
		if (coll.gameObject.tag == "Spikes") {
			Die ();
			return;
		}

		if (coll.gameObject.tag == "Mate") {

			EffectManager.Instance.AddEffect (5, coll.contacts [0].point);

			if (!hasEnded) {
				hasEnded = true;
				face.Emote (Face.Emotion.Brag);
				info.ShowText ("MILLIE FEELS GRATEFUL", "...ALSO HORNY");
				MessageSound ();
				Invoke ("TheEnd", 5f);
			}
		}

		float dieLimit = thickSkin ? 30f : 15f;

		if (coll.relativeVelocity.magnitude > dieLimit) {
			Die ();
		} else {

			AudioManager.Instance.PlayEffectAt (0, transform.position, Mathf.Clamp(coll.relativeVelocity.magnitude * 0.05f, 0f, 2f));

			if (coll.relativeVelocity.magnitude > 5f) {

				EffectManager.Instance.AddEffect (6, coll.contacts [0].point);

				hasDoubleJumped = false;
				cam.BaseEffect (coll.relativeVelocity.magnitude * 0.1f);
			}
		}
	}

	void MessageSound() {
		AudioManager.Instance.PlayEffectAt (1, transform.position, 0.5f);
	}

	void TheEnd() {
		info.ShowText ("-- THE END --", "THANKS FOR PLAYING!", 30f);
		MessageSound ();
		dimmer.FadeIn (5f);
		Invoke ("BackToStart", 7f);
	}

	void BackToStart() {
		BackToStart (false);
	}

	void BackToStart(bool doSounds) {
		SceneManager.LoadSceneAsync ("Start");

		if (doSounds) {
			AudioManager.Instance.PlayEffectAt (8, transform.position, 0.5f);
			AudioManager.Instance.PlayEffectAt (9, transform.position, 0.5f);
		}
	}

	void OnTriggerEnter2D(Collider2D coll) {
		
		if (coll.tag == "Checkpoint") {
			spawnPoint = coll.transform.position;
		}

		if (coll.tag == "Leaf") {

			AudioManager.Instance.PlayEffectAt (5, transform.position, 0.75f);
			AudioManager.Instance.PlayEffectAt (4, transform.position, 0.75f);
			AudioManager.Instance.PlayEffectAt (9, transform.position, 0.75f);

			if (Random.value < 0.5f) {
				face.Emote (Face.Emotion.Happy, Face.Emotion.Default, 2f);
			} else {
				face.Emote (Face.Emotion.Brag);
			}

			cam.BaseEffect (1.5f);
			EffectManager.Instance.AddEffect (1, coll.transform.position);
			EffectManager.Instance.AddEffect (2, coll.transform.position);
			EffectManager.Instance.AddEffect (7, coll.transform.position);

			Destroy (coll.gameObject);
			points++;
			Debug.Log (points + " points");

			SkillInfo ();

		}
	}

	void SkillInfo() {
		
		if (points == 1) {
			info.ShowText ("MILLIE LEARNED", "TO JUMP", 6f, 1);
			MessageSound ();
			canJump = true;
		}

		if (points == 4) {
			info.ShowText ("MILLIE LEARNED", "HIGHER JUMP");
			MessageSound ();
			highJump = true;
		}

		if (points == 6) {
			info.ShowText ("MILLIE GREW", "THICKER SKIN");
			MessageSound ();
			thickSkin = true;
		}

		if (points == 11) {
			info.ShowText ("MILLIE LEARNED", "DOUBLE JUMP");
			MessageSound ();
			canDoubleJump = true;
		}

		if (points == 15) {
			info.ShowText ("MILLIE WANTS", "TO MATE!");
			MessageSound ();
			canDoubleJump = true;
			mate.SetActive (true);
		}
			
	}

	void Die() {

		AudioManager.Instance.PlayEffectAt (6, transform.position, 1f);
		AudioManager.Instance.PlayEffectAt (7, transform.position, 1f);
		AudioManager.Instance.PlayEffectAt (0, transform.position, 2f);
		AudioManager.Instance.PlayEffectAt (8, transform.position, 0.75f);

		AudioManager.Instance.HighpassOn (4f);

		sleepParticles.gameObject.SetActive (false);

		EffectManager.Instance.AddEffect (0, transform.position);
		EffectManager.Instance.AddEffect (1, transform.position);
		EffectManager.Instance.AddLimitedEffect (3, transform.position);
		gameObject.SetActive (false);
		Invoke ("Respawn", 2f);

		cam.BaseEffect (3f);
	}

	void Respawn() {

		if (!Application.isEditor) {
			Cursor.visible = false;
		}

		AudioManager.Instance.PlayEffectAt (8, spawnPoint, 0.75f);
		AudioManager.Instance.PlayEffectAt (10, spawnPoint, 2f);
		AudioManager.Instance.PlayEffectAt (3, spawnPoint, 0.5f);

		EffectManager.Instance.AddEffect (7, spawnPoint);
		EffectManager.Instance.AddEffect (1, spawnPoint);

		body.velocity = Vector2.zero;
		body.angularVelocity = 0f;

		tailBody.velocity = Vector2.zero;
		tailBody.angularVelocity = 0f;

		transform.position = spawnPoint;
		tailBody.transform.localPosition = new Vector3 (0, -1.783f, 0f);

		gameObject.SetActive (true);

		face.Emote (Face.Emotion.Angry, Face.Emotion.Default, 5f);

		transform.localScale = Vector3.one * 0.5f;
	}

	void RollCheck() {
		
		if (Mathf.Abs (body.angularVelocity) > 20f && rollCooldown <= 0f) {
			float fixedAngle = transform.rotation.eulerAngles.z % 360;

			if (Mathf.Abs (fixedAngle - 180f) < 10f) {
				rollCooldown = 0.25f;
				AudioManager.Instance.PlayEffectAt (13, transform.position, 0.1f);
				AudioManager.Instance.PlayEffectAt (1, transform.position, 0.2f);
			}
		}

		rollCooldown -= Time.deltaTime;
	}
}
