using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider))]
public class WinGameTrigger : MonoBehaviour {

	void OnTriggerEnter(Collider coll)
	{
		if(coll.gameObject.tag != "Player")
			return;

		GlobalEventHandler.GetInstance().ThrowEvent(this, EEventType.GAME_WON, null);
	}
}
