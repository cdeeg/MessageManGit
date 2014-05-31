using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;

[RequireComponent(typeof(AudioSource))]
public class GUIMessageController : MonoBehaviour
{
	public GameObject guiAnchor;
	public float defaultAnswerTime = 8f;
	public GameObject indicator;

	public tk2dTextMesh senderText;
	public tk2dTextMesh messageText;
	public tk2dTextMesh answerText;

	public Color colorSenderName = Color.blue;
	public Color colorOpaque = Color.grey;
	public Color colorCorrect = Color.black;
	public Color colorWrong = Color.red;
	public string colorPrefix = "^g";

	public AudioClip hitKey;
	public AudioClip messageDone;

	string colorCodeSenderName="";
	string colorCodeOpaque="";
	string colorCodeCorrect="";
	string colorCodeWrong="";

	char[] currentMessage;
	string unformattedText = "";
	bool messageAnswered;
	bool stopCoroutines;

	AudioSource audio;

	StringBuilder tmpString;

	void Start()
	{
		GlobalEventHandler.GetInstance().RegisterListener(EEventType.MESSAGE_ACTIVATED, SetMessage);
		GlobalEventHandler.GetInstance().RegisterListener(EEventType.SHOVED, ShovedByNpcs);
		guiAnchor.gameObject.SetActive(false);

		audio = GetComponent<AudioSource>();
		if(audio == null) Debug.LogWarning("GUIMessageController: No audio source found!");

		colorCodeSenderName = colorPrefix+ColorToHexString(colorSenderName);
		colorCodeOpaque = colorPrefix+ColorToHexString(colorOpaque);
		colorCodeCorrect = colorPrefix+ColorToHexString(colorCorrect);
		colorCodeWrong = colorPrefix+ColorToHexString(colorWrong);
		tmpString = new StringBuilder();
	}

	void OnDestroy()
	{
		GlobalEventHandler.GetInstance().UnregisterListener(EEventType.MESSAGE_ACTIVATED, SetMessage);
		GlobalEventHandler.GetInstance().UnregisterListener(EEventType.SHOVED, ShovedByNpcs);
	}

	public void ToggleGUI(bool visible)
	{
		guiAnchor.gameObject.SetActive(visible);
	}

	string ColorToHexString(Color c)
	{
		StringBuilder build = new StringBuilder();
		byte b = (byte)(c.r*255);
		build.Append(string.Format("{0:x2}",b));
		b = (byte)(c.g*255);
		build.Append(string.Format("{0:x2}",b));
		b = (byte)(c.b*255);
		build.Append(string.Format("{0:x2}",b));
		b = (byte)(c.a*255);
		build.Append(string.Format("{0:x2}",b));
		return build.ToString();
	}

	#region Event methods
	void ShovedByNpcs (object sender, EventArgs args)
	{
		messageAnswered = false;
		HighscoreInformationData.GetInstance().FailedMessages++;
	}

	void SetMessage (object sender, System.EventArgs args)
	{
		if(indicator != null && !indicator.gameObject.activeSelf) return;
		MessageEventArgs msgArgs = (MessageEventArgs)args;
		if(msgArgs == null) return;

		messageAnswered = false;
		stopCoroutines = false;

		senderText.text = colorCodeSenderName + msgArgs.Sender;
		messageText.text = colorCodeCorrect + msgArgs.Text;
		answerText.text = colorCodeOpaque + msgArgs.Answer;

		unformattedText = msgArgs.Answer;
		currentMessage = msgArgs.Answer.ToCharArray();

		// show big phone
		guiAnchor.gameObject.SetActive(true);

		StartCoroutine(UserTyping());
	}

	void SendFinishEvent()
	{
		if(audio != null) audio.PlayOneShot(messageDone);
		if(messageAnswered) HighscoreInformationData.GetInstance().SuccessfulMessages++;
		GlobalEventHandler.GetInstance().ThrowEvent(this, EEventType.MESSAGE_OUTGOING, new SuccessMessageEventArgs(messageAnswered));
		guiAnchor.gameObject.SetActive(false);
	}
	#endregion

	#region Coroutines
	IEnumerator HighlightText(int subIndex, bool isWrong)
	{
		yield return null;

		tmpString = new StringBuilder();
		string sub = unformattedText.Substring(0, subIndex);
		if(isWrong)
		{
			// red character
			tmpString.Append(colorCodeCorrect);
			tmpString.Append(sub);
			tmpString.Append(colorCodeWrong);
			if(currentMessage[subIndex] == ' ')
				tmpString.Append("_");
			else
				tmpString.Append(currentMessage[subIndex]);
			tmpString.Append(colorCodeOpaque);
			tmpString.Append(unformattedText.Substring(subIndex+1));
		}
		else
		{
			tmpString.Append(colorCodeCorrect);
			tmpString.Append(sub);
			tmpString.Append(currentMessage[subIndex]);
			tmpString.Append(colorCodeOpaque);
			tmpString.Append(unformattedText.Substring(subIndex+1));
		}

		answerText.text = tmpString.ToString();
	}

	IEnumerator UserTyping()
	{
		yield return null;

		int arrayIndex = 0;
		char currentChar = currentMessage[arrayIndex];
		while(!messageAnswered && !stopCoroutines)
		{
			if(Input.anyKeyDown)
			{
				if(Input.inputString.Length > 0)
				{
					char inp = Input.inputString.ToCharArray()[0];
					if(inp == currentChar)
					{
						yield return StartCoroutine(HighlightText(arrayIndex, false));
						if(audio != null) audio.PlayOneShot(hitKey);
						arrayIndex++;
						if(arrayIndex == currentMessage.Length)
						{
							messageAnswered = true;
							continue;
						}
						else
						{
							currentChar = currentMessage[arrayIndex];
						}
					}
					else
					{
						yield return StartCoroutine(HighlightText(arrayIndex, true));
					}
				}

				// wait two frames!
				yield return null;
			}

			yield return null;
		}

		if(!stopCoroutines)
			SendFinishEvent();

		stopCoroutines = true;
	}
	#endregion
}
