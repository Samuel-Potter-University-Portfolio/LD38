using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using UnityEngine.UI;

public class CraftingRequirement : MonoBehaviour {

	[SerializeField]
	private Image ItemImage;
	[SerializeField]
	private Text AmountText;

	private ItemID Item;
	private uint Amount;

	public void Set(ItemID item, uint amount)
	{
		Item = item;
		Amount = amount;
		Redraw();
    }

	public void Redraw()
	{
		int textureID = ItemController.Library[Item].TextureID;
		ItemImage.sprite = ItemController.Main.ItemSheet[textureID];

		uint TotalAmount = PlayerInput.Main.mChestOverlay.Count.ContainsKey(Item) ? PlayerInput.Main.mChestOverlay.Count[Item] : 0;
		AmountText.text = Amount + "/" + TotalAmount;
		AmountText.color = TotalAmount >= Amount ? Color.green : Color.red;
	}
}
