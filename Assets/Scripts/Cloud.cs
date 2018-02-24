using UnityEngine;
using System.Collections;

public class Cloud : MonoBehaviour {

	private float speed;

	private float min = -150f;
	private float max = 150f;

	// Use this for initialization
	void Awake () {

		SpriteRenderer sprite = GetComponent<SpriteRenderer> ();

		float r = Random.value * 0.005f;

		float depth = 2 + Random.Range(-1f, 7f);

		float xdir = (Random.value < 0.5f) ? 1f : -1f;
		float ydir = 1f;

		transform.localPosition = new Vector3 (transform.localPosition.x + Random.Range(-20, 20), transform.localPosition.y + Random.Range(-5, 5), depth);
		transform.localScale = new Vector3 (xdir * (1f + r), ydir * (1f + r), 1f);

		sprite.color = new Color (1, 1, 1, 0.1f + Random.value / 10f);

		speed = 0.1f + Random.value * 0.5f;
	}

	void Update() {
		transform.Translate(Vector3.right * Time.deltaTime * speed);

		if (transform.localPosition.x > max) {
			transform.localPosition = new Vector3 (min, transform.localPosition.y, transform.localPosition.z);
		}
	}
}
