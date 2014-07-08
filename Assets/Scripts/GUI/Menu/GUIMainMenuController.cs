using UnityEngine;
using System.Collections;

public class GUIMainMenuController : GUIMenuController
{
	void StartNewGame()
	{
		HighscoreInformationData.GetInstance().Reset();
		Application.LoadLevel("TutorialScene");
	}
	
	void QuitGame()
	{
#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
#else
		Application.Quit();
#endif
	}
	
	void ShowCredits()
	{
	}
}
