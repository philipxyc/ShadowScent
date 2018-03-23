﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

public class CharacterManage : MonoBehaviour
{

    public GameObject characterPrefab;
	private GameObject player;
	private CompassController compassControl;
	private TouchController touchControl;

    void Start()
    {

        player = Instantiate<GameObject>(characterPrefab, gameObject.transform);
        player.tag = "Player";

		compassControl = new CompassController();
		touchControl = new TouchController();
    }

    void OnGUI()
    {
		compassControl.Debug();
    }

    void Update()
    {
		ControlPlayer();
    }

	private void ControlPlayer()
	{
		int moveDir = 0;
		Character c = player.GetComponent<Character>();

		if (Input.GetKey(KeyCode.A)) {
			c.Yaw(-1);
		}
		if (Input.GetKey(KeyCode.D)) {
			c.Yaw(1);
		}
		if (Input.GetKey(KeyCode.W)) {
			moveDir = 1;
		}
		if (Input.GetKey(KeyCode.S)) {
			moveDir = -1;
		}

		touchControl.Update();

		if (touchControl.directionY == TouchController.Direction.UP) {
			moveDir = 1;
		} else if (touchControl.directionY == TouchController.Direction.DOWN) {
			moveDir = -1;
		}

		c.Propel(moveDir);


		compassControl.Update();

		if (compassControl.changed) {
			c.YawTo(compassControl.value);
		}
	}

}

public class TouchController
{
	public enum Direction {
		NONE,
		UP,
		DOWN,
		LEFT,
		RIGHT
	}

	private float prePosY, prePosX, turningThreshold = 40;
	public Direction directionX = Direction.NONE;
	public Direction directionY = Direction.NONE;

	public void Update()
	{
		if (Input.touchCount > 0) {
			Touch t = Input.GetTouch(0);

			switch (t.phase) {
			case TouchPhase.Began:
				prePosY = t.position.y;
				prePosX = t.position.x;

				break;
			
			case TouchPhase.Moved:
				float tempY = t.position.y - prePosY, tempX = t.position.x - prePosX;

				if (Mathf.Abs(tempY) > turningThreshold) {
					if (tempY > 0) {
						directionY = Direction.UP;
					} else if (tempY < 0) {
						directionY = Direction.DOWN;
					}

					prePosY = t.position.y;
				}

				if (Mathf.Abs(tempX) > turningThreshold) {
					if (tempX > 0) {
						directionX = Direction.RIGHT;
					} else if (tempX < 0) {
						directionX = Direction.LEFT;
					}

					prePosX = t.position.x;
				}


				break;
			
			case TouchPhase.Ended:
				directionX = Direction.NONE;
				directionY = Direction.NONE;

				break;
			}
		}
	}
}

public class CompassController
{
	private bool flag = true;
	private Quaternion qe;
	private float basis, mh, lmh, llmh, mhft, lmhft;
	public Quaternion value;
	public bool changed = false;

	public CompassController()
	{
		Input.compass.enabled = true;
		Input.gyro.enabled = true;

		basis = 0f;
		lmh = Input.compass.magneticHeading;
		llmh = Input.compass.magneticHeading;
		lmhft = Input.compass.magneticHeading;
	}

	public void Debug()
	{
        GUILayout.Label(Input.gyro.attitude.ToString());
        GUILayout.Label(Input.compass.magneticHeading.ToString());
        GUILayout.Label(basis.ToString());
        GUILayout.Label(mh.ToString());
	}

	public void Update()
	{
		if (flag && Input.compass.magneticHeading != 0)
		{
			basis = Input.compass.magneticHeading;
			flag = false;
		}

		mh = Input.compass.magneticHeading - basis > 0 ? Input.compass.magneticHeading - basis : Input.compass.magneticHeading - basis + 360;
		if (llmh > 270)
		{
			if (lmh < 90)
				lmh += 360;
			if (mh < 90)
				mh += 360;
		}
		else if (lmh > 270)
		{
			if (llmh < 90)
				llmh += 360;
			if (mh < 90)
				mh += 360;
		}
		else if (mh > 270)
		{
			if (llmh < 90)
				llmh += 360;
			if (lmh < 90)
				lmh += 360;
		}

		mhft = 0.3f * llmh + 0.4f * lmh + 0.3f * mh;

		if (lmhft > 270)
		{
			if (mhft < 90)
				mhft += 360;
		}
		else if (mhft > 270)
		{
			if (lmhft < 90)
				lmhft += 360;
		}

		if (lmhft - mhft <= -2 || lmhft - mhft >= 2) {
			value = Quaternion.Euler(0, mhft, 0);

			changed = true;

			llmh = lmh % 360;
			lmh = mh % 360;
			lmhft = mhft % 360;
		} else {
			changed = false;
		}
	}

}