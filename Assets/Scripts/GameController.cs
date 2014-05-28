using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour {

	public GameObject gameOverScreen;

	IGameContext currentContext;

	float passedTime = 0f;
	float waitDuration = 6f;

	string sender1 = "Lorem Ipsum";
	string sender2 = "This is a long one";
	string senderThisTime;
	bool stop = false;
	bool msgSent = false;
	bool allDone = false;

	void OnDestroy()
	{
		GlobalEventHandler.GetInstance().UnregisterListener(EEventType.MESSAGE_OUTGOING, ResetAll);
		GlobalEventHandler.GetInstance().UnregisterListener(EEventType.GAME_OVER, Finished);
		GlobalEventHandler.GetInstance().UnregisterListener(EEventType.GAME_WON, GameWon);
	}

	void Start()
	{
		senderThisTime = sender1;
		gameOverScreen.gameObject.SetActive(false);
		GlobalEventHandler.GetInstance().RegisterListener(EEventType.MESSAGE_OUTGOING, ResetAll);
		GlobalEventHandler.GetInstance().RegisterListener(EEventType.GAME_OVER, Finished);
		GlobalEventHandler.GetInstance().RegisterListener(EEventType.GAME_WON, GameWon);
	}

	void GameWon (object sender, System.EventArgs args)
	{
		gameOverScreen.gameObject.SetActive(true);
		allDone = true;
	}

	void Finished (object sender, System.EventArgs args)
	{
		gameOverScreen.gameObject.SetActive(true);
		allDone = true;
	}

	void ResetAll (object sender, System.EventArgs args)
	{
		passedTime = 0f;
		stop = false;
		msgSent = false;
	}

	void Update()
	{
		if( allDone ) return;

		if(!stop)
		{
			if(passedTime > waitDuration)
			{
				stop = true;
				if(senderThisTime == sender1)
					senderThisTime = sender2;
				else
					senderThisTime = sender1;
				GlobalEventHandler.GetInstance().ThrowEvent(this, EEventType.MESSAGE_INCOMING, new MessageEventArgs(senderThisTime, "",""));
			}
			passedTime += Time.deltaTime;
		}
		else
		{
			if((Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space)) && !msgSent)
			{
				msgSent = true;
				GlobalEventHandler.GetInstance().ThrowEvent(this, EEventType.MESSAGE_ACTIVATED, new MessageEventArgs(senderThisTime,"Dolor sit amet, consectetur.","You too, bro, you too"));
			}
		}
	}

}
