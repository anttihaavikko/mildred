using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartView : MonoBehaviour {
	
	private bool interacted = false;
	private bool canStart = false;
	public Dimmer dimmer;
	public Animator anim;

	void Start() {
		SceneManager.LoadSceneAsync ("Options", LoadSceneMode.Additive);
		Cursor.visible = true;
		Invoke ("EnableStarting", 1.5f);
	}

	// Update is called once per frame
	void Update () {

		if (Input.GetKeyDown (KeyCode.Escape) && Application.platform != RuntimePlatform.WebGLPlayer && !Application.isEditor) {
			interacted = true;
			Debug.Log ("Quit...");
			Application.Quit ();
			return;
		}

		if (Input.GetMouseButton (0)) {
			return;
		}

		if (canStart && Input.anyKeyDown && !interacted && !Input.GetKey(KeyCode.Escape)) {
			interacted = true;
			anim.SetTrigger ("hide");
			dimmer.FadeIn (0.5f);
			Invoke ("StartGame", 2f);

			AudioManager.Instance.PlayEffectAt (8, Vector3.zero, 0.5f);
			AudioManager.Instance.PlayEffectAt (9, Vector3.zero, 0.5f);
		}
	}

	void EnableStarting() {
		canStart = true;
	}

	void StartGame() {

		SceneManager.LoadSceneAsync ("Main");

		if (!Application.isEditor) {
			Cursor.visible = false;
		}
	}

	void DoSound(int clip) {
		AudioManager.Instance.PlayEffectAt(clip, Vector3.zero, 0.5f);
	}
}
