
public enum EEventType
{
	NONE = -1,
	MESSAGE_INCOMING,
	MESSAGE_ACTIVATED,
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

	public ParsedTweet(int id, string send, string msg)
	{
		ID = id;
		Sender = send;
		Message = msg;
	}
}

public class ParsedMessage : ParsedTweet
{
	public int Predecessor { get; private set; }
	public int Successor { get; private set; }
	public string Answer { get; private set; }

	public ParsedMessage(int id, string send, string msg, string ans, int pred, int succ)
		:base(id, send, msg)
	{
		Predecessor = pred;
		Successor = succ;
		Answer = ans;
	}

	public override string ToString ()
	{
		return string.Format ("[ParsedMessage: Predecessor={0}, Answer={1}]", Predecessor, Answer);
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
