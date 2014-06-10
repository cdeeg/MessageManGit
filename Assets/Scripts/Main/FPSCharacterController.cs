using UnityEngine;
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

	float bobspeed = 4f;

	float previous = 0f;

	void Start()
	{
		isMoving = false;
		origPos = playerCamera.transform.localPosition.y;
		if(rigidbody == null)
		{
			Debug.LogError("FPSCharacterController: No rigidbody found!");
			return;
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
		if(!doHeadBob) return;

		if(isMoving || playerCamera.transform.localPosition.y != origPos)
		{
			stuff += Time.deltaTime*bobspeed;
			Vector3 pos = playerCamera.transform.localPosition;
			pos.y = origPos + Mathf.Sin(stuff) * .04f;
			if(!isMoving)
			{
				if(playerCamera.transform.localPosition.y+.03f >= origPos || playerCamera.transform.localPosition.y-.03f <= origPos)
				{
					stuff = 0f;
					pos.y = origPos;
					bobspeed = 4f;
				}
			}
			if(Mathf.Sin(stuff) == 0f) stuff = 0f;
			if(Mathf.Sin(stuff) < 0f && previous >= 0f || Mathf.Sin(stuff) >= 0f && previous < 0f)
			{
				Debug.Log("CHANGE BOB!");
				bobspeed = Random.Range(3f, 6f);
			}
			previous = Mathf.Sin(stuff);
			playerCamera.transform.localPosition = pos;
		}
	}
}
