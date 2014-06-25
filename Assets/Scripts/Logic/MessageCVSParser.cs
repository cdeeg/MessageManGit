using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Text;

public class MessageCVSParser {

//	string RESOURCE_PATH="InstantMessages";
	const int WALK_THRESHOLD = 10;

	delegate void ParserDelegate(string[] args);

//	private List<ParsedTweet> parsedTweets;
	private List<ParsedMessage> parsedMessages;
//	private List<ParsedObstacleMessage> parsedObstacleMessages;

	private List<ParsedMessage> sentMessages;
	private List<int> sentMessagesIds;
	private List<int> notSentMessagesIds;
	private int queuedMessageId;

	private static MessageCVSParser instance;

	private MessageCVSParser()
	{
//		parsedTweets = new List<ParsedTweet>();
		parsedMessages = new List<ParsedMessage>();
//		parsedObstacleMessages = new List<ParsedObstacleMessage>();

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

	public void Clear()
	{
		queuedMessageId = -1;

		parsedMessages.Clear();
		sentMessagesIds.Clear();
		sentMessages.Clear();
		notSentMessagesIds.Clear();

		instance = null;
	}

	#region Initialization
	void Parse()
	{
		queuedMessageId = -1;
		ReadFile("CSV/InstantMessages", ParseMessage);
	}

	void ReadFile(string fileName, ParserDelegate deleg)
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
					deleg(entries);
			}
		}
	}

	void ParseTweet(string[] args)
	{
		if(args.Length != 2) return;
	}

	void ParseMessage(string[] args)
	{
		// not the correct number of arguments? forget it and cancel
		if(args.Length != 6) return;

		// check for message predecessor/successor
		if(string.IsNullOrEmpty( args[4] )) args[4] = "-1";
		if(string.IsNullOrEmpty( args[5] )) args[5] = "-1";

		int id;
		if(!int.TryParse(args[0].Trim(), out id))
			Debug.LogError("Message ID was no int! " + args[0]);
		int predec;
		if(!int.TryParse(args[4].Trim(), out predec))
			Debug.LogError("Message predecessor was no int! " + args[4]);
		int succ;
		if(!int.TryParse(args[4].Trim(), out succ))
			Debug.LogError("Message successor was no int! " + args[5]);

		ParsedMessage newMsg = new ParsedMessage(id,args[1].Trim(),args[2].Trim(), args[3].Trim(), predec, succ);
		parsedMessages.Add(newMsg);
		notSentMessagesIds.Add(newMsg.ID);
	}

	void ParseObstacleMessage(string[] args)
	{
		if(args.Length != 4) return;
	}
	#endregion

	#region Get Messages
	ParsedTweet GetRandomTweet()
	{
		return null;
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

		sentMessages.Add(msg);

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

		int ran = Random.Range(0, notSentMessagesIds.Count);
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

		sentMessages.Add(m);
		sentMessagesIds.Add(m.ID);
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
		return null;
	}
	#endregion
}
