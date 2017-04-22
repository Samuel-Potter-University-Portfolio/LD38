using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

[RequireComponent(typeof(Person))]
public class PlayerInput : MonoBehaviour {

	public static PlayerInput Main { get; private set; }
	public Person mPerson { get; private set; }

	[SerializeField]
	private RectTransform[] ResourceBars;

	public float InteractRange = 6.0f;
	private ItemSlot CurrentlySelected;

	void Start ()
	{
		Main = this;
		mPerson = GetComponent<Person>();

		mPerson.HungerBar.SetAnimBar(ResourceBars[0]);
		mPerson.ThirstBar.SetAnimBar(ResourceBars[1]);
		mPerson.StaminaBar.SetAnimBar(ResourceBars[2]); 
		mPerson.HealthBar.SetAnimBar(ResourceBars[3]);
	}
	
	void Update ()
	{
		UpdateMovement();

		//Place item
		if (mPerson.TouchingGround && Input.GetKeyDown(KeyCode.E) && CurrentlySelected != null && CurrentlySelected.mMeta.PlacesBlock != BlockID.None)
		{
			WorldController.Main.Place(CurrentlySelected.mMeta.PlacesBlock, Mathf.RoundToInt(transform.position.x / WorldController.BLOCK_SIZE), Mathf.RoundToInt(transform.position.y / WorldController.BLOCK_SIZE));

			CurrentlySelected.SetID(ItemID.None);
			HotbarSelected(null);
        }
    }

	void UpdateMovement()
	{
		if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
			mPerson.AddInput(Vector2.left);
		if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
			mPerson.AddInput(Vector2.right);

		if (Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
			mPerson.AddInput(Vector2.up);

	}

	public void HotbarSelected(ItemSlot slot)
	{
		if(slot == null || slot.ID == ItemID.None)
			CurrentlySelected = null;
		else
			CurrentlySelected = slot;

		foreach (ItemSlot hotbarSlot in mPerson.HotBar)
			hotbarSlot.UpdateColour(CurrentlySelected == null || CurrentlySelected.ID == ItemID.None ? false : hotbarSlot == CurrentlySelected);
    }
}
