﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour {

	public void Open()
	{
		PlayerInput.Main.mChestOverlay.Open();
	}
}
