using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using System.Xml;


public struct RecipeMeta
{
	public ItemID Output;

	public Dictionary<ItemID, uint> Requirements;
	public float Duration;
}


public class RecipeController : MonoBehaviour
{
	public static List<RecipeMeta> Library { get; private set; }


	void Start ()
	{
		LibInit();
    }

	public static void LibInit()
	{
		Library = new List<RecipeMeta>();

		XmlDocument recipeDoc = new XmlDocument();
		recipeDoc.Load("Assets/recipes.xml");

		foreach (XmlNode node in recipeDoc.DocumentElement.ChildNodes)
		{
			RecipeMeta meta = new RecipeMeta();

			meta.Output = (ItemID)XML.GetUInt(node.Attributes["ID"]);
			meta.Duration = (float)XML.GetInt(node.Attributes["Duration"]);
			meta.Requirements = new Dictionary<ItemID, uint>();

			foreach (XmlNode req in node.ChildNodes)
			{
				ItemID id = (ItemID)XML.GetUInt(req.Attributes["ID"]);
				uint Amount = XML.GetUInt(req.Attributes["Amount"]);

				meta.Requirements[id] = Amount;
			}
			
			Library.Add(meta);
		}

		Debug.Log("Loaded " + Library.Count + " recipe meta");
	}
}
