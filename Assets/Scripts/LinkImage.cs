using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinkImage : MonoBehaviour {

	public string link = "https://alakajam.com/";

	public void OnMouseEnter() {
		transform.localScale = Vector3.one;
		Tweener.Instance.ScaleTo (transform, new Vector3 (0.8f, 1.2f, 1f), 0.5f, 0f, TweenEasings.SineEaseInOut, 0);
	}

	public void OnMouseExit() {
//		Tweener.Instance.ScaleTo (transform, new Vector3 (1.1f, 0.9f, 1f), 0.5f, 0f, TweenEasings.SineEaseInOut, 0);
	}

	public void OnClick() {
		Application.OpenURL(link);
	}
}
