﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfoText : MonoBehaviour {

	private Text textBox;
	private Animator anim;
	public Sprite[] helpImages;
	public Image helpImage;

	// Use this for initialization
	void Start () {
		textBox = GetComponent<Text> ();
		anim = GetComponent<Animator> ();
	}

	public void ShowText(string text1, string text2, float delay = 4f, int image = -1) {

		AudioManager.Instance.LowpassOn (delay);

		text1 = text1.Replace ("THE END", "<color=#FF8E00>THE END</color>");
		text1 = text1.Replace ("MILLIE", "<color=#FF8E00>MILLIE</color>");
		text2 = text2.Replace ("MILLIE", "<color=#FF8E00>MILLIE</color>");
		textBox.text = text1 + "\n<size=50>" + text2 + "</size>";

		if (image > -1) {
			textBox.text += "\n";
			helpImage.sprite = helpImages [image];
			helpImage.enabled = true;
		} else {
			helpImage.enabled = false;
		}

		CancelInvoke ("HideText");
		Invoke ("HideText", delay);
		anim.ResetTrigger ("hide");
		anim.ResetTrigger ("show");
		anim.SetTrigger ("show");
	}

	private void HideText() {
		anim.ResetTrigger ("hide");
		anim.ResetTrigger ("show");
		anim.SetTrigger ("hide");
	}
}
