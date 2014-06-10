using UnityEngine;
using System.Collections;

public class NPCHandler : MonoBehaviour {

	public Transform playerTransform;
	public int maxNpcs = 20;

	public NPCAgent agentPrefabMale;
	public NPCAgent agentPrefabFemale;

	#region Unity functions
	void Start ()
	{
		if(agentPrefabMale == null || agentPrefabFemale == null) return;

//		GlobalEventHandler.GetInstance().RegisterListener(EEventType.GAME_OVER, Finish);
//		GlobalEventHandler.GetInstance().RegisterListener(EEventType.GAME_WON, Finish);

		agentPrefabFemale.CreatePool();
		agentPrefabMale.CreatePool();
	}

	void OnDestroy()
	{
		if(agentPrefabMale == null || agentPrefabFemale == null) return;

//		GlobalEventHandler.GetInstance().UnregisterListener(EEventType.GAME_OVER, Finish);
//		GlobalEventHandler.GetInstance().UnregisterListener(EEventType.GAME_WON, Finish);
	}
	#endregion

	#region Start/End
	void InitNpcs()
	{
		maxNpcs = (int)(maxNpcs/2);
		for(int x=0; x<maxNpcs/2; x++)
		{
			agentPrefabFemale.Spawn(transform).SeekPosition(playerTransform.position);
			agentPrefabMale.Spawn(transform).SeekPosition(playerTransform.position);
		}
	}

	void Finish(object sender, System.EventArgs args)
	{
		NPCAgent[] agents = GetComponentsInChildren<NPCAgent>();
		foreach(NPCAgent agent in agents)
			agent.StopActions();
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
