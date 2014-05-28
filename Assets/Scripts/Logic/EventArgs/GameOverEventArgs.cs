using System;

public class GameOverEventArgs : EventArgs
{
	public EGameOverReason Reason { get; private set; }

	public enum EGameOverReason
	{
		TIME_UP,
		NO_FRIENDS
	}

	public GameOverEventArgs(EGameOverReason reason)
	{
		Reason = reason;
	}
}
