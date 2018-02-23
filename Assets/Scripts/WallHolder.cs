using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallHolder : MonoBehaviour {

	public GameObject lefts, rights;

	// ==================

	private static WallHolder instance = null;

	public static WallHolder Instance {
		get { return instance; }
	}

	// ==================

	void Awake() {
		if (instance != null && instance != this) {
			Destroy (this.gameObject);
			return;
		} else {
			instance = this;
			UpdateWalls (1);
		}
	}

	public void UpdateWalls(int dir) {
		lefts.SetActive (dir == -1);
		rights.SetActive (dir == 1);
	}
}
