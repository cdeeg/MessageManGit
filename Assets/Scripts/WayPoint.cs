using UnityEngine;
using System.Collections;

public class WayPoint : MonoBehaviour {

	void OnTriggerEnter(Collider col){
	


		if(col.tag == "NPC" && col.GetComponent<walk>().MyTarget == transform.name){

			col.GetComponent<walk>().Counter2 = 0;
			col.GetComponent<walk>().Waypoints.Add(gameObject);
		}
	}
}
