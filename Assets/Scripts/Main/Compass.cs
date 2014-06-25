using UnityEngine;
using System.Collections;

public class Compass : MonoBehaviour{
	
	Transform target;
	
	void Start()
	{
		bool doLoop = true;
		while(doLoop)
		{
			if(!StartEndPosHandler.IsFinished) continue;

			doLoop = false;
		}

		target = StartEndPosHandler.CurrentEndPoint;
		Debug.Log("TARGET IS:" + target.position);
		
	}
	
	
	void Update (){

		Vector3 RPos  = target.position - transform.position;
		RPos = RPos + ( new Vector3 ( 0, transform.position.y, 0));

		Quaternion newRotation = Quaternion.LookRotation (RPos);
//		newRotation = Quaternion.Euler(0, newRotation.y, 0);
		//newRotation.y += 270f;
		//newRotation.x = newRotation.x + 45;
		//newRotation.z = newRotation.z + 45;
		transform.rotation = newRotation;
	

		
	}
	
}