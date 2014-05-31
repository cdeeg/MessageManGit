using UnityEngine;
using System.Collections;

public class GUIHighscoreRow : MonoBehaviour {

	public tk2dTextMesh nameLabel;
	public tk2dTextMesh scoreLabel;

	void Init(string name, string score)
	{
		nameLabel.text = name;
		scoreLabel.text = score;
	}
}
