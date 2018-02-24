using UnityEngine;
using System.Collections;

public class FollowCamera : MonoBehaviour {

	public Transform target;
	public float dampTime = 0.15f;
	public Vector3 offset = Vector3.zero;

	private Vector3 velocity = Vector3.zero;

	private Rigidbody2D body;

	void Start() {
		body = target.GetComponent<Rigidbody2D> ();
	}

	// Update is called once per frame
	void Update () 
	{
		if (target) {
			Vector3 point = Camera.main.WorldToScreenPoint(target.position);
			Vector3 delta = target.position - Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, point.z));
			Vector3 destination = transform.position + delta + new Vector3(offset.x * target.localScale.x, offset.y, offset.z);
			float z = Mathf.MoveTowards (transform.position.z, -6f - body.velocity.magnitude * 0.75f, Time.deltaTime * 10f);
			destination.z = z;
			transform.position = Vector3.SmoothDamp(transform.position, destination, ref velocity, dampTime);

			Quaternion targetRot = Quaternion.Euler(new Vector3 (0, Mathf.Clamp(body.angularVelocity * -0.005f, -5f, 5f), 0f));
//			transform.rotation = targetRot;
			transform.rotation = Quaternion.RotateTowards (transform.rotation, targetRot, Time.deltaTime * 2f);
		}

	}
}