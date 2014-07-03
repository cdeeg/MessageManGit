using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
//using System;
using UnityEngine;

public class MessageCVSParser {

	const int WALK_THRESHOLD = 10;

	delegate void ParserDelegate(string[] args, List<ParsedTweet> targetList);

	private List<ParsedTweet> parsedTweets;
	private List<ParsedMessage> parsedMessages;
//	private List<ParsedObstacleMessage> parsedObstacleMessages;

	private List<ParsedTweet> timeTweets;
	private List<ParsedTweet> friendsTweets;

	private List<ParsedMessage> sentMessages;
	private List<int> sentMessagesIds;
	private List<int> notSentMessagesIds;
	private int queuedMessageId;
	private int timeTweetQueued;
	private int friendsTweetQueued;

	private int prevSentId;
	private ParsedMessage prevSentMessage;

	private static MessageCVSParser instance;

	System.Random rani;

	private MessageCVSParser()
	{
		parsedTweets = new List<ParsedTweet>();
		parsedMessages = new List<ParsedMessage>();
//		parsedObstacleMessages = new List<ParsedObstacleMessage>();

		timeTweets = new List<ParsedTweet>();
		friendsTweets = new List<ParsedTweet>();
		
		sentMessages = new List<ParsedMessage>();
		sentMessagesIds = new List<int>();
		notSentMessagesIds = new List<int>();

		Parse();
	}

	public static MessageCVSParser GetInstance()
	{
		if(instance == null) instance = new MessageCVSParser();

		return instance;
	}

	public void SentMessageCorrectly(bool success)
	{
		if(!success)
		{
			queuedMessageId = -1;
			notSentMessagesIds.Add(prevSentId);
			parsedMessages.Add(prevSentMessage);
		}
		else
		{
			sentMessagesIds.Add(prevSentId);
			sentMessages.Add(prevSentMessage);
		}
	}

	public void Clear()
	{
		queuedMessageId = -1;
		timeTweetQueued = -1;
		friendsTweetQueued = 0;
		timeTweetQueued = 0;
		
		parsedMessages.Clear();
		sentMessagesIds.Clear();
		sentMessages.Clear();
		notSentMessagesIds.Clear();

		parsedTweets.Clear();

		instance = null;
	}

	#region Initialization
	void Parse()
	{
		rani = new System.Random( System.DateTime.Now.Second );

		queuedMessageId = -1;
		timeTweetQueued = -1;
		friendsTweetQueued = 0;
		timeTweetQueued = 0;

		ReadFile("CSV/InstantMessages", ParseMessage);
		ReadFile("CSV/RandomTweetsDateTweets", ParseTweet, parsedTweets);
		ReadFile("CSV/DateTweetsTime", ParseTweet, timeTweets);
		ReadFile("CSV/DateTweetsFriends", ParseTweet, friendsTweets);
	}

	void ReadFile(string fileName, ParserDelegate deleg, List<ParsedTweet> targetList = null)
	{
		bool firstLine = true;
		TextAsset txt = Resources.Load(fileName) as TextAsset;
		string allText = txt.text;

		string[] lines = allText.Split('\n');
		foreach(string line in lines)
		{
			if(!string.IsNullOrEmpty(line))
			{
				string[] entries = line.Split(',');				// split string and check if...
				if(firstLine) { firstLine = false; continue; }	// ...is the first line (header)
				if(string.IsNullOrEmpty(entries[0])) continue;	// ...the line is empty or not initialized
				if(entries[0].StartsWith("/")) continue;		// ...commented out 
				if (entries.Length > 0)
					deleg(entries, targetList);
			}
		}
	}

	void ParseTweet(string[] args, List<ParsedTweet> targetList)
	{
		// not the correct number of arguments? forget it and cancel
		if(args.Length != 5) return;
		
		// check for tweet predecessor/successor
		if(string.IsNullOrEmpty( args[1] )) args[1] = "-1";
		if(string.IsNullOrEmpty( args[2] )) args[2] = "-1";
		
		int id;
		if(!int.TryParse(args[0].Trim(), out id))
			Debug.LogError("Tweet ID was no int! " + args[0]);
		int predec;
		if(!int.TryParse(args[1].Trim(), out predec))
			Debug.LogError("Tweet predecessor was no int! " + args[1]);
		int succ;
		if(!int.TryParse(args[2].Trim(), out succ))
			Debug.LogError("Tweet successor was no int! " + args[2]);

		ParsedTweet tweet = null;
		if(predec > -1 || succ > -1)
			tweet = new ParsedTweet(id, args[3].Trim(), args[4].Trim());
		else
			tweet = new ParsedTweet(id, args[3].Trim(), args[4].Trim(), predec, succ);

		if(tweet != null)
			targetList.Add(tweet);
	}

	void ParseMessage(string[] args, List<ParsedTweet> targetList)
	{
		// not the correct number of arguments? forget it and cancel
		if(args.Length != 6) return;

		// check for message predecessor/successor
		if(string.IsNullOrEmpty( args[4].Trim() )) args[4] = "-1";
		if(string.IsNullOrEmpty( args[5].Trim() )) args[5] = "-1";

		int id;
		if(!int.TryParse(args[0].Trim(), out id))
			Debug.LogError("Message ID was no int! " + args[0]);
		int predec;
		if(!int.TryParse(args[4].Trim(), out predec))
			Debug.LogError("Message predecessor was no int! " + args[4]);
		int succ;
		if(!int.TryParse(args[5].Trim(), out succ))
			Debug.LogError("Message successor was no int! " + args[5]);

		ParsedMessage newMsg = new ParsedMessage(id,args[1].Trim(),args[2].Trim(), args[3].Trim(), predec, succ);
		parsedMessages.Add(newMsg);
		notSentMessagesIds.Add(newMsg.ID);
	}

