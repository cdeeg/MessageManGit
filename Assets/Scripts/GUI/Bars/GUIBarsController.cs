using UnityEngine;
using System.Collections;

public class GUIBarsController : MonoBehaviour {

	const float MODULO_THINGY = 60f;

	public GUITweenableBar timeBar;
	public GUITweenableBar friendsBar;

	public tk2dTextMesh countDown;

	float initialTimeInSeconds;
	bool isPaused = false;
	bool allDone = false;
	int initMinutes;

	void Start()
	{
		GlobalEventHandler.GetInstance().RegisterListener(EEventType.MESSAGE_OUTGOING, EvaluateSuccess);
		GlobalEventHandler.GetInstance().RegisterListener(EEventType.CHEAT_PAUSE_TIME, PauseGame);
		GlobalEventHandler.GetInstance().RegisterListener(EEventType.PAUSE_GAME, PauseGame);
		
		initialTimeInSeconds = StartEndPosHandler.CurrentData.timeLimit;

		timeBar.Init(initialTimeInSeconds);
		countDown.text = string.Format("{0:D2}:{1:D2}", (int)(timeBar.CurrentValue/60f), (int)(timeBar.CurrentValue%60f));
		friendsBar.Init(StartEndPosHandler.CurrentData.friends);
		HighscoreInformationData.GetInstance().InitialTime = initialTimeInSeconds;

		StartCoroutine(UpdateTime());
	}

	void OnDestroy()
	{
		GlobalEventHandler.GetInstance().UnregisterListener(EEventType.MESSAGE_OUTGOING, EvaluateSuccess);
		GlobalEventHandler.GetInstance().UnregisterListener(EEventType.CHEAT_PAUSE_TIME, PauseGame);
		GlobalEventHandler.GetInstance().UnregisterListener(EEventType.PAUSE_GAME, PauseGame);
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
		HighscoreInformationData.GetInstance().LostFriends = StartEndPosHandler.CurrentData.friends-friendsBar.CurrentValue;
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
				float timePlayed = Mathf.Abs(timeBar.CurrentValue-initialTimeInSeconds);

				HighscoreInformationData.GetInstance().TimePlayed = timePlayed;

				if(timeBar.CurrentValue % MODULO_THINGY == 0f)
				{
					ClockEventArgs args = new ClockEventArgs((int)(timeBar.CurrentValue/MODULO_THINGY), initialTimeInSeconds);
					GlobalEventHandler.GetInstance().ThrowEvent(this, EEventType.UPDATE_CLOCK, args);
				}

				countDown.text = string.Format("{0:D2}:{1:D2}", (int)(timeBar.CurrentValue/60f), (int)(timeBar.CurrentValue%60f));
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
