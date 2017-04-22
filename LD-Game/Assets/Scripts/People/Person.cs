using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Person : MonoBehaviour {

	public bool IsPlayer = true;

	
	void Start ()
	{
		Camera camera = GetComponentInChildren<Camera>();
		camera.gameObject.SetActive(IsPlayer);

		PlayerInput playerInput = GetComponent<PlayerInput>();
		playerInput.enabled = IsPlayer;

		AIInput aiInput = GetComponent<AIInput>();
		aiInput.enabled = !IsPlayer;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
