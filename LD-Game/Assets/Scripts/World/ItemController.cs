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
}
