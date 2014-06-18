using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class walk : MonoBehaviour {


	private			Animator					Anim;
	private			NavMeshAgent				NavAgent;
	public			List<GameObject> 			Waypoints		 = new List<GameObject>();
	private			int							Counter;
	public			int							Counter2;
	public 			int 						nextpoint;
	public			string						MyTarget;

	void Start () {

		Anim = gameObject.GetComponent<Animator>();
		NavAgent = GetComponent<NavMeshAgent>();
	
	}
	

	void Update () {



		if (Counter<1){
			GameObject[] WPs;
			WPs = GameObject.FindGameObjectsWithTag ("WP");
			foreach(GameObject Child in WPs)
				Waypoints.Add (Child);
			Counter++;


		}
		if (Counter2<1){

			nextpoint = Random.Range(0,Waypoints.Count-1);
			NavAgent.SetDestination(Waypoints[nextpoint].transform.position);
			MyTarget = Waypoints[nextpoint].transform.name;
			Waypoints.Remove(Waypoints[nextpoint]);
			Counter2++;
		}
		Anim.SetFloat("Speed",1);
	}
}
