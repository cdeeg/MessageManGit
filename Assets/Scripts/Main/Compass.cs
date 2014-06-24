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
		Vector3 RPos = target.position - transform.position;
		Quaternion newRotation = Quaternion.LookRotation (RPos);

		// don't rotate x axis, else the compass will look down onto goal
		Vector3 eul = newRotation.eulerAngles;
		eul.x = 0f;
		newRotation.eulerAngles = eul;

		transform.rotation = newRotation;
	}
	
}