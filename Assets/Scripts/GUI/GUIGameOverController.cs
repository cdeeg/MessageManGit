using UnityEngine;
using System.Collections;

public class GUIGameOverController : MonoBehaviour {

	public tk2dTextMesh gameOverTitleText;
	public tk2dTextMesh gameOverReasonText;

	// Use this for initialization
	void Start ()
	{
//		GlobalEventHandler.GetInstance().RegisterListener(EEventType.GAME_OVER, GameOver);
	}
	
	// Update is called once per frame
	void OnDestroy ()
	{
//		GlobalEventHandler.GetInstance().UnregisterListener(EEventType.GAME_OVER, GameOver);
	}

	void GameOver(object sender, System.EventArgs args)
	{
		GameOverEventArgs goArgs = (GameOverEventArgs)args;
		if(goArgs == null) return;

		if(goArgs.Reason == GameOverEventArgs.EGameOverReason.NO_FRIENDS)
		{
			gameOverTitleText.text = "Game Over";
			gameOverReasonText.text = "You don't have any friends left!";
		}
		else if(goArgs.Reason == GameOverEventArgs.EGameOverReason.TIME_UP)
		{
			gameOverTitleText.text = "Game Over";
			gameOverReasonText.text = "The time is up!"; // TODO change this to "You are late to your date!" or shit
		}
		else
		{
			Debug.LogError("GUIGameOverController: Unknown reason for Game Over!");
		}
	}
}
