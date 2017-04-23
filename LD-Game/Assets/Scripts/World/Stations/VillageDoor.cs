using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VillageDoor : MonoBehaviour
{
	public static VillageDoor Main { get; private set; }

	public float Health = 10.0f;
	public float NormalizedHealth {	get { return Mathf.Clamp(Health/10.0f, 0.0f, 1.0f); } }


	void Start ()
	{
		Main = this;	
	}

	public bool Attack(ItemID what, Person who)
	{
		if (who.IsPlayer)
			return false;

		float damage = ItemController.Library.ContainsKey(what) ? ItemController.Library[what].Damage : 0.1f;
		

		Health -= damage;
		if (Health <= 0)
		{
			Health = 0;
			
			Destroy(gameObject);
			//DED
		}
		return true;
	}
	
}
