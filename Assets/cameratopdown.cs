using UnityEngine;
using System.Collections;

public class cameratopdown : MonoBehaviour {


	private GameObject 		target;


	void Start () {

		target = GameObject.FindGameObjectWithTag("Player");

	}


	void Update () {

		transform.position = new Vector3 (target.transform.position.x,transform.position.y,target.transform.position.z);
	
	}
}
