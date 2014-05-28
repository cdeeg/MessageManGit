using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(AudioSource))]
public class GUITweetController : MonoBehaviour {

	private const int INIT_POOL_OBJECTS = 3;

	public GUITweet tweetPrefab;

	public float offset = 6f;
	public bool orientationDown = false;
	public bool isNotificationAnchor = true;

	public AudioClip tweetIncoming;

	AudioSource audio;
	GUITweet[] existingTweets;

	#region Unity methods
	void Awake ()
	{
		if(tweetPrefab == null)
		{
			Debug.LogWarning("GUITweetController: No component found!");
		}

		audio = GetComponent<AudioSource>();

		if(!isNotificationAnchor)
			GlobalEventHandler.GetInstance().RegisterListener(EEventType.TWEET_INCOMING, ShowTweet);
		else
			GlobalEventHandler.GetInstance().RegisterListener(EEventType.MESSAGE_INCOMING, ShowTweet);

		tweetPrefab.CreatePool();
	}

	void OnDestroy()
	{
		if(!isNotificationAnchor)
			GlobalEventHandler.GetInstance().UnregisterListener(EEventType.TWEET_INCOMING, ShowTweet);
		else
			GlobalEventHandler.GetInstance().UnregisterListener(EEventType.MESSAGE_INCOMING, ShowTweet);
	}
	#endregion

	#region Visible stuff
	void ReorderTweets(GUITweet newTweet)
	{
		existingTweets = GetComponentsInChildren<GUITweet>();

		GUITweet tweet;
		for(int x=existingTweets.Length-1; x>=0; x--)
		{
			tweet = existingTweets[x];
			Vector3 pos = tweet.transform.localPosition;
			if(orientationDown)
				pos.y -= newTweet.Height+offset;
			else
				pos.y += newTweet.Height+offset;
			tweet.MoveToPosition(pos.y);
		}

		newTweet.transform.localPosition = Vector3.zero;
	}

	void ShowTweet (object sender, System.EventArgs args)
	{
		MessageEventArgs msgArgs = (MessageEventArgs)args;
		if(msgArgs == null) return;

		GUITweet tweet = tweetPrefab.Spawn(transform);
		tweet.Activate(msgArgs.Sender);
		Vector3 spawnPos = Vector3.zero;
		spawnPos.y -= tweet.Height;
		tweet.transform.localPosition = spawnPos;
		tweet.DoneFading += TweetIsDoneFading;

		if(audio != null) audio.PlayOneShot(tweetIncoming);

		ReorderTweets(tweet);
	}

	void TweetIsDoneFading (object sender, System.EventArgs e)
	{
		GUITweet tweet = (GUITweet)sender;
		if(tweet == null) return;

		tweet.DoneFading -= TweetIsDoneFading;
		tweet.transform.localPosition = Vector3.zero;
		tweet.Hide();
		tweet.Recycle();
	}
	#endregion
}
