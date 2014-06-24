using UnityEngine;
using System.Collections;

public class PlaneArrowScript : MonoBehaviour {

	private GameObject 		target;
	public Quaternion	rotation = Quaternion.identity;
	
	void Start () {
		
		target = GameObject.FindGameObjectWithTag("Player");
		
	}
	
	
	void Update () {
		
		transform.position = new Vector3 (target.transform.position.x,transform.position.y,target.transform.position.z);

		rotation.eulerAngles = new Vector3 (0, target.transform.rotation.y, 0);
//		transform.rotation.eulerAngles = rotation;

		//transform.eulerAngles = new Vector3 (transform.rotation.x,target.transform.rotation.y,transform.rotation.z);
		//transform.localRotation = Quaternion.Euler(transform.rotation.x, target.transform.localRotation.y*100*Time.deltaTime, transform.rotation.z);
		//Quaternion rotation = Quaternion.Euler(new Vector3(0, target.transform.localRotation.y, 0));
		//transform.rotation.eulerAngles = new Vector3(0, target.transform.rotation.y, 0);

		//transform.rotation = target.transform.forward* Time.deltaTime*20;
	}
}
