﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : MonoBehaviour {

	public bool isVisible;

	void Start ()
	{
		if (LevelManage.currentPlayerId == 1) {
			GetComponent<AudioSource>().enabled = false;
		}

		SetVisible(false);
	}

	public void SetVisible(bool flag)
	{
		MeshRenderer[] meshRenderers = GetComponentsInChildren<MeshRenderer>();

		foreach (MeshRenderer mesh in meshRenderers) {
			mesh.enabled = flag;
		}

		Light l = GetComponent<Light>();
		l.enabled = flag;

		isVisible = flag;
	}
}
