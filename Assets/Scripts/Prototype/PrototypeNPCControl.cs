using UnityEngine;
using System.Collections;

public class PrototypeNPCControl : MonoBehaviour {

	public Animator animator;
	public float walkRadius = 5f;

	public float speed = 4f;

	int failedTimes = 5;
	int failedYet = 0;

	Vector3 finalPosition;

	// Use this for initialization
	void Start ()
	{
		GetRandomPointOnNavMesh();
	}

	void GetRandomPointOnNavMesh()
	{
		if(failedYet == failedTimes)
		{
			Debug.LogError("No point found!");
			finalPosition = Vector3.zero;
			return;
		}
		Vector3 randomDirection = Random.insideUnitSphere * walkRadius;
		randomDirection += transform.position;
		NavMeshHit hit;
		if( NavMesh.SamplePosition(randomDirection, out hit, walkRadius, 1))
		{
			finalPosition = hit.position;
			transform.LookAt(finalPosition);
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
		if(Vector3.Distance(transform.position, finalPosition) <= 1f)
		{
			GetRandomPointOnNavMesh();
		}
		else
		{
			transform.position = Vector3.MoveTowards(transform.position, finalPosition, speed * Time.deltaTime);
		}
	}
}