	void ParseObstacleMessage(string[] args, List<ParsedTweet> targetList)
	{
		if(args.Length != 4) return;
	}
	#endregion

	#region Get Messages
	public ParsedTweet GetTimeUpdatedMessage()
	{
		if(timeTweetQueued < timeTweets.Count)
		{
			ParsedTweet timeTweet = timeTweets[timeTweetQueued];
			timeTweetQueued++;

			return timeTweet;
		}
		return null;
	}

	public ParsedTweet GetFriendsUpdatedMessage()
	{
		if(friendsTweetQueued < timeTweets.Count)
		{
			ParsedTweet timeTweet = timeTweets[friendsTweetQueued];
			friendsTweetQueued++;

			return timeTweet;
		}
		return null;
	}

	public ParsedTweet GetRandomTweet()
	{
		int rand = rani.Next(0, parsedTweets.Count);

		ParsedTweet tweet = parsedTweets[rand];
		parsedTweets.RemoveAt(rand);

		if(parsedTweets.Count == 0)
		{
			parsedTweets.Clear();
			ReadFile("CSV/RandomTweetsDateTweets", ParseTweet);
		}

		return tweet;
	}

	ParsedMessage GetMessageById(int id)
	{
		int idxRem = -1;
		int idxMsg = -1;
		ParsedMessage msg = null;
		for(int x=0; x<parsedMessages.Count;x++)
		{
			if(parsedMessages[x].ID == id)
			{
				msg = parsedMessages[x];
				idxMsg = x;
				break;
			}
		}

		if(idxMsg > -1)
			parsedMessages.RemoveAt(idxMsg);

		for(int idx=0; idx < notSentMessagesIds.Count;idx++)
		{
			if(notSentMessagesIds[idx] == id)
			{
				idxRem = idx;
				break;
			}
		}

		if(idxRem > -1)
			notSentMessagesIds.RemoveAt(idxRem);

		prevSentMessage = msg;
		prevSentId = msg.ID;

		if(parsedMessages.Count == 0)
		{
			parsedMessages = new List<ParsedMessage>();
			parsedMessages.AddRange(sentMessages);
			sentMessages = new List<ParsedMessage>();
			
			sentMessagesIds.Clear();
		}

		if(msg != null)
		{
			if(msg.Successor == -1)
				queuedMessageId = -1;
			else
				queuedMessageId = msg.Successor;
		}
		else
			queuedMessageId = -1;

		return msg;
	}

	public ParsedMessage GetRandomMessage()
	{
		// message has successor? return that instead of random message
		if(queuedMessageId > -1)
			return GetMessageById(queuedMessageId);

		int ran = Random.Range(0, notSentMessagesIds.Count); //rani.Next(0, notSentMessagesIds.Count);
		int unsent = notSentMessagesIds[ran];
		notSentMessagesIds.RemoveAt(ran);
		int count = -1;

		ParsedMessage msg = null;
		for(int x=0; x<parsedMessages.Count;x++)
		{
			if(parsedMessages[x].ID == unsent)
			{
				msg = parsedMessages[x];
				count = x;
				break;
			}
		}

		if(msg == null || count < 0)
			return GetRandomMessage();

		if(msg.ID == msg.Predecessor)
		{
			Debug.LogError("MessageCVSParser: Message predecessor has the same ID as message: "+msg.ID+"!");
			return null;
		}

		int listItem;
		ParsedMessage m = FindPreceedingMessage(msg, out listItem);
		if(m == null)
		{
			Debug.LogError("MessageCVSParser: Message was null for ID "+unsent+"!");
			return null;
		}
		if(listItem == -1) listItem = count;

		prevSentId = m.ID;
		prevSentMessage = m;
		parsedMessages.RemoveAt(listItem);

		if(parsedMessages.Count == 0)
		{
			parsedMessages = new List<ParsedMessage>();
			parsedMessages.AddRange(sentMessages);
			sentMessages = new List<ParsedMessage>();

			sentMessagesIds.Clear();
		}

		queuedMessageId = m.Successor;

		return m;
	}

	ParsedMessage FindPreceedingMessage(ParsedMessage successor, out int listItemNo, int myIndex = -1, int walkingSince = 0)
	{
		// no predecessor? return the current message
		if(successor.Predecessor == -1)
		{
			listItemNo = myIndex;
			return successor;
		}

		// check if predecessor was sent already
		for(int x=0; x<sentMessagesIds.Count;x++)
		{
			if(sentMessagesIds[x] == successor.Predecessor)
			{
				listItemNo = x;
				return successor;
			}
		}

		for(int x=0; x<parsedMessages.Count; x++)
		{
			if(parsedMessages[x].ID == successor.Predecessor)
			{
				listItemNo = x;
				if(walkingSince >= WALK_THRESHOLD) return null;
				walkingSince++;
				return FindPreceedingMessage(parsedMessages[x], out listItemNo, myIndex = x, walkingSince);
			}
		}

		// nothing? return null
		listItemNo = -1;
		return null;
	}

	ParsedObstacleMessage GetRandomObstacleMessage()
	{
		// TODO
		return null;
	}
	#endregion
}
