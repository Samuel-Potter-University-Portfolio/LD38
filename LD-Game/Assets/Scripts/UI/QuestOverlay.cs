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
	[SerializeField]
	private Text Time;


	void Start()
	{
		gameObject.SetActive(false);
	}

	void Update ()
	{
		if (Input.GetKey(KeyCode.Escape))
			Close();

		if (QuestController.Main.IsTimedQuest && QuestController.Main.IsQuestActive)
		{
			int time = QuestController.Main.RemainingTime;

			if (QuestController.Main.IsTimedQuest)
				Time.text = time + " SECONDS REMAINING";
			else
				Time.text = "NO TIME LIMIT";

			//Reopen if time is up
			if (time == 0)
				Open();
		}
	}

	public void Open()
	{
		if (!QuestController.Main.IsQuestActive)
		{
			Title.text = "NO ACTIVE QUEST";
			Description.text = "You slacker!\nStop reading the damn board and do your job!";
			Time.text = "";
		}
		else
		{
			bool HandedIn = QuestController.Main.HandInQuest();

            QuestMeta meta = QuestController.Main.CurrentQuest;
            Title.text = meta.Title;
			Description.text = meta.Description;

			if(HandedIn)
				Time.text = "COMPLETED";
			else if (QuestController.Main.IsTimedQuest)
				Time.text = QuestController.Main.RemainingTime + " SECONDS REMAINING";
			else
				Time.text = "NO TIME LIMIT";
		}

		gameObject.SetActive(true);
		PlayerInput.Main.enabled = false;
	}

	public void Close()
	{
		gameObject.SetActive(false);
		PlayerInput.Main.enabled = true;
	}
}
