using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using UnityEngine.UI;

public class ItemSlot : MonoBehaviour {

	public ItemID ID;
	public ItemMeta mMeta;
	public bool IsHotbar = false;
	[SerializeField]
	private Image ItemImage;
	private RawImage BackgroundImage;

	private Color DefaultColour;


	void Start()
	{
		BackgroundImage = GetComponent<RawImage>();
		if(BackgroundImage != null)
			DefaultColour = BackgroundImage.color;

        SetID(ID);
	}

	public void SetID(ItemID ID)
	{
		this.ID = ID;

		if (ItemImage == null || BackgroundImage == null)
			return;

		if (!ItemController.Library.ContainsKey(ID))
		{
			ItemImage.enabled = false;
			return;
		}

		mMeta = ItemController.Library[ID];
        int textureID = mMeta.TextureID;

		if (textureID == -1)
			ItemImage.enabled = false;
		else
		{
			ItemImage.sprite = ItemController.Main.ItemSheet[textureID];
			ItemImage.enabled = true;
		}
	}

	public void OnPressed()
	{
		if (ChestOverlay.IsOpen)
		{
			if (ID == ItemID.None)
				return;

			//Move into chest
			if (IsHotbar)
			{
				if (PlayerInput.Main.mChestOverlay.GiveItem(ID))
					SetID(ItemID.None);
			}
			//Move into hotbar
			else
			{
				if (PlayerInput.Main.mPerson.GiveItem(ID))
					SetID(ItemID.None);
			}
		}

		//Equip
		else if (IsHotbar)
		{ 
			PlayerInput.Main.mPerson.Equip(this);
        }
	}

	public void UpdateColour(bool selected)
	{
		if (selected)
			BackgroundImage.color = Color.white;
		else
			BackgroundImage.color = DefaultColour;
	}
}
