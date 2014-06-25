﻿using UnityEngine;
using System.Collections;

public class FPSCharacterController : MonoBehaviour {

	public float walkSpeed;
	public float runSpeed;
	public float rotateSpeed = 4f;

	public bool doHeadBob = false;
	public Camera playerCamera;

	public PrototypeGameController gctrl;

	Vector2 input;
	bool isMoving;
	float stuff;
	float origPos;
	float origPosX;

	float bobspeed = 4f;

	float previous = 0f;

	Compass cmp;
	bool cmpIsHidden;

	static Vector3 myForward;

	public static Vector3 MyForward
	{
		get { return myForward; }
	}

	void Start()
	{
		isMoving = false;

		cmpIsHidden = false;
		cmp = GetComponentInChildren<Compass>();
		if(cmp == null)
			Debug.LogError("FPSCharacterController: No compass found!");
		origPos = playerCamera.transform.localPosition.y;
		origPosX = playerCamera.transform.localPosition.x;
		if(rigidbody == null)
		{
			Debug.LogError("FPSCharacterController: No rigidbody found!");
			return;
		}

		GlobalEventHandler.GetInstance().RegisterListener(EEventType.MESSAGE_INCOMING, ToggleCompass);
		GlobalEventHandler.GetInstance().RegisterListener(EEventType.MESSAGE_OUTGOING, ToggleCompass);
	}

	void OnDestroy()
	{
		GlobalEventHandler.GetInstance().UnregisterListener(EEventType.MESSAGE_INCOMING, ToggleCompass);
		GlobalEventHandler.GetInstance().UnregisterListener(EEventType.MESSAGE_OUTGOING, ToggleCompass);
	}

	void ToggleCompass (object sender, System.EventArgs args)
	{
		if(cmp != null)
		{
			cmp.gameObject.SetActive(cmpIsHidden);
			cmpIsHidden = !cmpIsHidden;
		}
	}

	void FixedUpdate ()
	{
		if(gctrl.StopGame || gctrl.GamePaused) return;

		float forwards = Input.GetAxis("Vertical");
		float rotate = Input.GetAxis("Horizontal");
		bool run = Input.GetKey(KeyCode.LeftShift);

		float speed = run ? runSpeed : walkSpeed;

		input = new Vector2( rotate, forwards );
		
		// normalize input if it exceeds 1 in combined length:
		if (input.sqrMagnitude > 1) input.Normalize();

		Vector3 desiredMove = transform.forward * input.y * speed;

		float yv = rigidbody.velocity.y;
		
		// Set the rigidbody's velocity according to the ground angle and desired move
		rigidbody.velocity = desiredMove + Vector3.up * yv;

		transform.RotateAround(transform.position, Vector3.up, 0.1f * input.x * rotateSpeed);
		isMoving = (forwards != 0f);

	}

	void Update()
	{
		if(isMoving)
			cmp.gameObject.SetActive(false);
		else if(!cmpIsHidden)
			cmp.gameObject.SetActive(true);

		myForward = transform.forward;

		if(!doHeadBob) return;

		if(isMoving || playerCamera.transform.localPosition.y != origPos || playerCamera.transform.localPosition.x != origPosX)
		{
			stuff += Time.deltaTime*bobspeed;
			Vector3 pos = playerCamera.transform.localPosition;
			pos.y = origPos + Mathf.Sin(stuff) * .04f;
			pos.x = origPosX + Mathf.Sin(stuff) * .04f;
			if(!isMoving)
			{
				if(playerCamera.transform.localPosition.y+.03f >= origPos
				   || playerCamera.transform.localPosition.y-.03f <= origPos)
				{
					stuff = 0f;
					pos.y = origPos;
					pos.x = origPosX;
					bobspeed = 4f;
				}
			}
			if(Mathf.Sin(stuff) == 0f) stuff = 0f;
			if(Mathf.Sin(stuff) < 0f && previous >= 0f || Mathf.Sin(stuff) >= 0f && previous < 0f)
				bobspeed = Random.Range(3f, 6f);
			previous = Mathf.Sin(stuff);
			playerCamera.transform.localPosition = pos;
		}
	}
}
