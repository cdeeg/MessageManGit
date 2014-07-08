using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(AudioSource))]
public class GUITweetController : MonoBehaviour {

	private const int INIT_POOL_OBJECTS = 3;
	private const float OFFSET_SCREEN_BOUNDS = 10f;

	public GUITweet tweetPrefab;

	public float offset = 6f;
	public bool orientationDown = false;
	public bool isNotificationAnchor = true;

	public AudioClip tweetIncoming;

	AudioSource audioSour;
	GUITweet[] existingTweets;

	List<GUITweet> queue;
	bool running;

	#region Unity methods
	void Awake ()
	{
		if(tweetPrefab == null)
		{
			Debug.LogWarning("GUITweetController: No component found!");
		}

		audioSour = GetComponent<AudioSource>();

		if(!isNotificationAnchor)
		{
			GlobalEventHandler.GetInstance().RegisterListener(EEventType.TWEET_INCOMING, ShowTweet);
			GlobalEventHandler.GetInstance().RegisterListener(EEventType.SPECIAL_TWEET, ShowTweet);
		}
		else
			GlobalEventHandler.GetInstance().RegisterListener(EEventType.MESSAGE_INCOMING, ShowTweet);

		tweetPrefab.CreatePool();
		queue = new List<GUITweet>();
		running = false;
	}

	void OnDestroy()
	{
		queue.Clear();
		if(!isNotificationAnchor)
		{
			GlobalEventHandler.GetInstance().UnregisterListener(EEventType.TWEET_INCOMING, ShowTweet);
			GlobalEventHandler.GetInstance().UnregisterListener(EEventType.SPECIAL_TWEET, ShowTweet);
		}
		else
			GlobalEventHandler.GetInstance().UnregisterListener(EEventType.MESSAGE_INCOMING, ShowTweet);
	}
	#endregion

	#region Visible stuff
	IEnumerator ReorderTweets(float newTweetHeight, Vector3 spawnPos)
	{
		running = true;

		GUITweet tweet;
		for(int x=existingTweets.Length-1; x>=0; x--)
		{
			tweet = existingTweets[x];
			Vector3 pos = tweet.transform.localPosition;
			if(orientationDown)
				pos.y -= newTweetHeight+offset;
			else
				pos.y += newTweetHeight+offset;

			tweet.MoveToPosition(pos.y);
//			tweet.transform.localPosition = pos;

			yield return null;
		}

		yield return null;
		
		if(queue.Count > 0)
		{
			GUITweet queuedTweet = queue[0];
			queue.RemoveAt(0);

			if(!orientationDown)
				spawnPos = new Vector3(OFFSET_SCREEN_BOUNDS, OFFSET_SCREEN_BOUNDS+(queuedTweet.Height));
			else
				spawnPos = new Vector3(-OFFSET_SCREEN_BOUNDS, -OFFSET_SCREEN_BOUNDS-(queuedTweet.Height));

			StartCoroutine(ReorderTweets(queuedTweet.Height, spawnPos));
		}
		else
		{
			running = false;
		}
	}

	void ShowTweet (object sender, System.EventArgs args)
	{
		MessageEventArgs msgArgs = (MessageEventArgs)args;
		if(msgArgs == null) return;

		existingTweets = GetComponentsInChildren<GUITweet>();

		GUITweet tweet = tweetPrefab.Spawn(transform);
		string senName = msgArgs.Sender;
		if(senName.StartsWith("@")) senName = senName.Substring(1);
		if(isNotificationAnchor)
			tweet.Activate(msgArgs.Sender, senName);
		else
			tweet.Activate(msgArgs.Sender, senName, msgArgs.Text, msgArgs.IsSpecial);

		Vector3 spawnPos;
		if(!orientationDown)
			spawnPos = new Vector3(OFFSET_SCREEN_BOUNDS, OFFSET_SCREEN_BOUNDS+(tweet.Height));
		else
			spawnPos = new Vector3(-OFFSET_SCREEN_BOUNDS, -OFFSET_SCREEN_BOUNDS-(tweet.Height));

		tweet.transform.localPosition = spawnPos;
		tweet.DoneFading += TweetIsDoneFading;

		if(audioSour != null) audioSour.PlayOneShot(tweetIncoming);

		if(!running) StartCoroutine( ReorderTweets(tweet.Height, spawnPos) );
		else queue.Add(tweet);
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
