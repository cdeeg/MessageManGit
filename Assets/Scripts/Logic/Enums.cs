
public enum EEventType
{
	NONE = -1,
	MESSAGE_INCOMING,
	MESSAGE_ACTIVATED,
	MESSAGE_ACTIVATED_PLAYER,
	MESSAGE_OUTGOING,
	TWEET_INCOMING,
	GAME_WON,
	GAME_OVER,
	RESET_ALL,
	OBSTACLE_SET,
	OBSTACLE_REMOVED,
	PAUSE_GAME,
	UPDATE_CLOCK,
	SPECIAL_TWEET,
	SHOVED,

	CHEAT_PAUSE_TIME,
	CHEAT_UNPAUSE_TIME
}

public class ParsedTweet
{
	public int ID { get; private set; }
	public string Sender { get; private set; }
	public string Message { get; private set; }

	// optional
	public int Predecessor { get; private set; }
	public int Successor { get; private set; }

	public ParsedTweet(int id, string send, string msg, int pred = -1, int succ = -1)
	{
		ID = id;
		Sender = send;
		Message = msg;
		Predecessor = pred;
		Successor = succ;
	}
}

public class ParsedMessage : ParsedTweet
{
	public string Answer { get; private set; }

	public ParsedMessage(int id, string send, string msg, string ans, int pred, int succ)
		:base(id, send, msg, pred, succ)
	{
		Answer = ans;
	}
	
	public override string ToString ()
	{
		return string.Format ("[ParsedMessage: ID={0} Sender={1} Message={2} Answer={3} Pred={4} Succ={5}]", ID, Sender, Message, Answer, Predecessor, Successor);
	}
}

public class ParsedObstacleMessage : ParsedMessage
{
	public string Obstacle { get; private set; }

	public ParsedObstacleMessage(int id, string send, string msg, string ans, string obstacle)
		: base(id, send, msg, ans, -1, -1)
	{
		Obstacle = obstacle;
	}
}
