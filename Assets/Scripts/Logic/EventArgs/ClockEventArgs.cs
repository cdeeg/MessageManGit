using System;

public class ClockEventArgs : EventArgs
{
	public int InitTime { get; set; }
	public int TimeLeft { get; set; }

	public ClockEventArgs(int timeLeft, int initTime)
	{
		InitTime = initTime;
		TimeLeft = timeLeft;
	}
}
