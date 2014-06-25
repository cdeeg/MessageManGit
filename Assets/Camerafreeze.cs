using UnityEngine;
using System.Collections;

public class Camerafreeze : MonoBehaviour {

	public Transform Player;
	
	private float MinZRot, MaxZRot;
	private float ZRot;
	
	void Awake()
	{
		MinZRot = -70.0f;
		MaxZRot = 70.0f;
		ZRot = 0;
	}
	
	void Update ()
	{
		ZRot = -Mathf.Atan2(Player.position.x - transform.position.x, Player.position.y - transform.position.y) * (180 / Mathf.PI);
		ZRot = Mathf.Clamp(ZRot, MinZRot, MaxZRot);
		
		transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, ZRot);
	}
}
