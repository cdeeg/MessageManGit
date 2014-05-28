using UnityEngine;
using System.Collections;

public class PrototypeNPCControl : MonoBehaviour {

	public Animator animator;
	public float walkRadius = 5f;

	NavMeshAgent agent;
	int failedTimes = 5;
	int failedYet = 0;

	// Use this for initialization
	void Start ()
	{
		agent = GetComponent<NavMeshAgent>();
		if(agent == null)
		{
			Debug.LogError("No NavMeshAgent found!");
		}
	}

	void GetRandomPointOnNavMesh()
	{
		if(failedYet == failedTimes)
		{
			Debug.LogError("No point found!");
		}
		Vector3 randomDirection = Random.insideUnitSphere * walkRadius;
		randomDirection += transform.position;
		NavMeshHit hit;
		if( NavMesh.SamplePosition(randomDirection, out hit, walkRadius, 1))
		{
			Vector3 finalPosition = hit.position;
			agent.SetDestination(finalPosition);
		}
		else
		{
			failedYet++;
			GetRandomPointOnNavMesh();
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
		if(agent.remainingDistance <= 1f)
		{
			GetRandomPointOnNavMesh();
		}
	}
}
