using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoblinDoor : MonoBehaviour {

	public static bool Active = false;

	[SerializeField]
	private AIInput[] BaseGoblins;

	private float SpawnTimer;
	private int Wave;


	void Start ()
	{
		
	}
	
	void Update ()
	{
		if (!Active)
			return;
		
		SpawnTimer -= Time.deltaTime;

		if (SpawnTimer <= 0.0f)
		{
			Spawn();
			SpawnTimer = 20.0f;
		}
	}

	void Spawn()
	{
        Instantiate(BaseGoblins[0], transform.position, Quaternion.identity);
		++Wave;
	}
}
