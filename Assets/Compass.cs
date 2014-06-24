using UnityEngine;
using System.Collections;

public class Compass : MonoBehaviour{
	
	
	public GameObject target;
	
	void Start(){

		target = GameObject.Find ("Finish");
		
	}
	
	
	void Update (){

		Vector3 RPos  = target.transform.position - transform.position;

		Quaternion newRotation = Quaternion.LookRotation (RPos);
		//newRotation.y += 270f;
		//newRotation.x = newRotation.x + 45;
		//newRotation.z = newRotation.z + 45;
		transform.rotation = newRotation;

		
	}
	
}