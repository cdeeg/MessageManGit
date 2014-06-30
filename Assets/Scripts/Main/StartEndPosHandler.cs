using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StartEndPosHandler : MonoBehaviour {

	public GameObject player;
	public GameObject destinationPrefab;

	public StartEndPoints[] points;

	static bool finished;
	static StartEndPoints currentData;

	public static bool IsFinished
	{ get { return finished; }}

	public static StartEndPoints CurrentData
	{
		get { return currentData; }
	}

	void Awake()
	{
		finished = false;
		List<StartEndPoints> checker = new List<StartEndPoints>();
		for(int x=0; x<points.Length; x++)
		{
			if(points[x].startPosition == null || points[x].endPosition == null)
				continue;

			checker.Add(points[x]);
		}

		if(checker.Count == 0)
		{
			Debug.LogWarning("StartEndPosHandler: No valid start-end-point combinations found!");
			return;
		}

		points = checker.ToArray();

		SetStartEndPoints();
	}

	void CheckForNull()
	{
		if(destinationPrefab == null)
		{
			Debug.LogWarning("StartEndPosHandler: No destination prefab found!");
			return;
		}
		
		if(points.Length == 0)
		{
			Debug.LogWarning("StartEndPosHandler: No start-end-point combinations found!");
			return;
		}
	}

	void SetStartEndPoints()
	{
		int rand = Random.Range(0, points.Length);
		currentData = points[rand];

		Vector3 pos = currentData.startPosition.position;
		pos.y = player.transform.position.y;
		player.transform.position = pos;

		finished = true;
	}
}
