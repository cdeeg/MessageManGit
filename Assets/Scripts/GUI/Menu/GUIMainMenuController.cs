using UnityEngine;
using System.Collections;

public class GUIMainMenuController : GUIMenuController
{
	void StartNewGame()
	{
<<<<<<< HEAD
		HighscoreInformationData.GetInstance().Reset();
=======
>>>>>>> FETCH_HEAD
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
