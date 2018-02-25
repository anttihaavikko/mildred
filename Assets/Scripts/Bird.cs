using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bird : MonoBehaviour {

	private Animator anim;

	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator> ();
	}
	
	public void Fly() {
		anim.SetTrigger ("fly");
		Vector3 targetPos = transform.position + Vector3.up * 10f + Vector3.left * Random.Range(0, 5f) * Mathf.Sign(transform.localScale.x);
		float duration = 3.5f;
		Tweener.Instance.MoveTo (transform, targetPos, duration, 0f, TweenEasings.BackEaseIn, 0);
		Invoke ("DoRemove", duration + 1f);
	}

	void DoRemove() {
		Destroy (gameObject);
	}

	public void Flap() {
		AudioManager.Instance.PlayEffectAt (14, transform.position, 0.25f);
	}
}
