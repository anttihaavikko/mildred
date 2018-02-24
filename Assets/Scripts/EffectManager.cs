using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : MonoBehaviour {

	public GameObject[] effects;
	private List<GameObject> limitedObjects;
	private int limit = 20;

	// ==================

	private static EffectManager instance = null;

	public static EffectManager Instance {
		get { return instance; }
	}

	// ==================

	void Awake() {
		if (instance != null && instance != this) {
			Destroy (this.gameObject);
			return;
		} else {
			instance = this;
		}

		limitedObjects = new List<GameObject> ();
	}

	public GameObject AddEffect(int effect, Vector3 position) {
		GameObject e = Instantiate (effects[effect], transform);
		e.transform.position = position;
		return e;
	}

	public GameObject AddEffectToParent(int effect, Vector3 position, Transform parent) {
		GameObject e = Instantiate (effects[effect], parent);
		e.transform.position = position;
		return e;
	}

	public GameObject AddLimitedEffect(int effect, Vector3 position) {
		GameObject e = AddEffect (effect, position);
		limitedObjects.Add (e);

		if (limitedObjects.Count > limit) {
			GameObject first = limitedObjects [0];
			Destroy (first);
			limitedObjects.RemoveAt (0);
		}

		return e;
	}
}
