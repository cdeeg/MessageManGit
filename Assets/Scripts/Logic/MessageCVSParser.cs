using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Text;

public class MessageCVSParser {

	string RESOURCE_PATH="Assets/Resources/CSV/";
	const int WALK_THRESHOLD = 10;

	delegate void ParserDelegate(string[] args);

//	private List<ParsedTweet> parsedTweets;
	private List<ParsedMessage> parsedMessages;
//	private List<ParsedObstacleMessage> parsedObstacleMessages;

	private List<ParsedMessage> sentMessages;
	private List<int> sentMessagesIds;
	private List<int> notSentMessagesIds;

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
		parsedMessages.Clear();
		sentMessagesIds.Clear();
		sentMessages.Clear();
		notSentMessagesIds.Clear();

		instance = null;
	}

	#region Initialization
	void Parse()
	{
		ReadFile("InstantMessages.csv", ParseMessage);
	}

	void ReadFile(string fileName, ParserDelegate deleg)
	{
		string line;
		bool firstLine = true;
		StreamReader theReader = new StreamReader(RESOURCE_PATH + fileName, Encoding.Default);

		using (theReader)
		{
			do
			{
				line = theReader.ReadLine();
				
				if (line != null)
				{
					string[] entries = line.Split(',');
					if(firstLine) { firstLine = false; continue; }
					if(string.IsNullOrEmpty(entries[1])) continue;
					if (entries.Length > 0)
						deleg(entries);
				}
			}
			while (line != null);
			
			theReader.Close();
		}
	}

	void ParseTweet(string[] args)
	{
		if(args.Length != 2) return;
	}

	void ParseMessage(string[] args)
	{
		if(args.Length != 5) return;

		if(string.IsNullOrEmpty( args[4] )) args[4] = "-1";

		ParsedMessage newMsg = new ParsedMessage(int.Parse(args[0]),args[1].Trim(),args[2].Trim(), args[3].Trim(), int.Parse(args[4]));
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

	public ParsedMessage GetRandomMessage()
	{
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
		// check if message is a follow up -> if yes, check if predecessing
		// message was already sent; if not, send it now

//		Debug.Log("MESSG: " + msg);

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

		return m;
	}

	ParsedMessage FindPreceedingMessage(ParsedMessage successor, out int listItemNo, int myIndex = -1, int walkingSince = 0)
	{
//		if(successor == null) Debug.Log("FAIL!");

		// no predecessor? return the current message
		if(successor.Predecessor == -1)
		{
//			Debug.Log("NO PRED! " + successor.ID);
			listItemNo = myIndex;
			return successor;
		}

		// check if predecessor was sent already
		for(int x=0; x<sentMessagesIds.Count;x++)
		{
			if(sentMessagesIds[x] == successor.Predecessor)
			{
//				Debug.Log("FOUND PRED! " + successor.ID);
				listItemNo = x;
				return successor;
			}
		}

		// find predecessor and check for another predecessor
		for(int x=0; x<parsedMessages.Count; x++)
		{
			if(parsedMessages[x].ID == successor.Predecessor)
			{
				listItemNo = x;
//				Debug.Log("SEARCHING ANOTHER PRED FOR ID " + parsedMessages[x].ID + " (WALKING SINCE: "+walkingSince+")");
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
