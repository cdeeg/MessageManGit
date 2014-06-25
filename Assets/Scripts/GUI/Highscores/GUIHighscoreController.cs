using UnityEngine;
using System.Collections;

public class GUIHighscoreController : MonoBehaviour {

	public GUIHighscoreRow[] rows;

	void Start()
	{
		if(rows.Length < 6) Debug.LogError("GUIHighscoreController: Not enough rows!");

		int minutes = (int)(HighscoreInformationData.GetInstance().TimePlayed/60f);
		int seconds = (int)(HighscoreInformationData.GetInstance().TimePlayed % 60f);
		string timeString = string.Format("{0:D2}:{1:D2}", minutes, seconds);
		int iniMinutes = (int)(HighscoreInformationData.GetInstance().InitialTime/60f);
		int iniSeconds = (int)(HighscoreInformationData.GetInstance().InitialTime % 60f);
		string initTimeString = string.Format("{0:D2}:{1:D2}", iniMinutes, iniSeconds);

		rows[0].Init("Friends Left", HighscoreInformationData.GetInstance().LeftFriends);
		rows[1].Init("Friends Lost", HighscoreInformationData.GetInstance().LostFriends);
		rows[2].Init("Time Played", timeString, initTimeString);
		rows[3].Init("Missed Messages", HighscoreInformationData.GetInstance().FailedMessages);
		rows[4].Init("Sent Messages", HighscoreInformationData.GetInstance().SuccessfulMessages);

		float scoreOverall = HighscoreInformationData.GetInstance().LeftFriends * (HighscoreInformationData.GetInstance().InitialTime - HighscoreInformationData.GetInstance().TimePlayed);
//		scoreOverall -= HighscoreInformationData.GetInstance().FailedMessages * HighscoreInformationData.GetInstance().ShovedByNpcs;

		rows[5].Init("Score", scoreOverall);

		HighscoreInformationData.GetInstance().Reset();
	}
}
