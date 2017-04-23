using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarDrawer : MonoBehaviour {

	[SerializeField]
	private GameObject HealthBar;
	[SerializeField]
	private GameObject VillageHealthBar;


	void Update ()
	{
		HealthBar.transform.localScale = new Vector3(PlayerInput.Main.mPerson.NormalizedHealth, 1.0f, 1.0f);
		VillageHealthBar.transform.localScale = new Vector3(VillageDoor.Main.NormalizedHealth, 1.0f, 1.0f);
	}
}
