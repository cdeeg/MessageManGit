using UnityEngine;
using System.Collections;

public class NPCAgent : MonoBehaviour {

	public float walkRadius = 5f;

	public float maxDistanceRespawn = 10f;
	
	public float speed = 4f;
	
	int failedTimes = 5;
	int failedYet = 0;
	
	Vector3 finalPosition;
	Animation anim;

	bool gamePaused;

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
	void Update ()
	{
		if(gamePaused)
		{
			if(anim.isPlaying)
				anim.Stop();

			return;
		}
		else
		{
			if(!anim.isPlaying)
				anim.Play();
		}

		if(Vector3.Distance(transform.position, finalPosition) <= 3f)
		{
			GetRandomPointOnNavMesh();
		}
		else
		{
			transform.position = Vector3.MoveTowards(transform.position, finalPosition, speed * Time.deltaTime);
		}

		if(Vector3.Distance( NPCHandler.PlayerPos, transform.position) >= NPCHandler.maxDistancePlayer)
		{
			SeekPosition(NPCHandler.PlayerPos, true);
			GetRandomPointOnNavMesh();
		}
	}
}
