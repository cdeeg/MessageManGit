﻿using UnityEngine;
using System.Collections;

public class GUIPhoneController : MonoBehaviour {

	public float resizeTime;
	public tk2dSlicedSprite messageIndicator;
	public float defaultMessageOnDisplayTime;
	
	public GUIMessageController messageView;
	
	bool messageViewActive;
	bool messageMissed;
	bool pause;
	
	void Start ()
	{
		GlobalEventHandler.GetInstance().RegisterListener(EEventType.MESSAGE_INCOMING, ShowMessageIndicator);
		GlobalEventHandler.GetInstance().RegisterListener(EEventType.MESSAGE_ACTIVATED, FocusOnPhone);
		GlobalEventHandler.GetInstance().RegisterListener(EEventType.MESSAGE_OUTGOING, MinimizePhone);
		GlobalEventHandler.GetInstance().RegisterListener(EEventType.PAUSE_GAME, GamePaused);
		
		messageViewActive = false;
		messageMissed = false;
		pause = false;
		messageIndicator.gameObject.SetActive(false);
	}
	
	void OnDestroy()
	{
		GlobalEventHandler.GetInstance().UnregisterListener(EEventType.MESSAGE_INCOMING, ShowMessageIndicator);
		GlobalEventHandler.GetInstance().UnregisterListener(EEventType.MESSAGE_ACTIVATED, FocusOnPhone);
		GlobalEventHandler.GetInstance().UnregisterListener(EEventType.MESSAGE_OUTGOING, MinimizePhone);
		GlobalEventHandler.GetInstance().UnregisterListener(EEventType.PAUSE_GAME, GamePaused);
	}
	
	#region Message Indicator
	void GamePaused (object sender, System.EventArgs args)
	{
		pause = !pause;
	}

	void ShowMessageIndicator (object sender, System.EventArgs args)
	{
		messageIndicator.gameObject.SetActive(true);
		messageMissed = true;
		StartCoroutine(ShowIndicatorOnDisplay());
	}
	
	IEnumerator ShowIndicatorOnDisplay()
	{
		float passedTime = 0f;
		while(passedTime < defaultMessageOnDisplayTime)
		{
			if(pause)
			{
				yield return null;
				continue;
			}

			passedTime += Time.deltaTime;
			
			yield return null;
		}
		
		messageIndicator.gameObject.SetActive(false);
		if(messageMissed)
			GlobalEventHandler.GetInstance().ThrowEvent(this, EEventType.MESSAGE_OUTGOING, new SuccessMessageEventArgs(false));
	}
	#endregion
	
	#region Zoom In/Out
	void FocusOnPhone (object sender, System.EventArgs args)
	{
		if(!messageMissed) return;
		
		messageIndicator.gameObject.SetActive(false);
		messageMissed = false;
		ToggleMessageView();
	}
	
	void MinimizePhone (object sender, System.EventArgs args)
	{
		if(sender == this) return;
		
		ToggleMessageView();
	}
	
	void ToggleMessageView()
	{
		messageViewActive = !messageViewActive;
		messageView.ToggleGUI(messageViewActive);
	}
	#endregion
}
