using UnityEngine;
using System.Collections;

public class GUIHighscoreController : MonoBehaviour {

	public GUIHighscoreRow[] rows;
	public PrototypeGameController gameController;

	Camera myCamera;

	void Start ()
	{
		myCamera = GetComponent<Camera>();
		myCamera.enabled = false;

		GlobalEventHandler.GetInstance().RegisterListener(EEventType.GAME_WON, OnGameWon);
	}
	
	void OnDestroy ()
	{
		GlobalEventHandler.GetInstance().UnregisterListener(EEventType.GAME_WON, OnGameWon);
	}

	void OnGameWon (object sender, System.EventArgs args)
	{
		myCamera.enabled = true;
	}
}
