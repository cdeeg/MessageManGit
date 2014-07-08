using UnityEngine;
using System.Collections;
using System;

public class GUITweet : MonoBehaviour {

	public tk2dSlicedSprite background;
	public tk2dSlicedSprite icon;
	public tk2dTextMesh senderName;
	public tk2dTextMesh messageText;
	public float offsetSenderMessage = 5.5f;
	public float offsetTextBackground = 40f;
	public float stayTime = 3f;
	public float fadeTime = 2f;

	public string specialTweetBackground = "bubble_red";
	public string defaultIcon = "";
	public bool alwaysUseDefaultIcon = false;
	
	public event EventHandler DoneFading;

	tk2dSpriteCollection myCollection;
	string normalTweetBackground;
	float height;
	float width;
	float moveTime = 1f;
	bool instaFade;
	bool pause;
	bool specialTweet;
	float minBgSize;
	bool hasMessage = false;

	bool isMoving;
	float targetMovePos;

	public float Height
	{
		get { return height; }
	}

	public float Width
	{
		get { return width; }
	}

	public bool IsSpecial
	{
		get { return specialTweet; }
	}

	void Awake()
	{
		GlobalEventHandler.GetInstance().RegisterListener(EEventType.MESSAGE_ACTIVATED, SetInstaFade);
		GlobalEventHandler.GetInstance().RegisterListener(EEventType.PAUSE_GAME, PauseFading);

		pause = false;
		normalTweetBackground = background.CurrentSprite.name;
	}

	void OnDestroy()
	{
		GlobalEventHandler.GetInstance().UnregisterListener(EEventType.MESSAGE_ACTIVATED, SetInstaFade);
		GlobalEventHandler.GetInstance().UnregisterListener(EEventType.PAUSE_GAME, PauseFading);
	}

	public void Activate(string sender, string pictureName, string message = "", bool useSpecialColor = false)
	{
		background.gameObject.SetActive(true);
		minBgSize = icon.dimensions.y + offsetTextBackground*2;
		instaFade = false;
		specialTweet = useSpecialColor;
		senderName.text = sender;
		Vector3 pos = senderName.transform.localPosition;
		pos.y = background.transform.localPosition.y - offsetTextBackground;
		senderName.transform.localPosition = pos;
		Vector3 iconPos = icon.transform.localPosition;
		iconPos.y = background.transform.localPosition.y - offsetTextBackground;
		icon.transform.localPosition = iconPos;
		float sizeSender = senderName.GetEstimatedMeshBoundsForString(sender).size.y + offsetSenderMessage;
		float backgroundsize = sizeSender + offsetTextBackground*2;
		if(!String.IsNullOrEmpty(message))
		{
			hasMessage = true;
			messageText.gameObject.SetActive(true);
			messageText.text = message;
			messageText.transform.localPosition = new Vector3(senderName.transform.localPosition.x, senderName.transform.localPosition.y - sizeSender, senderName.transform.localPosition.z);
			backgroundsize += messageText.GetEstimatedMeshBoundsForString(message).size.y;
			Vector2 bgScale = background.dimensions;
			bgScale.y = backgroundsize;
			background.dimensions = bgScale;
		}
		else
		{
			messageText.text = "";
			Vector2 bgScale = background.dimensions;
			bgScale.y = backgroundsize;
			background.dimensions = bgScale;
			messageText.gameObject.SetActive(false);
		}

		width = background.dimensions.x;
		height = backgroundsize;
		if(backgroundsize < minBgSize)
		{
			Vector2 bgScale = background.dimensions;
			bgScale.y = minBgSize;
			background.dimensions = bgScale;
			height = minBgSize;
		}

		if(alwaysUseDefaultIcon && !String.IsNullOrEmpty(defaultIcon)) icon.SetSprite(defaultIcon);
		else if(!String.IsNullOrEmpty(pictureName)) icon.SetSprite(pictureName);

		Color col = background.color;
		col.a = 1f;
		background.color = col;

		if(useSpecialColor) background.SetSprite(specialTweetBackground);
		else background.SetSprite(normalTweetBackground);

		col = senderName.color;
		col.a = 1f;
		senderName.color = col;

		col = messageText.color;
		col.a = 1f;
		messageText.color = col;

		col = icon.color;
		col.a = 1f;
		icon.color = col;

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
		targetMovePos = yPos;
		Vector3 myPos = transform.localPosition;
		myPos.z = 6f;
		transform.localPosition = myPos;
		if(!isMoving)
			StartCoroutine(MoveToPos());
	}

	void SendEvent()
	{
		if(DoneFading != null)
			DoneFading(this, null);

		isMoving = false;
		hasMessage = false;
		targetMovePos = 0f;
	}

	#region Coroutines
	IEnumerator MoveToPos()
	{
		isMoving = true;
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
			pos.y = Mathf.Lerp(pos.y, targetMovePos, passed/moveTime);
			transform.localPosition = pos;

			yield return null;
		}
		isMoving = false;

		Vector3 myPos = transform.localPosition;
		myPos.z = 0f;
		transform.localPosition = myPos;
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

			if(hasMessage)
			{
				col = messageText.color;
				col.a = alpha;
				messageText.color = col;
			}

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
