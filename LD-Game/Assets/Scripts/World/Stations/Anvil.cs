using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Anvil : MonoBehaviour {

	public void Open()
	{
        PlayerInput.Main.mCraftingOverlay.Open();
	}
}
