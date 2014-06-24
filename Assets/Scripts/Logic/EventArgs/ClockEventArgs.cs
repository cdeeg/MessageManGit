using System;

public class ClockEventArgs : EventArgs
{
	public float InitTime { get; set; }
	public int TimeLeft { get; set; }

	public ClockEventArgs(int timeLeft, float initTime)
	{
		InitTime = initTime;
		TimeLeft = timeLeft;
	}
}
