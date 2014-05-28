using UnityEngine;
using System.Collections;
using System;

public class ObstacleTrigger : MonoBehaviour {

	public Transform obstaclePosition;
	public GameObject obstacleObject;
	public bool setObstacle = true;

	public string MySender;
	public string MyMessage;
	public string MyAnswer;

	void OnTriggerEnter(Collider coll)
	{
		if(coll.gameObject.tag != "Player")
			return;

		if(setObstacle)
		{
			if(obstaclePosition.childCount > 0) return;

			GameObject myInstance = (GameObject)Instantiate(obstacleObject, obstaclePosition.position, obstacleObject.transform.rotation);
			myInstance.transform.parent = obstaclePosition;
			GlobalEventHandler.GetInstance().ThrowEvent(this, EEventType.OBSTACLE_SET, new MessageEventArgs(MySender, MyMessage, MyAnswer));
		}
		else
		{
			if(obstaclePosition.childCount == 0) return;

			GameObject obs = obstaclePosition.GetChild(0).gameObject;
			obs.gameObject.SetActive(false);
			Destroy((UnityEngine.Object)obs);
			GlobalEventHandler.GetInstance().ThrowEvent(this, EEventType.OBSTACLE_REMOVED, new MessageEventArgs(MySender, MyMessage, MyAnswer));
		}
	}
}
