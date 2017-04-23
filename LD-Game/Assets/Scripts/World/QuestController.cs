using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using System.Xml;


public struct QuestMeta
{
	public bool Story;
	public string Title;
	public string Description;
	public int Time;

	public Dictionary<ItemID, uint> Get;
	public Dictionary<ItemID, uint> Requires;
	public Dictionary<ItemID, uint> Rewards;
}


public class QuestController : MonoBehaviour {

	public static QuestController Main { get; private set; }
	public static List<QuestMeta> StoryLibrary { get; private set; }
	public static List<QuestMeta> RandomLibrary { get; private set; }

	private float QuestCoolDown;
	private float QuestTotalTime;
	public int QuestCount { get; private set; }

	public bool IsQuestActive { get; private set; }
	public bool IsTimedQuest { get; private set; }
	public int RemainingTime { get { return (int)Mathf.Clamp(QuestCoolDown, 0.0f, QuestTotalTime); } }
	public QuestMeta CurrentQuest { get; private set; }

	void Start ()
	{
		Main = this;
		LibInit();
		QuestCount = -1;
    }

	static void LibInit()
	{
		StoryLibrary = new List<QuestMeta>();
		RandomLibrary = new List<QuestMeta>();

		XmlDocument recipeDoc = new XmlDocument();
		recipeDoc.Load("Assets/quests.xml");

		foreach (XmlNode node in recipeDoc.DocumentElement.ChildNodes)
		{
			QuestMeta meta = new QuestMeta();

			meta.Story = XML.GetBool(node.Attributes["Story"]);
			meta.Title = XML.GetString(node.Attributes["Title"]);
			meta.Description = XML.GetString(node.Attributes["Description"]);
			meta.Time = XML.GetInt(node.Attributes["Time"], -1);


			Dictionary<ItemID, uint> Get = new Dictionary<ItemID, uint>();
			Dictionary<ItemID, uint> Requires = new Dictionary<ItemID, uint>();
			Dictionary<ItemID, uint> Rewards = new Dictionary<ItemID, uint>();


			foreach (XmlNode req in node.ChildNodes)
			{
				if (req.Name.ToLower().Equals("get"))
				{
					ItemID ID = (ItemID)XML.GetUInt(req.Attributes["ID"]);
					uint Amount = XML.GetUInt(req.Attributes["Amount"]);
					Get[ID] = Amount;
                }
				if (req.Name.ToLower().Equals("requires"))
				{
					ItemID ID = (ItemID)XML.GetUInt(req.Attributes["ID"]);
					uint Amount = XML.GetUInt(req.Attributes["Amount"]);
					Requires[ID] = Amount;
				}
				if (req.Name.ToLower().Equals("rewards"))
				{
					ItemID ID = (ItemID)XML.GetUInt(req.Attributes["ID"]);
					uint Amount = XML.GetUInt(req.Attributes["Amount"]);
					Rewards[ID] = Amount;
				}
			}

			meta.Get = Get;
			meta.Requires = Requires;
			meta.Rewards = Rewards;

			if(meta.Story)
				StoryLibrary.Add(meta);
			else
				RandomLibrary.Add(meta);
		}

		Debug.Log("Loaded " + StoryLibrary.Count + " story quest meta");
		Debug.Log("Loaded " + RandomLibrary.Count + " random quest meta");
	}

	void Update ()
	{
		if (IsTimedQuest || !IsQuestActive)
			QuestCoolDown -= Time.deltaTime;

		if (!IsQuestActive && QuestCoolDown <= 0.0f)
			StartNewQuest();
	}

	void StartNewQuest()
	{
		Debug.Log("Starting New Quest");
		IsQuestActive = true;

		//Random Quest
		if (++QuestCount >= StoryLibrary.Count)
			CurrentQuest = RandomLibrary[Random.Range(0, RandomLibrary.Count)];

		//Story Quest
		else
			CurrentQuest = StoryLibrary[QuestCount];

		//Give start items
		foreach (KeyValuePair<ItemID, uint> req in CurrentQuest.Get)
		{
			for (int i = 0; i < req.Value; ++i)
				PlayerInput.Main.mChestOverlay.GiveItem(req.Key);
		}

		QuestTotalTime = CurrentQuest.Time;
		QuestCoolDown = QuestTotalTime;
		IsTimedQuest = QuestTotalTime != -1.0f;


		if (QuestCount == 3)
		{
			Debug.Log("Goblin TIME");
			GoblinDoor.Active = true;
		}
    }

	public bool HandInQuest()
	{
		//Time is assumed to be requirement
		if (IsTimedQuest && CurrentQuest.Requires.Count == 0 && QuestCoolDown > 0.0f)
			return false;

		PlayerInput.Main.mChestOverlay.CountInventory();

		foreach (KeyValuePair<ItemID, uint> req in CurrentQuest.Requires)
		{
			if (!PlayerInput.Main.mChestOverlay.Count.ContainsKey(req.Key))
				return false;
			if (PlayerInput.Main.mChestOverlay.Count[req.Key] < req.Value)
				return false;
		}
		
		//Consume
		foreach (KeyValuePair<ItemID, uint> req in CurrentQuest.Requires)
			PlayerInput.Main.mChestOverlay.Consume(req.Key, (int)req.Value);


		//Give reward items
		foreach (KeyValuePair<ItemID, uint> req in CurrentQuest.Rewards)
		{
			for (int i = 0; i < req.Value; ++i)
				PlayerInput.Main.mChestOverlay.GivePlayerItem(req.Key);
		}
		

		//Wait before giving another quest
		QuestCoolDown = 15.0f;
		IsQuestActive = false;

		VillageDoor.Main.OnCompleteQuest(CurrentQuest);
		return true;
	}
}
