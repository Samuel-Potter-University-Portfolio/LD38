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

	public ChestOverlay mChestOverlay;
	public CraftingOverlay mCraftingOverlay;
	public QuestOverlay mQuestOverlay;

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
		if (Input.GetKeyDown(KeyCode.E))
		{
			int PlaceX = mPerson.WorldX;
			int PlaceY = mPerson.TouchingGround ? mPerson.WorldY : (int)((transform.position.y - 1.0f) / WorldController.BLOCK_SIZE);

			if (mPerson.CurrentlyEquiped != null && mPerson.CurrentlyEquiped.mMeta.PlacesBlock != BlockID.None && !WorldController.Main.HasBlock(PlaceX, PlaceY))
			{
				WorldController.Main.Place(mPerson.CurrentlyEquiped.mMeta.PlacesBlock, PlaceX, PlaceY);
				mPerson.CurrentlyEquiped.SetID(ItemID.None);
				mPerson.Equip(null);
			}
        }

		if (Input.GetMouseButton(0))
			if (mPerson.CurrentlyEquiped != null && mPerson.CurrentlyEquiped.ID != ItemID.None && mPerson.CurrentlyEquiped.mMeta.Tool)
				mPerson.mAnimator.Swing(mPerson.CurrentlyEquiped.mMeta.SwingTime, OnFinishSwing);
			else
				mPerson.mAnimator.Swing(0.20f, OnFinishSwing);
	}

	void OnFinishSwing()
	{
		if (Block.MouseOver != null)
			Block.MouseOver.AttemptHit(mPerson.CurrentlyEquiped != null ? mPerson.CurrentlyEquiped.ID : ItemID.None);
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
}
