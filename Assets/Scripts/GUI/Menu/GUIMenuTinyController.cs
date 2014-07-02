using UnityEngine;
using System.Collections;
using System;

public class GUIMenuTinyController : GUIMenuController {

	public event EventHandler AnswerMessage;
	public event EventHandler DeclineMessage;

	void Start ()
	{
		base.Init();
	}

	void OnEnable()
	{
		base.Init();
	}
	
	void Answer()
	{
		if(AnswerMessage != null)
			AnswerMessage(this, null);
	}

	void Decline()
	{
		if(DeclineMessage != null)
			DeclineMessage(this, null);
	}
}
