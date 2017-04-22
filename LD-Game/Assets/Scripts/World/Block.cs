using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Xml;


public enum BlockID
{
	Dirt = 0,
	Stone,
	Grass,
}

public struct BlockMeta
{
	public bool Destructable;
	public bool Solid;

	public bool FlipX;
	public bool FlipY;

	public uint DestroyTool;
	public uint DroppedItem;
	public uint DroppedAmount;
}


[RequireComponent(typeof(SpriteRenderer))]
public class Block : MonoBehaviour
{
	private static Dictionary<BlockID, BlockMeta> library = new Dictionary<BlockID, BlockMeta>();

	[System.NonSerialized]
	public BlockID id;
	[System.NonSerialized]
	public uint x;
	[System.NonSerialized]
	public uint y;
	
	public void WorldInit(WorldController worldController)
	{
		SpriteRenderer renderer = GetComponent<SpriteRenderer>();
		renderer.sprite = WorldController.Main.TileSheet[(uint)id];

		BlockMeta meta = library[id];

		if (meta.FlipX)
			renderer.flipX = Random.value >= 0.5f;
		if (meta.FlipY)
			renderer.flipY = Random.value >= 0.5f;
	}

	public static void LibInit(WorldController worldController)
	{
		XmlDocument blocksDoc = new XmlDocument();
		blocksDoc.Load("Assets/blocks.xml");

		foreach (XmlNode node in blocksDoc.DocumentElement.ChildNodes)
		{
			BlockID id = (BlockID)GetInt(node.Attributes["ID"]);

			BlockMeta meta = new BlockMeta();

			meta.Destructable = GetBool(node.Attributes["Destructable"]);
			meta.Solid = GetBool(node.Attributes["Solid"]);

			meta.FlipX = GetBool(node.Attributes["FlipX"]);
			meta.FlipY = GetBool(node.Attributes["FlipY"]);
			
            meta.DestroyTool = GetUInt(node.Attributes["DestroyTool"]);
			meta.DroppedItem = GetUInt(node.Attributes["DroppedItem"]);
			meta.DroppedAmount = GetUInt(node.Attributes["DroppedAmount"]);

			library[id] = meta;
		}

		Debug.Log("Loaded " + library.Count + " block meta");
    }

	private static int GetInt(XmlAttribute attrib, int defaultValue = 0)
	{
		if (attrib == null)
			return defaultValue;
		else
			return System.Int32.Parse(attrib.Value);
	}

	private static uint GetUInt(XmlAttribute attrib, uint defaultValue = 0)
	{
		if (attrib == null)
			return defaultValue;
		else
			return System.UInt32.Parse(attrib.Value);
	}

	private static bool GetBool(XmlAttribute attrib, bool defaultValue = false)
	{
		if (attrib == null)
			return defaultValue;
		else
			return attrib.Value.ToLower().Equals("true");
	}

	void Update ()
	{
		
	}
}
