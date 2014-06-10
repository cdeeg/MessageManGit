using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(NavMeshAgent))]
public class NPCAgent : MonoBehaviour {

	public float minIdleTimeSpan = 3f;
	public float maxIdleTimeSpan = 5f;
	public float minWalkTimeSpan = 4f;
	public float maxWalkTimeSpan = 10f;

	public float speed = 0.5f;

	Animator animator;
	bool stop;
	Vector3 moveTarget;

	void Start ()
	{
		animator = GetComponent<Animator>();

		stop = false;

		ChooseAction();
	}

	public void SeekPosition(Vector3 playerPos)
	{}

	public void StopActions()
	{
		stop = true;
	}

	public void Update()
	{
		Vector3 pos = Vector3.MoveTowards(transform.position, moveTarget, speed * Time.deltaTime);
		transform.position = pos;
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
			transform.LookAt(target);
			moveTarget = target;


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
