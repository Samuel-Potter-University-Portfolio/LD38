using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using UnityEngine.UI;


public class QuestOverlay : MonoBehaviour
{

	[SerializeField]
	private Text Title;
	[SerializeField]
	private Text Description;


	void Start()
	{
		gameObject.SetActive(false);
	}

	void Update ()
	{
		if (Input.GetKey(KeyCode.Escape))
			Close();
	}

	public void Open()
	{
		Title.text = "NO ACTIVE QUEST";
		Description.text = "Stop reading the damn board and go kill those goblins already!";

		gameObject.SetActive(true);
		PlayerInput.Main.enabled = false;
	}

	public void Close()
	{
		gameObject.SetActive(false);
		PlayerInput.Main.enabled = true;
	}
}
