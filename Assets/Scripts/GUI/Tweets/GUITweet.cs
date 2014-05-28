using UnityEngine;
using System.Collections;
using System;

public class GUITweet : MonoBehaviour {

	public tk2dSlicedSprite background;
	public tk2dSlicedSprite icon;
	public tk2dTextMesh senderName;
	public float stayTime = 3f;
	public float fadeTime = 2f;

	public event EventHandler DoneFading;

	float height;
	float moveTime = 1f;
	bool instaFade;
	bool pause;

	public float Height
	{
		get { return height; }
	}

	void Awake()
	{
		GlobalEventHandler.GetInstance().RegisterListener(EEventType.MESSAGE_ACTIVATED, SetInstaFade);
		GlobalEventHandler.GetInstance().RegisterListener(EEventType.PAUSE_GAME, PauseFading);

		pause = false;
	}

	void OnDestroy()
	{
		GlobalEventHandler.GetInstance().UnregisterListener(EEventType.MESSAGE_ACTIVATED, SetInstaFade);
		GlobalEventHandler.GetInstance().UnregisterListener(EEventType.PAUSE_GAME, PauseFading);
	}

	public void Activate(string sender)
	{
		background.gameObject.SetActive(true);
		instaFade = false;
		senderName.text = sender;

		Color col = background.color;
		col.a = 1f;
		background.color = col;
		
		col = senderName.color;
		col.a = 1f;
		senderName.color = col;

		col = icon.color;
		col.a = 1f;
		icon.color = col;

		height = background.dimensions.y;

		StartCoroutine(FadeMe());
	}

	void SetInstaFade (object sender, EventArgs args)
	{
		instaFade = true;
	}

	void PauseFading (object sender, EventArgs args)
	{
		pause = !pause;
	}

	public void Hide()
	{
		background.gameObject.SetActive(false);
	}

	public void MoveToPosition(float yPos)
	{
		StartCoroutine(MoveToPos(yPos));
	}

	void SendEvent()
	{
		if(DoneFading != null)
			DoneFading(this, null);
	}

	#region Coroutines
	IEnumerator MoveToPos(float yPos)
	{
		float passed = 0f;
		while (passed < moveTime)
		{
			if(pause)
			{
				yield return null;
				continue;
			}

			passed += Time.deltaTime;

			Vector3 pos = transform.localPosition;
			pos.y = Mathf.Lerp(pos.y, yPos, passed/moveTime);
			transform.localPosition = pos;

			yield return null;
		}
	}

	IEnumerator FadeMe()
	{
		float passedTime = 0f;

		while(passedTime < stayTime)
		{
			if(pause)
			{
				yield return null;
				continue;
			}

			passedTime += Time.deltaTime;

			if(instaFade)
				passedTime = stayTime;

			yield return null;
		}

		passedTime = 0f;
		Color col = background.color;
		while(passedTime < fadeTime)
		{
			if(pause)
			{
				yield return null;
				continue;
			}

			passedTime += Time.deltaTime;
			if(instaFade)
				passedTime = fadeTime;
			
			col = background.color;
			float alpha = Mathf.Lerp (col.a, 0f, passedTime/fadeTime);

			col.a = alpha;
			background.color = col;

			col = senderName.color;
			col.a = alpha;
			senderName.color = col;

			col = icon.color;
			col.a = alpha;
			icon.color = col;

			yield return null;
		}

		yield return null;

		SendEvent();
	}
	#endregion
}
