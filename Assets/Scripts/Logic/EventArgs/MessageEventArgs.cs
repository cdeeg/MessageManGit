using System;

public class MessageEventArgs : EventArgs
{
	public string Sender {get;set;}
	public string Text {get;set;}
	public string Answer {get;set;}
	public bool IsSpecial {get;set;}

	public MessageEventArgs()
	{
		Sender = "";
		Text = "";
		Answer = "";
		IsSpecial = false;
	}

	public MessageEventArgs(string sender, string text, string answer)
	{
		Sender = sender;
		Text = text;
		Answer = answer;
		IsSpecial = false;
	}

	public MessageEventArgs(string sender, string text, bool isSpecial)
	{
		Sender = sender;
		Text = text;
		Answer = "";
		IsSpecial = isSpecial;
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