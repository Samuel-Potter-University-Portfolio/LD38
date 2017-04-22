﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestOverlay : MonoBehaviour {

	public static bool IsOpen { get; private set; }
	private ItemSlot[] Slots;
	public Dictionary<ItemID, uint> Count { get; private set; }

	void Start ()
	{
		Slots = GetComponentsInChildren<ItemSlot>();
		gameObject.SetActive(false);
		CountInventory();
    }
	
	void Update ()
	{
		if (Input.GetKey(KeyCode.Escape))
			Close();
    }

	public void Open()
	{
		gameObject.SetActive(true);
		PlayerInput.Main.enabled = false;
		IsOpen = true;
    }

	public void Close()
	{
		gameObject.SetActive(false);
		PlayerInput.Main.enabled = true;
		IsOpen = false;
    }

	public void CountInventory()
	{
		if (Count == null)
			Count = new Dictionary<ItemID, uint>();
		else
			Count.Clear();

		foreach (ItemSlot slot in Slots)
		{
			if (slot.ID != ItemID.None)
			{
				if (Count.ContainsKey(slot.ID))
					Count[slot.ID]++;
				else
					Count[slot.ID] = 0;
			}
		}
	}

	public bool GiveItem(ItemID ID)
	{
		foreach (ItemSlot slot in Slots)
		{
			if (slot.ID == ItemID.None)
			{
				slot.SetID(ID);
				CountInventory();
                return true;
			}
		}

		CountInventory();
        return false;
	}
}
