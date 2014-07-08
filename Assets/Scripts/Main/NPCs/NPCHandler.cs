using UnityEngine;
using System.Collections;

public class NPCHandler : MonoBehaviour {

	public Transform playerTransform;
	public int maxNpcs = 20;

	public float distanceFromPlayer = 100f;

	public static float maxDistancePlayer;

	public NPCAgent[] npcPrefabs;

	static Vector3 playerPos;

	public static Vector3 PlayerPos
	{
		get { return playerPos; }
	}

	#region Unity functions
	void Start ()
	{
//		if(agentPrefabMale == null || agentPrefabFemale == null) return;
		if(npcPrefabs.Length == 0) return;

		maxDistancePlayer = distanceFromPlayer;

		playerTransform = GameObject.FindGameObjectWithTag("Player").transform;

		playerPos = playerTransform.position;
		for(int x=0; x<npcPrefabs.Length; x++)
			npcPrefabs[x].CreatePool();

		InitNpcs();
	}

	void Update()
	{
		playerPos = playerTransform.position;
	}

	void OnDestroy()
	{
//		if(agentPrefabMale == null || agentPrefabFemale == null) return;
	}
	#endregion

	#region Start/End
	void InitNpcs()
	{
		for(int x=0; x<maxNpcs; x++)
		{
			// get random NPC prefab, check if it exists, then spawn it
			int rand = Random.Range(0, npcPrefabs.Length);
			if(npcPrefabs[rand] != null) npcPrefabs[rand].Spawn(transform).SeekPosition(playerTransform.position);
			else x--;
		}
	}

	#endregion
}
