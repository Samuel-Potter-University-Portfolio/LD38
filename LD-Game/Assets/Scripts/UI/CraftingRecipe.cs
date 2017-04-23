using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using UnityEngine.UI;


public class CraftingRecipe : MonoBehaviour {

	[SerializeField]
	private Image ItemImage;
	private CraftingRequirement[] Requirements;
	private RecipeMeta mMeta;
	

	public void Set(RecipeMeta recipe)
	{
        mMeta = recipe;
		Redraw();
    }

	public void Redraw()
	{
		int textureID = ItemController.Library[mMeta.Output].TextureID;
		ItemImage.sprite = ItemController.Main.ItemSheet[textureID];

		if (Requirements == null)
			Requirements = GetComponentsInChildren<CraftingRequirement>();
		if (Requirements.Length == 0)
			return;

		int i = 0;
		foreach (KeyValuePair<ItemID, uint> req in mMeta.Requirements)
		{
			Requirements[i].Set(req.Key, req.Value);
			Requirements[i].gameObject.SetActive(true);

			if (++i >= Requirements.Length-1)
				break;
		}


		for (int n = i; n < Requirements.Length; ++n)
			Requirements[n].gameObject.SetActive(false);
	}

	public void OnPressed()
	{
		if (ItemController.Main.Craft(mMeta))
			PlayerInput.Main.mCraftingOverlay.Redraw();
	}
}
