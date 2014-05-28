using System;

public class MessageEventArgs : EventArgs
{
	public string Sender {get;set;}
	public string Text {get;set;}
	public string Answer {get;set;}

	public MessageEventArgs()
	{
		Sender = "";
		Text = "";
		Answer = "";
	}

	public MessageEventArgs(string sender, string text, string answer)
	{
		Sender = sender;
		Text = text;
		Answer = answer;
	}
}

public class SuccessMessageEventArgs : MessageEventArgs
{
	public bool Success { get; set; }

	public SuccessMessageEventArgs(bool success)
		: base()
	{
		Success = success;
	}
}