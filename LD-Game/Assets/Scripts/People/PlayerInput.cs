using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Person))]
public class PlayerInput : MonoBehaviour {


	private Person mPerson;
	
	void Start ()
	{
		mPerson = GetComponent<Person>();
    }
	
	void Update ()
	{
		UpdateMovement();
    }

	void UpdateMovement()
	{
		if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
			mPerson.AddInput(Vector2.left);
		if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
			mPerson.AddInput(Vector2.right);

		if (Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.UpArrow))
			mPerson.AddInput(Vector2.up);

	}
}
