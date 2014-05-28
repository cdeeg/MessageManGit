using UnityEngine;
using System.Collections;

public class HUIPhoneController : MonoBehaviour {

	public GameObject guiSmallPhone;
	public tk2dSlicedSprite background;
	public tk2dSlicedSprite map; // TODO change to ClippedSprite
	public float resizeTime;
	public tk2dSlicedSprite messageIndicator;
	public float defaultMessageOnDisplayTime;

	public GUIMessageController messageView;

	bool messageViewActive;
	bool messageMissed;

	void Start ()
	{
		GlobalEventHandler.GetInstance().RegisterListener(EEventType.MESSAGE_INCOMING, ShowMessageIndicator);
		GlobalEventHandler.GetInstance().RegisterListener(EEventType.MESSAGE_ACTIVATED, FocusOnPhone);
		GlobalEventHandler.GetInstance().RegisterListener(EEventType.MESSAGE_OUTGOING, MinimizePhone);

		messageViewActive = false;
		messageMissed = false;
		messageIndicator.gameObject.SetActive(false);
	}

	void OnDestroy()
	{
		GlobalEventHandler.GetInstance().UnregisterListener(EEventType.MESSAGE_INCOMING, ShowMessageIndicator);
		GlobalEventHandler.GetInstance().UnregisterListener(EEventType.MESSAGE_ACTIVATED, FocusOnPhone);
		GlobalEventHandler.GetInstance().UnregisterListener(EEventType.MESSAGE_OUTGOING, MinimizePhone);
	}

	#region Message Indicator
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

		guiSmallPhone.gameObject.SetActive(false);
		messageMissed = false;
		ToggleMessageView();
	}

	void MinimizePhone (object sender, System.EventArgs args)
	{
		if(sender == this) return;

		ToggleMessageView();
		guiSmallPhone.gameObject.SetActive(true);
	}

	void ToggleMessageView()
	{
		messageViewActive = !messageViewActive;
		messageView.ToggleGUI(messageViewActive);
	}
	#endregion
}
