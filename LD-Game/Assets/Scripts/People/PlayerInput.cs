using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

[RequireComponent(typeof(Person))]
public class PlayerInput : MonoBehaviour {


	private Person mPerson;

	[SerializeField]
	private RectTransform[] ResourceBars;

	void Start ()
	{
		mPerson = GetComponent<Person>();

		mPerson.HungerBar.SetAnimBar(ResourceBars[0]);
		mPerson.ThirstBar.SetAnimBar(ResourceBars[1]);
		mPerson.StaminaBar.SetAnimBar(ResourceBars[2]);
		mPerson.HealthBar.SetAnimBar(ResourceBars[3]);
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
