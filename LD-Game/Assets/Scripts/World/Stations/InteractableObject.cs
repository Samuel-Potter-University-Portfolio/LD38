using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Events;

public class InteractableObject : MonoBehaviour {

	[SerializeField]
	private GameObject Prompt;
	private bool PlayerInRange;
	[SerializeField]
	private UnityEvent OnInteraction;

	private float PromptAnimation;
	private Vector2 StartScale;

	private float Cooldown;

	void Start ()
	{
		StartScale = Prompt.transform.localScale;
    }
	
	void Update ()
	{
		if (Cooldown > 0.0f)
			Cooldown -= Time.deltaTime;

		//Prompt animation
		if (PlayerInRange)
		{
			Prompt.SetActive(true);
			PromptAnimation += Time.deltaTime * 5.0f;

			float v = Mathf.Sin(PromptAnimation) * 0.5f + 0.5f;
			Prompt.transform.localScale = StartScale + new Vector2(0.2f, 0.2f) * v;
		}
		else
			Prompt.SetActive(false);

		//Functionality
		if (PlayerInRange && Input.GetKey(KeyCode.F) && Cooldown <= 0.0f)
		{
			if(OnInteraction != null)
				OnInteraction.Invoke();
			Cooldown = 1.0f;
        }
    }

	void OnTriggerEnter2D(Collider2D collider)
	{
		if (collider.gameObject.tag == "Player")
			PlayerInRange = true;
	}

	void OnTriggerExit2D(Collider2D collider)
	{
		if (collider.gameObject.tag == "Player")
			PlayerInRange = false;
	}
}
