using UnityEngine;
using System.Collections;

public class GUIMainMenuController : GUIMenuController
{
	void StartNewGame()
	{
		Application.LoadLevel("jpscene");
	}
	
	void QuitGame()
	{
#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
#else
		Application.Quit();
#endif
	}
	
	void ViewHighscores()
	{
	}
}
