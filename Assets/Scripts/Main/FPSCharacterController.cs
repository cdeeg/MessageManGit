using UnityEngine;
using System.Collections;

public class FPSCharacterController : MonoBehaviour {

	const float TIME_STOP_ANIM = 0.09f;

	public float walkSpeed;
	public float runSpeed;
	public float rotateSpeed = 4f;

	public bool doHeadBob = false;
	public Camera playerCamera;

	public PrototypeGameController gctrl;

	public float animationSpeed = 1f;
	public float lookAtPhoneRotationDuration = 1f;

	Vector2 input;
	bool isMoving;
	bool wasMoving;
	float stuff;
	float origPos;
	float origPosX;

	float bobspeed = 4f;

	float previous = 0f;

	Compass cmp;
	Animation handsDownAnim;
	bool cmpIsHidden;
	bool msgIsActive;
	bool msgWasActive;

	Quaternion originalRotation;
	GameObject lookAtPhoneTarget;
	Quaternion targetRotation;
	bool isLookingAtPhone = false;
	ParsedMessage msg;

	static Vector3 myForward;

	public static Vector3 MyForward
	{
		get { return myForward; }
	}

	void Start()
	{
		isMoving = false;
		msgIsActive = false;
		msgWasActive = false;
		cmpIsHidden = false;

		cmp = GetComponentInChildren<Compass>();
		if(cmp == null)
			Debug.LogError("FPSCharacterController: No compass found!");
		handsDownAnim = GetComponentInChildren<Animation>();
		if(handsDownAnim == null)
			Debug.LogError("FPSCharacterController: No animation for character found!");
		else
		{
			handsDownAnim.Play();
			handsDownAnim["Take 001"].speed = -animationSpeed;
		}
		
		originalRotation = Quaternion.identity; //playerCamera.transform.rotation;
		GUIPhoneController playerPhoneCtrl = GetComponentInChildren<GUIPhoneController>();
		if(playerPhoneCtrl != null)
		{
			lookAtPhoneTarget = playerPhoneCtrl.gameObject;
//			targetRotation = Quaternion.LookRotation( lookAtPhoneTarget - playerCamera.transform.position);
		}
		else
			Debug.LogError("FPSCharacterController: No phone to look at!");

		origPos = playerCamera.transform.localPosition.y;
		origPosX = playerCamera.transform.localPosition.x;

		if(rigidbody == null)
		{
			Debug.LogError("FPSCharacterController: No rigidbody found!");
			return;
		}

		GlobalEventHandler.GetInstance().RegisterListener(EEventType.MESSAGE_INCOMING, ToggleCompass);
		GlobalEventHandler.GetInstance().RegisterListener(EEventType.MESSAGE_OUTGOING, MsgDone);
		GlobalEventHandler.GetInstance().RegisterListener(EEventType.MESSAGE_ACTIVATED_PLAYER, LookTowardsPhone);
		GlobalEventHandler.GetInstance().RegisterListener(EEventType.MESSAGE_ACTIVATED, MsgActivated);
	}

	void OnDestroy()
	{
		GlobalEventHandler.GetInstance().UnregisterListener(EEventType.MESSAGE_INCOMING, ToggleCompass);
		GlobalEventHandler.GetInstance().UnregisterListener(EEventType.MESSAGE_OUTGOING, MsgDone);
		GlobalEventHandler.GetInstance().UnregisterListener(EEventType.MESSAGE_ACTIVATED_PLAYER, LookTowardsPhone);
		GlobalEventHandler.GetInstance().UnregisterListener(EEventType.MESSAGE_ACTIVATED, MsgActivated);
	}

	void LookTowardsPhone (object sender, System.EventArgs args)
	{
		originalRotation = playerCamera.transform.rotation; //lookAt ? playerCamera.transform.rotation : Quaternion.identity;
		msg = ((TempMessageEventArgs)args).Message;
		StartCoroutine(LookAtPhone(true));
	}

