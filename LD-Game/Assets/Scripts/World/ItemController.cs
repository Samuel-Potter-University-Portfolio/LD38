using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using System.Xml;


public enum ItemID
{
	None = 0,
	Stick,
	Log,
	Pickaxe,
	Rock,
	Post
}

public struct ItemMeta
{
	public int TextureID;
	public bool Tool;
	public BlockID PlacesBlock;
}

public class ItemController : MonoBehaviour
{
	public static ItemController Main { get; private set; }

	public Sprite[] ItemSheet;
	public static Dictionary<ItemID, ItemMeta> Library { get; private set; }

	private ItemID CraftingItem;
	private float CraftTime = 0;

	void Start ()
	{
		Main = this;
		LibInit();
    }

	private static void LibInit()
	{
		Library = new Dictionary<ItemID, ItemMeta>();

		XmlDocument itemsDoc = new XmlDocument();
		itemsDoc.Load("Assets/items.xml");

		foreach (XmlNode node in itemsDoc.DocumentElement.ChildNodes)
		{
			ItemID id = (ItemID)XML.GetUInt(node.Attributes["ID"]);

			ItemMeta meta = new ItemMeta();

			meta.TextureID = XML.GetInt(node.Attributes["TextureID"], -1);
			meta.Tool = XML.GetBool(node.Attributes["Tool"]);
			meta.PlacesBlock = (BlockID)XML.GetUInt(node.Attributes["PlacesBlock"]);

			Library[id] = meta;
		}

		Debug.Log("Loaded " + Library.Count + " item meta");
	}

	void Update()
	{
		if (CraftingItem != ItemID.None)
		{
			CraftTime -= Time.deltaTime;
			if (CraftTime <= 0.0f)
			{
				bool given = PlayerInput.Main.mChestOverlay.GiveItem(CraftingItem);
				Debug.Log("Finished Craft Craft Given:" + given);
				CraftingItem = ItemID.None;
			}
        }
	}

	public bool Craft(RecipeMeta recipe)
	{
		PlayerInput.Main.mChestOverlay.CountInventory();

		foreach (KeyValuePair<ItemID, uint> req in recipe.Requirements)
		{
			uint Count = PlayerInput.Main.mChestOverlay.Count.ContainsKey(req.Key) ? PlayerInput.Main.mChestOverlay.Count[req.Key] : 0;

			if (Count < req.Value)
				return false;
        }

		foreach (KeyValuePair<ItemID, uint> req in recipe.Requirements)
			PlayerInput.Main.mChestOverlay.Consume(req.Key, (int)req.Value);

		CraftTime = recipe.Duration;
		CraftingItem = recipe.Output;

		Debug.Log("Start Craft " + CraftingItem + " " + CraftTime);
		return true;
	}
}
