using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class WeaponSlot : MonoBehaviour {

	private List<Person> InRange;

	void Start ()
	{
		InRange = new List<Person>();
	}

	public void Use(ItemID item, Person who)
	{
		for (int i = 0; i<InRange.Count; ++i)
		{
			Person person = InRange[i];

			if (person != null && person.gameObject != null)
				person.Attack(item, who);
			else
			{
				InRange.Remove(person);
				--i;
			}
		}

		VillageDoor village = VillageDoor.Main;

		if (village == null)
			return;


		Vector2 villageDif = village.transform.position - transform.position;
		const float VillageRange = 2.0f;

		if (villageDif.sqrMagnitude <= VillageRange * VillageRange)
			village.Attack(item, who);
	}

	void OnTriggerEnter2D(Collider2D collider)
	{
		if (collider.gameObject.tag == "Player" || collider.gameObject.tag == "Enemy")
		{
			Person person = collider.gameObject.GetComponent<Person>();
			if (person != null)
				InRange.Add(person);
		}
	}

	void OnTriggerExit2D(Collider2D collider)
	{
		if (collider.gameObject.tag == "Player" || collider.gameObject.tag == "Enemy")
		{
			Person person = collider.gameObject.GetComponent<Person>();
			if (person != null)
				InRange.Remove(person);
		}
	}
}
