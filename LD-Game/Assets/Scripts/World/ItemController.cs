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
	Post,
	Sword,
	Hammer,
	Seed,
	Axe,
	Scaffold,
	Key,
	Wheat
}

public enum ToolID
{
	None = 0,
	Pickaxe,
	Axe,
	Sword
}

public struct ItemMeta
{
	public int TextureID;
	public BlockID PlacesBlock;

	public bool Tool;
	public ToolID ToolType;
	public float SwingTime;
	public float Damage;
}

public class ItemController : MonoBehaviour
{
	public static ItemController Main { get; private set; }

	public Sprite[] ItemSheet;
	public static Dictionary<ItemID, ItemMeta> Library { get; private set; }

	private ItemID CraftingItem;
	private float CraftTime = 0;
	private float TotalCraftTime = 0;

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
			meta.PlacesBlock = (BlockID)XML.GetUInt(node.Attributes["PlacesBlock"]);

			meta.Tool = XML.GetBool(node.Attributes["Tool"]);
			meta.ToolType = (ToolID)XML.GetUInt(node.Attributes["ToolType"]);
			meta.SwingTime = (float)XML.GetInt(node.Attributes["SwingTime"]) / 1000.0f;
			meta.Damage = (float)XML.GetInt(node.Attributes["Damage"], 45) /100.0f;

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
				PlayerInput.Main.mCraftingOverlay.OnCraftComplete();
            }
			else
				PlayerInput.Main.mCraftingOverlay.OnCraftUpdate(CraftTime, TotalCraftTime);
        }
	}

	public bool Craft(RecipeMeta recipe)
	{
		if (CraftingItem != ItemID.None)
			return false;

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
		TotalCraftTime = CraftTime;
		CraftingItem = recipe.Output;
		PlayerInput.Main.mCraftingOverlay.OnBeginCraft(CraftingItem);

		Debug.Log("Start Craft " + CraftingItem + " " + CraftTime);
		return true;
	}
}
