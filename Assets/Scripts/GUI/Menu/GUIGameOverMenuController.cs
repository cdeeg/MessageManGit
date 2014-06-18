using UnityEngine;
using System.Collections;

public class GUIGameOverMenuController : GUIMenuController
{
	public tk2dTextMesh gameOverTitleText;
	public tk2dTextMesh gameOverReasonText;

	void Start()
	{
		base.Init();

		if(gameOverTitleText == null || gameOverReasonText == null) return;

		if(HighscoreInformationData.GetInstance().TimePlayed == HighscoreInformationData.GetInstance().InitialTime)
		{
			gameOverTitleText.text = "Game Over";
			gameOverReasonText.text = "Your date left!";
		}
		else if(HighscoreInformationData.GetInstance().LeftFriends == 0f)
		{
			gameOverTitleText.text = "Game Over";
			gameOverReasonText.text = "You don't have any friends left!";
		}
	}

	void OnGameQuit()
	{
		Application.LoadLevel("MainMenu");
	}
	
	void OnGameRetry()
	{
		Application.LoadLevel("jpscene");
	}
}