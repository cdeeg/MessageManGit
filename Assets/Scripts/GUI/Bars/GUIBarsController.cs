﻿using UnityEngine;
using System.Collections;

public class GUIBarsController : MonoBehaviour {

	const float MODULO_THINGY = 60f;

	public GUITweenableBar timeBar;
	public GUITweenableBar friendsBar;

	public float initialFriendsAmount = 10f;
	public float initialTimeInSeconds = 180f;

	bool isPaused = false;

	bool allDone = false;

	int initMinutes;

	void Start()
	{
		GlobalEventHandler.GetInstance().RegisterListener(EEventType.MESSAGE_OUTGOING, EvaluateSuccess);
//		GlobalEventHandler.GetInstance().RegisterListener(EEventType.GAME_WON, DoNotUpdate);
//		GlobalEventHandler.GetInstance().RegisterListener(EEventType.GAME_OVER, DoNotUpdate);
		GlobalEventHandler.GetInstance().RegisterListener(EEventType.CHEAT_PAUSE_TIME, PauseGame);

		timeBar.Init(initialTimeInSeconds);
		friendsBar.Init(initialFriendsAmount);
		initMinutes = (int)(initialTimeInSeconds/60f);
		HighscoreInformationData.GetInstance().InitialTime = initMinutes;

		StartCoroutine(UpdateTime());
	}

	void OnDestroy()
	{
		GlobalEventHandler.GetInstance().UnregisterListener(EEventType.MESSAGE_OUTGOING, EvaluateSuccess);
//		GlobalEventHandler.GetInstance().UnregisterListener(EEventType.GAME_WON, DoNotUpdate);
//		GlobalEventHandler.GetInstance().UnregisterListener(EEventType.GAME_OVER, DoNotUpdate);
		GlobalEventHandler.GetInstance().UnregisterListener(EEventType.CHEAT_PAUSE_TIME, PauseGame);
	}

	void PauseGame (object sender, System.EventArgs args)
	{
		isPaused = !isPaused;
	}

	void DoNotUpdate (object sender, System.EventArgs args)
	{
		allDone = true;
		StopCoroutine("UpdateTime");
	}

	void EvaluateSuccess (object sender, System.EventArgs args)
	{
		SuccessMessageEventArgs msgArgs = (SuccessMessageEventArgs)args;
		if(msgArgs == null) return;

		if(!msgArgs.Success)
		{
			HighscoreInformationData.GetInstance().FailedMessages++;
			UpdateFriends(-1f, true);
		}
		else
		{
			HighscoreInformationData.GetInstance().SuccessfulMessages++;
			UpdateFriends(1f, true);
		}


		HighscoreInformationData.GetInstance().LeftFriends = friendsBar.CurrentValue;
		HighscoreInformationData.GetInstance().LostFriends = initialFriendsAmount-friendsBar.CurrentValue;
	}

	void UpdateFriends(float value, bool isDelta)
	{
		friendsBar.UpdateBar(value, isDelta);
		if(friendsBar.CurrentValue <= 0f)
		{
			GlobalEventHandler.GetInstance().ThrowEvent(this, EEventType.GAME_OVER, new GameOverEventArgs(GameOverEventArgs.EGameOverReason.NO_FRIENDS));
		}
	}

	void TimeIsUp()
	{
		if(!allDone)
			GlobalEventHandler.GetInstance().ThrowEvent(this, EEventType.GAME_OVER, new GameOverEventArgs(GameOverEventArgs.EGameOverReason.TIME_UP));
	}

	IEnumerator UpdateTime()
	{
		while(timeBar.CurrentValue > 0f)
		{
			yield return new WaitForSeconds(1f);
			if(!isPaused)
			{
				timeBar.UpdateBar(-1f, true);

				HighscoreInformationData.GetInstance().TimePlayed = Mathf.Abs(timeBar.CurrentValue-initialTimeInSeconds);

				if(timeBar.CurrentValue % MODULO_THINGY == 0f)
				{
					ClockEventArgs args = new ClockEventArgs((int)(timeBar.CurrentValue/MODULO_THINGY), initMinutes);
					GlobalEventHandler.GetInstance().ThrowEvent(this, EEventType.UPDATE_CLOCK, args);
				}
			}
			else
			{
				yield return null;
			}
		}

		yield return null;

		TimeIsUp();
	}
}
