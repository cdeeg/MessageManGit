using UnityEngine;
using System.Collections;

public class NPCAgent : MonoBehaviour {

	public float walkRadius = 5f;

	public float maxDistanceRespawn = 10f;
	
	public float speed = 4f;
	public float rotateSpeed = 4f;
	
	int failedTimes = 5;
	int failedYet = 0;
	
	Vector3 finalPosition;
	Vector3 oldVelocity = Vector3.zero;
	Animation anim;

	bool gamePaused;
	bool chOldVelo = false;

	void Awake()
	{
		if(rigidbody == null)
			gameObject.AddComponent<Rigidbody>();

		Rigidbody rig = GetComponent<Rigidbody>();

		// don't do weird stuff with rotation (also, no flying!)
		rig.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

		if(collider == null)
			gameObject.AddComponent<CapsuleCollider>();

		CapsuleCollider coll = GetComponent<CapsuleCollider>();
		if(coll != null)
		{
			coll.radius = 1.2f;
		}
	}

	void Start()
	{
		GlobalEventHandler.GetInstance().RegisterListener(EEventType.PAUSE_GAME, StopAnimation);
		gamePaused = false;
		anim = GetComponent<Animation>();
		if(anim == null)
		{
			Debug.LogError("NPCAgent: No Animation component found! Animation won't stop if game is paused!");
		}
	}

	void OnDestroy()
	{
		GlobalEventHandler.GetInstance().UnregisterListener(EEventType.PAUSE_GAME, StopAnimation);
	}

	void StopAnimation (object sender, System.EventArgs args)
	{
		gamePaused = !gamePaused;
	}
	
	void GetRandomPointOnNavMesh()
	{
		if(failedYet == failedTimes)
		{
			Debug.LogError("No point found!");
			finalPosition = Vector3.zero;
			transform.LookAt(finalPosition);
			return;
		}
		Vector3 randomDirection = Random.insideUnitSphere * walkRadius;
		randomDirection += transform.position;
		NavMeshHit hit;
		if( NavMesh.SamplePosition(randomDirection, out hit, walkRadius, 1))
		{
			finalPosition = hit.position;
			transform.LookAt(finalPosition);
			failedYet = 0;
		}
		else
		{
			failedYet++;
			GetRandomPointOnNavMesh();
		}
	}

	public void SeekPosition(Vector3 playerPos, bool withDistance = false)
	{
		Vector3 randDir = Random.insideUnitSphere * walkRadius;
		randDir += NPCHandler.PlayerPos + FPSCharacterController.MyForward*3f;

		NavMeshHit hit;
		if(NavMesh.SamplePosition(randDir, out hit, walkRadius, 1))
		{
			transform.position = hit.position;
			if(withDistance && Vector3.Distance(hit.position, playerPos) <= 10f)
			{
				transform.position = hit.position + new Vector3(Random.Range(maxDistanceRespawn, NPCHandler.maxDistancePlayer - 10f), 0f, Random.Range(maxDistanceRespawn, NPCHandler.maxDistancePlayer - 10f));
			}
		}

		GetRandomPointOnNavMesh();
	}
	
	// Update is called once per frame
	void FixedUpdate ()
	{
		if(gamePaused)
		{
			if(anim.isPlaying)
				anim.Stop();

//			if(oldVelocity.x == 0f && oldVelocity.y == 0f && oldVelocity.z == 0f)
			if(!chOldVelo)
			{
				oldVelocity = rigidbody.velocity;
				chOldVelo = true;
			}

			rigidbody.velocity = Vector3.zero;

			return;
		}
		else
		{
			if(!anim.isPlaying)
				anim.Play();

			//if(oldVelocity.x != 0f && oldVelocity.y != 0f && oldVelocity.z != 0f)
			if(chOldVelo)
			{
				rigidbody.velocity = oldVelocity;
				oldVelocity = Vector3.zero;
				chOldVelo = false;
			}
		}

		if(Vector3.Distance(transform.position, finalPosition) <= 4f)
		{
			GetRandomPointOnNavMesh();
		}
		else
		{
			Vector3 pos = Vector3.MoveTowards(transform.position, finalPosition, speed * Time.deltaTime);
			Vector2 input = new Vector2( pos.z, pos.x );
			if (input.sqrMagnitude > 1) input.Normalize();
			
			Vector3 desiredMove = transform.forward * input.y * speed;
			float yv = rigidbody.velocity.y;
			rigidbody.velocity = desiredMove + Vector3.up * yv;
			transform.RotateAround(transform.position, Vector3.up, 0.1f * input.x * rotateSpeed);
		}

		if(Vector3.Distance( NPCHandler.PlayerPos, transform.position) >= NPCHandler.maxDistancePlayer)
		{
			SeekPosition(NPCHandler.PlayerPos, true);
			GetRandomPointOnNavMesh();
		}
	}
}
