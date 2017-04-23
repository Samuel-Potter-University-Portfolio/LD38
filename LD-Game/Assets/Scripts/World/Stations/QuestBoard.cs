using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestBoard : MonoBehaviour
{

	[SerializeField]
	private GameObject QuestNotifier;
	private float AnimTrack;

	private int lastQuestID = -1;

	public void Open()
	{
        PlayerInput.Main.mQuestOverlay.Open();
		lastQuestID = QuestController.Main.QuestCount;
	}

	void Update()
	{
		if (QuestController.Main.IsQuestActive && QuestController.Main.QuestCount != lastQuestID)
		{
			AnimTrack += Time.deltaTime;
			float v = Mathf.Sin(AnimTrack * 3.0f) * 0.5f;

			QuestNotifier.SetActive(true);
			QuestNotifier.transform.localPosition = new Vector2(1.0f, 5.04f) + Vector2.up * v;
			QuestNotifier.transform.localScale = new Vector2(1, 1) + new Vector2(0.3f, 0.3f) * v;
		}
		else
			QuestNotifier.SetActive(false);
	}
}
