using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIInput : MonoBehaviour {

	public Person mPerson { get; private set; }

	void Start()
	{
		mPerson = GetComponent<Person>();
		mPerson.Equip(0);
	}
	
}
