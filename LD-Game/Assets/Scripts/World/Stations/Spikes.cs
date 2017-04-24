using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Spikes : MonoBehaviour {

	[SerializeField]
	private float Speed = 0.5f;
	[SerializeField]
	private float Damage = 0.2f;
	private List<Person> Affected = new List<Person>();


	void Update()
	{
		for (int i = 0; i < Affected.Count; ++i)
		{
			Person person = Affected[i];

			if (person == null || person.gameObject == null)
			{
				Affected.Remove(person);
				--i;
				continue;
			}

			person.Attack(Damage * Time.deltaTime);
			person.Body.velocity *= Speed;
        }
	}

	void OnTriggerEnter2D(Collider2D collider)
	{
		Person person = collider.gameObject.GetComponent<Person>();
		if (person && !person.IsPlayer)
			Affected.Add(person);
	}

	void OnTriggerExit2D(Collider2D collider)
	{
		Person person = collider.gameObject.GetComponent<Person>();
		if (person && !person.IsPlayer)
		{
			Affected.Remove(person);
		}
	}
	
}
