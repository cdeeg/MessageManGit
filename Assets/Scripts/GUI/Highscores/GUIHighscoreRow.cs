using UnityEngine;
using System.Collections;

public class GUIHighscoreRow : MonoBehaviour {

	public tk2dTextMesh nameLabel;
	public tk2dTextMesh scoreLabel;

	public void Init(string name, float score, float ofMax = -1)
	{
		nameLabel.text = name;
		if(ofMax == -1)
			scoreLabel.text = score.ToString();
		else
			scoreLabel.text = string.Format("{0}/{1}", score.ToString(), ofMax.ToString());
	}

	public void Init(string name, string score, string ofMax = "")
	{
		nameLabel.text = name;
		if(string.IsNullOrEmpty(ofMax))
			scoreLabel.text = score.ToString();
		else
			scoreLabel.text = string.Format("{0}/{1}", score, ofMax);
	}
}
