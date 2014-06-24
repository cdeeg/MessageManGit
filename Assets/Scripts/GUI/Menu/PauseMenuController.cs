using UnityEngine;
using System.Collections;

public class PauseMenuController : GUIMenuController {

	void BackToMainMenu()
	{
		Application.LoadLevel("MainMenu");
	}

	void ContinueGame()
	{
		StartCoroutine(WaitBeforeClosing());
	}

	void RestartGame()
	{
		HighscoreInformationData.GetInstance().Reset();
		Application.LoadLevel("jpscene");
	}

	IEnumerator WaitBeforeClosing()
	{
		yield return new WaitForSeconds(0.3f);

		SendEvent();
	}

	void SendEvent()
	{
		GlobalEventHandler.GetInstance().ThrowEvent(this, EEventType.PAUSE_GAME, null);
	}
}
