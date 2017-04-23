using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingOverlay : MonoBehaviour {

	public static CraftingOverlay Main { get; private set; }

	private CraftingRecipe[] RecipeSlots;
	
	void Start ()
	{
		Main = this;
		gameObject.SetActive(false);
    }

	public void Redraw()
	{
		PlayerInput.Main.mChestOverlay.CountInventory();

		if (RecipeSlots == null)
			RecipeSlots = GetComponentsInChildren<CraftingRecipe>();
		if (RecipeSlots.Length == 0)
			return;

		int i = 0;
		foreach (RecipeMeta meta in RecipeController.Library)
		{
			RecipeSlots[i].Set(meta);
			RecipeSlots[i].gameObject.SetActive(true);

			if (++i >= RecipeSlots.Length - 1)
				break;
		}

		for (int n = i; n < RecipeSlots.Length; ++n)
			RecipeSlots[n].gameObject.SetActive(false);
	}

	void Update()
	{
		if (Input.GetKey(KeyCode.Escape))
			Close();
	}

	public void Open()
	{
		gameObject.SetActive(true);
		PlayerInput.Main.enabled = false;
		Redraw();
	}

	public void Close()
	{
		gameObject.SetActive(false);
		PlayerInput.Main.enabled = true;
	}
}
