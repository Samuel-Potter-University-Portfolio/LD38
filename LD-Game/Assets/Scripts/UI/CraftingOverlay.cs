using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using UnityEngine.UI;


public class CraftingOverlay : MonoBehaviour {

	public static CraftingOverlay Main { get; private set; }

	[SerializeField]
	private Image CraftingImage;
	[SerializeField]
	private RawImage CraftingBackground;
	[SerializeField]
	private Text CraftingImageText;
	[SerializeField]
	private Text ProductivityText;

	private CraftingRecipe[] RecipeSlots;
	
	void Start ()
	{
		Main = this;
		gameObject.SetActive(false);

		CraftingImageText.enabled = false;
		CraftingImage.enabled = false;
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

		ProductivityText.text = Mathf.RoundToInt(VillageDoor.Main.Productivity * 100) + "% Productivity";
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

	public void OnBeginCraft(ItemID Item)
	{
		int textureID = ItemController.Library[Item].TextureID;
		CraftingImage.sprite = ItemController.Main.ItemSheet[textureID];
		CraftingImage.enabled = true;
		CraftingImageText.enabled = true;
	}

	public void OnCraftUpdate(float RemainingTime, float TotalTime)
	{
		float v = 1.0f - RemainingTime / TotalTime;

		CraftingImageText.text = Mathf.RoundToInt(v * 100) + "%";

		if (v < 0.5f)
		{
			float vn = v / 0.5f;
			CraftingBackground.color = Color.red * (1.0f - vn) + Color.yellow * vn;
		}
		else
		{
			float vn = (v - 0.5f) / 0.5f;
			CraftingBackground.color = Color.yellow * (1.0f - vn) + Color.green * vn;
		}
    }

	public void OnCraftComplete()
	{
		CraftingImageText.enabled = false;
        CraftingImage.enabled = false;
		Redraw();
    }
}