	IEnumerator LookAtPhone(bool lookAt)
	{
		isLookingAtPhone = true;
		float timeElapsed = 0f;

		Vector3 rot = lookAtPhoneTarget.transform.position - playerCamera.transform.position;
		Quaternion target = Quaternion.LookRotation(rot, Vector3.up);

		while(timeElapsed < lookAtPhoneRotationDuration)
		{
			Quaternion current = Quaternion.identity;

			// rotate towards phone
			if(lookAt)
			{
				current = Quaternion.Slerp(originalRotation, target, timeElapsed/lookAtPhoneRotationDuration);
			}
			else // rotate to original rotation
			{
				current = Quaternion.Slerp(target, originalRotation, timeElapsed/lookAtPhoneRotationDuration);
			}

			playerCamera.transform.rotation = current;

			timeElapsed += Time.deltaTime;

			yield return null;
		}

		if(lookAt)
		{
			playerCamera.transform.rotation = target;
			SendActivateEvent();
		}
		else
		{
			playerCamera.transform.rotation = originalRotation;
		}

		yield return null;
	}

	void SendActivateEvent()
	{
		GlobalEventHandler.GetInstance().ThrowEvent(this, EEventType.MESSAGE_ACTIVATED, new MessageEventArgs(msg.Sender,msg.Message,msg.Answer));
	}

	void MsgActivated (object sender, System.EventArgs args)
	{
		msgIsActive = true;
		StartCoroutine(LookAtPhone(false));
		isLookingAtPhone = false;
	}

	void MsgDone (object sender, System.EventArgs args)
	{
		msgIsActive = false;
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
		if(gctrl.StopGame || gctrl.GamePaused || isLookingAtPhone) return;

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
		if(isLookingAtPhone) return;

		if(handsDownAnim.isPlaying && handsDownAnim["Take 001"].time >= (TIME_STOP_ANIM * (1f / animationSpeed))
		   && ( msgIsActive && msgIsActive == msgWasActive || Input.GetKey(KeyCode.LeftShift) ))
		{
			handsDownAnim["Take 001"].speed = 0.0f;
		}
		// show/hide compass
		if(isMoving)
			cmp.gameObject.SetActive(false);
		else if(!cmpIsHidden)
			cmp.gameObject.SetActive(true);

		// play "phone down/phone up" animation if possible
		if(Input.GetKey(KeyCode.LeftShift) != wasMoving && handsDownAnim != null
		   || msgIsActive != msgWasActive)
		{
			if(msgIsActive != msgWasActive) msgWasActive = msgIsActive;

			handsDownAnim.Play();
			handsDownAnim["Take 001"].speed = animationSpeed;

			wasMoving = Input.GetKey(KeyCode.LeftShift);
		}

		// set variable for external scripts
		myForward = transform.forward;

		// no head bob? don't calculate anything
		if(!doHeadBob) return;

		// do bobbing if...
		// 1. character is moving OR
		// 2. the camera has not its original height OR
		// 3. the camera has not its original horizontal position
		if(isMoving || playerCamera.transform.localPosition.y != origPos || playerCamera.transform.localPosition.x != origPosX)
		{
			stuff += Time.deltaTime*bobspeed;
			Vector3 pos = playerCamera.transform.localPosition;
			pos.y = origPos + Mathf.Sin(stuff) * .04f;
			pos.x = origPosX + Mathf.Sin(stuff) * .04f;
			// reset hand position if necessary
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
			// reset (probably) overflowing float
			if(Mathf.Sin(stuff) == 0f) stuff = 0f;
			// add some randomness to the bobbing
			if(Mathf.Sin(stuff) < 0f && previous >= 0f || Mathf.Sin(stuff) >= 0f && previous < 0f)
				bobspeed = Random.Range(3f, 6f);
			// set "previous" value
			previous = Mathf.Sin(stuff);
			playerCamera.transform.localPosition = pos;
		}
	}
}
