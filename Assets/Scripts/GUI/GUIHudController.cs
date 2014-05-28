using UnityEngine;
using System.Collections;

public class GUIHudController : MonoBehaviour {

	public tk2dCamera pauseMenuCamera;
	public GUIMenuController pauseMenu;
	public tk2dCamera hudCamera;

	public PrototypeGameController ctrl;

	bool isPaused;

	void Start()
	{
		isPaused = false;
		GlobalEventHandler.GetInstance().RegisterListener(EEventType.PAUSE_GAME, OnGamePaused);

		pauseMenuCamera.gameObject.SetActive(false);
		hudCamera.gameObject.SetActive(true);
	}

	void OnDestroy()
	{
		GlobalEventHandler.GetInstance().UnregisterListener(EEventType.PAUSE_GAME, OnGamePaused);
	}

	void OnGamePaused (object sender, System.EventArgs args)
	{
		pauseMenuCamera.gameObject.SetActive(!isPaused);
		if(!isPaused)
		{
			pauseMenu.ResetAllItems();
			pauseMenu.Init();
		}
		//hudCamera.gameObject.SetActive(isPaused);
		isPaused = !isPaused;
	}
}
