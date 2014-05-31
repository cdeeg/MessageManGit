﻿using UnityEngine;
using System.Collections;

public class HighscoreInformationData : MonoBehaviour {

	public float SuccessfulMessages { get; set; }
	public float FailedMessages { get; set; }

	public float LostFriends { get; set; }
	public float LeftFriends { get; set; }

	public float TimePlayed { get; set; }
	public float InitialTime { get; set; }

	public float ShovedByNpcs { get; set; }

	private static HighscoreInformationData myInstance;

	public static HighscoreInformationData GetInstance()
	{
		if(myInstance == null)
			myInstance = new HighscoreInformationData();

		return myInstance;
	}

	private HighscoreInformationData()
	{}
}
