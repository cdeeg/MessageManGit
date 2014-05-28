using UnityEngine;
using System.Collections;

public class LocationTrigger : MonoBehaviour {

	public EEventType sendEvent;

	void OnTriggerEnter(Collider coll)
	{
		if(coll.gameObject.tag != "Player")
			return;

		if(sendEvent != EEventType.NONE)
		{
			GlobalEventHandler.GetInstance().ThrowEvent(this, sendEvent, null);
		}
	}
}
