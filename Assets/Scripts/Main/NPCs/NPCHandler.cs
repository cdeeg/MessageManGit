using UnityEngine;
using System.Collections;

public class NPCHandler : MonoBehaviour {

	public Transform playerTransform;
	public int maxNpcs = 20;

	public float distanceFromPlayer = 100f;

	public static float maxDistancePlayer;

	public NPCAgent agentPrefabMale;
	public NPCAgent agentPrefabFemale;

	static Vector3 playerPos;

	public static Vector3 PlayerPos
	{
		get { return playerPos; }
	}

	#region Unity functions
	void Start ()
	{
		if(agentPrefabMale == null || agentPrefabFemale == null) return;

		maxDistancePlayer = distanceFromPlayer;

		playerPos = playerTransform.position;
		agentPrefabFemale.CreatePool();
		agentPrefabMale.CreatePool();

		InitNpcs();
	}

	void Update()
	{
		playerPos = playerTransform.position;
	}

	void OnDestroy()
	{
		if(agentPrefabMale == null || agentPrefabFemale == null) return;
	}
	#endregion

	#region Start/End
	void InitNpcs()
	{
		maxNpcs = (int)(maxNpcs/2);
		for(int x=0; x<maxNpcs/2; x++)
		{
			agentPrefabFemale.Spawn(transform).SeekPosition(playerTransform.position);
//			agentPrefabMale.Spawn(transform).SeekPosition(playerTransform.position);
		}
	}

	#endregion

	#region Static methods
	public static Vector3 GetRandomPoint(Vector3 startPos)
	{
		// TODO get random accessible point near the NPC with position startPos
		// TODO maybe angles (45° * x) from startPos/Vector3.forward of NPC?
		return Vector3.zero;
	}
	#endregion
}
