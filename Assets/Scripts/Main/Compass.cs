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
		
	}
	
	
	void Update ()
	{
		Vector3 RPos  = target.position - transform.position;
		RPos = RPos + ( new Vector3 ( 0, transform.position.y, 0));

		Quaternion newRotation = Quaternion.LookRotation (RPos);
		Vector3 eul = newRotation.eulerAngles;
		eul.y = 0f;
		newRotation.eulerAngles = eul;
		transform.rotation = newRotation;
	}
	
}