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
		}
	}

	void Spawn()
	{
		if (Wave == 0 || Wave == 1)
			Instantiate(BaseGoblins[0], transform.position, Quaternion.identity);
		else if (Wave == 2 || Wave == 3)
			Instantiate(BaseGoblins[1], transform.position, Quaternion.identity);
		else if (Wave == 4 || Wave == 5)
			Instantiate(BaseGoblins[2], transform.position, Quaternion.identity);
		else
			Instantiate(BaseGoblins[3], transform.position, Quaternion.identity);

		if (Wave == 0)
			SpawnTimer = 100.0f;
		else if (Wave == 1)
			SpawnTimer = 80.0f;
		else if (Wave == 2)
			SpawnTimer = 60.0f;
		else if (Wave == 4)
			SpawnTimer = 40.0f;
		else
			SpawnTimer = 20.0f;

		++Wave;
	}
}
