using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(NavMeshAgent))]
public class NPCAgent : MonoBehaviour {

	public float minIdleTimeSpan = 3f;
	public float maxIdleTimeSpan = 5f;
	public float minWalkTimeSpan = 4f;
	public float maxWalkTimeSpan = 10f;

	Animator animator;
	NavMeshAgent agent;
	bool stop;

	void Start ()
	{
		animator = GetComponent<Animator>();
		agent = GetComponent<NavMeshAgent>();

		stop = false;

		ChooseAction();
	}

	public void SeekPosition(Vector3 playerPos)
	{}

	public void StopActions()
	{
		stop = true;
	}

	void ChooseAction()
	{
		if(stop) return;

		// TODO
		// animator.SetBool(Animator.StringToHash("Idle"), false);
		// animator.SetBool(Animator.StringToHash("Walk"), false);

		if(Random.value >= .5f)
		{
			// idle
			float idleDur = Random.Range(minIdleTimeSpan, maxIdleTimeSpan);

			// TODO
			// animator.SetBool(Animator.StringToHash("Idle"), true);

			StartCoroutine(DoWait(idleDur));
		}
		else
		{
			// walk to point
			Vector3 target = NPCHandler.GetRandomPoint(transform.position);
			agent.SetDestination(target);

			// TODO
			// animator.SetBool(Animator.StringToHash("Walk"), true);

			float walkDur = Random.Range(minWalkTimeSpan, maxWalkTimeSpan);
			StartCoroutine(DoWait(walkDur));
		}
	}

	IEnumerator DoWait(float duration)
	{
		float passedTime = 0f;
		while(passedTime < duration)
		{
			passedTime += Time.deltaTime;

			yield return null;
		}

		ChooseAction();
	}
}
