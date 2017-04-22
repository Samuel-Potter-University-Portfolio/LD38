using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using UnityEngine.UI;

public class ItemSlot : MonoBehaviour {

	public ItemID ID;
	[SerializeField]
	private Image ItemImage;
	

	void Start()
	{
		SetID(ID);
	}

	public void SetID(ItemID ID)
	{
		this.ID = ID;

		if (!ItemController.Library.ContainsKey(ID))
		{
			ItemImage.enabled = false;
			return;
		}

		int textureID = ItemController.Library[ID].TextureID;

		if (textureID == -1)
			ItemImage.enabled = false;
		else
		{
			ItemImage.sprite = ItemController.Main.ItemSheet[textureID];
			ItemImage.enabled = true;
		}

	}
}
