using UnityEngine;
using System.Collections;

public class PrototypeGameController : MonoBehaviour {
	
	public float minTimeTweetWait = 2f;
	public float maxTimeTweetWait = 10f;

	public float minTimeMsgWait = 5f;
	public float maxTimeMsgWait = 18f;
	
	bool stop = false;
	bool stopMyself = false;
	bool msgSent = false;
	bool allDone = false;
	bool waitingForSendMessage=false;

	float tweetWaitTime = 4f;
	float currentTweetWaitTimePassed = 0f;

	float msgWaitTime;
	float currentMsgWaitTimePassed = 0f;

	bool canPause = true;

	bool isPaused;

	ParsedMessage currentMessage;

	public bool StopGame
	{
		get { return stop; }
	}

	public bool GamePaused
	{
		get { return isPaused; }
	}

	void OnDestroy()
	{
		GlobalEventHandler.GetInstance().UnregisterListener(EEventType.MESSAGE_OUTGOING, ResetAll);
		GlobalEventHandler.GetInstance().UnregisterListener(EEventType.GAME_OVER, Finished);
		GlobalEventHandler.GetInstance().UnregisterListener(EEventType.GAME_WON, GameWon);
		GlobalEventHandler.GetInstance().UnregisterListener(EEventType.PAUSE_GAME, PauseUnPause);
		GlobalEventHandler.GetInstance().UnregisterListener(EEventType.UPDATE_CLOCK, TimeUpdated);

		MessageCVSParser.GetInstance().Clear();
	}
	
	void Start()
	{
		GlobalEventHandler.GetInstance().RegisterListener(EEventType.MESSAGE_OUTGOING, ResetAll);
		GlobalEventHandler.GetInstance().RegisterListener(EEventType.GAME_OVER, Finished);
		GlobalEventHandler.GetInstance().RegisterListener(EEventType.GAME_WON, GameWon);
		GlobalEventHandler.GetInstance().RegisterListener(EEventType.PAUSE_GAME, PauseUnPause);
		GlobalEventHandler.GetInstance().RegisterListener(EEventType.UPDATE_CLOCK, TimeUpdated);

		msgWaitTime = Random.Range(minTimeMsgWait, maxTimeMsgWait);

		MessageCVSParser.GetInstance().Init();
	}

	void GameWon(object sender, System.EventArgs args)
	{
		allDone = true;
		Application.LoadLevel("Highscores");
	}

	void TimeUpdated (object sender, System.EventArgs args)
	{
		ParsedTweet tweet = MessageCVSParser.GetInstance().GetTimeUpdatedMessage();
		if(tweet != null)
		{
			string msg = tweet.Message;
			if(msg.Contains("{0}"))
			{
				float minutes = Mathf.Ceil( (HighscoreInformationData.GetInstance().InitialTime - HighscoreInformationData.GetInstance().TimePlayed)/60f);
				msg.Replace("{0}", ((int)minutes).ToString());
			}
			GlobalEventHandler.GetInstance().ThrowEvent(this, EEventType.SPECIAL_TWEET, new MessageEventArgs(tweet.Sender, msg, true));
		}
	}

	void PauseUnPause (object sender, System.EventArgs args)
	{
		if(sender.GetType() != typeof(PrototypeGameController))
			isPaused = false;
	}

	void Finished (object sender, System.EventArgs args)
	{
		allDone = true;
		Application.LoadLevel("GameOverScreen");
	}
	
	void ResetAll (object sender, System.EventArgs args)
	{
		stop = false;
		stopMyself = false;
		msgSent = false;
		waitingForSendMessage = false;
	}

	IEnumerator WaitForSecondsPassed()
	{
		yield return new WaitForSeconds(1f);
		canPause = true;
	}

	void CheatsPause()
	{
		canPause = false;
	}

	void EndEverything()
	{
		Application.LoadLevel("MainMenu");
	}
	
	void Update()
	{
		if( allDone || isPaused ) return;

		if(Input.GetKeyUp(KeyCode.Escape))
		{
			isPaused = true;
			GlobalEventHandler.GetInstance().ThrowEvent(this, EEventType.PAUSE_GAME, null);
			return;
		}

		if(currentTweetWaitTimePassed > tweetWaitTime)
		{
			ParsedTweet tweet = MessageCVSParser.GetInstance().GetRandomTweet();
			GlobalEventHandler.GetInstance().ThrowEvent(this, EEventType.TWEET_INCOMING, new MessageEventArgs(tweet.Sender, tweet.Message, false));
			currentTweetWaitTimePassed = 0f;
			tweetWaitTime = Random.Range(minTimeTweetWait, maxTimeTweetWait);
		}

		currentTweetWaitTimePassed += Time.deltaTime;

		if(!stopMyself)
		{
			if(Input.GetKeyDown(KeyCode.P) && canPause)
			{
				GlobalEventHandler.GetInstance().ThrowEvent(this, EEventType.CHEAT_PAUSE_TIME, null);
			}

//			if(Input.GetKeyDown(KeyCode.M))
//			{
//				currentMessage = MessageCVSParser.GetInstance().GetRandomMessage();
//				if(currentMessage == null) return;
//
//				stopMyself = true;
//				GlobalEventHandler.GetInstance().ThrowEvent(this, EEventType.MESSAGE_INCOMING, new MessageEventArgs(currentMessage.Sender, currentMessage.Message, currentMessage.Answer));
//			}

			if(currentMsgWaitTimePassed > msgWaitTime && !waitingForSendMessage)
			{
				waitingForSendMessage = true;
				currentMsgWaitTimePassed = 0f;
				currentMessage = MessageCVSParser.GetInstance().GetRandomMessage();
				if(currentMessage == null)
				{
					waitingForSendMessage = false;
					return;
				}

				stopMyself = true;
				GlobalEventHandler.GetInstance().ThrowEvent(this, EEventType.MESSAGE_INCOMING, new MessageEventArgs(currentMessage.Sender, currentMessage.Message, currentMessage.Answer));
			}
			else if(!waitingForSendMessage) { currentMsgWaitTimePassed += Time.deltaTime; }
		}
		else
		{
			if(Input.GetAxis("Vertical") != 0f || Input.GetAxis("Horizontal") != 0f)
				return;

			if((Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space)) && !msgSent)
			{
				stop = true;
				stopMyself = true;
				msgSent = true;
				msgWaitTime = Random.Range(minTimeMsgWait, maxTimeMsgWait);
				GlobalEventHandler.GetInstance().ThrowEvent(this, EEventType.MESSAGE_ACTIVATED_PLAYER, new TempMessageEventArgs(currentMessage));
			}
		}
	}
	
}
