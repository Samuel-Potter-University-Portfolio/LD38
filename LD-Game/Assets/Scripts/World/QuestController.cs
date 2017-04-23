using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using System.Xml;


public struct QuestMeta
{
	public bool Story;
	public string Title;
	public string Description;

	public Dictionary<ItemID, uint> Get;
	public Dictionary<ItemID, uint> Requires;
	public Dictionary<ItemID, uint> Rewards;
}


public class QuestController : MonoBehaviour {

	public static QuestController Main { get; private set; }
	public static List<QuestMeta> StoryLibrary { get; private set; }
	public static List<QuestMeta> RandomLibrary { get; private set; }
	
	private float QuestCoolDown;
	private int QuestCount = -1;
	public bool IsQuestActive { get; private set; }
	public QuestMeta CurrentQuest { get; private set; }

	void Start ()
	{
		Main = this;
		LibInit();
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
		QuestCoolDown -= Time.deltaTime;
		if (QuestCoolDown <= 0)
		{
			StartNewQuest();
			QuestCoolDown = 60.0f;
        }
    }

	void StartNewQuest()
	{
		IsQuestActive = true;

		//Random Quest
		if (++QuestCount >= StoryLibrary.Count)
		{

		}
		//Story Quest
		else
			CurrentQuest = StoryLibrary[QuestCount];
    }

	public void OnInventoryChange(Dictionary<ItemID, uint> Count)
	{
		if (!IsQuestActive)
			return;
	}
}
