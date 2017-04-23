using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

[RequireComponent(typeof(Person))]
public class PlayerInput : MonoBehaviour {

	public static PlayerInput Main { get; private set; }
	public Person mPerson { get; private set; }

	public GameObject PlacePrompt;
	public float InteractRange = 6.0f;

	public ChestOverlay mChestOverlay;
	public CraftingOverlay mCraftingOverlay;
	public QuestOverlay mQuestOverlay;

	void Start ()
	{
		Main = this;
		mPerson = GetComponent<Person>();
	}
	
	void Update ()
	{
		if (mPerson.IsDead)
			return;

		UpdateMovement();


		if (mPerson.CurrentlyEquiped != null && mPerson.CurrentlyEquiped.mMeta.PlacesBlock != BlockID.None)
			PlacePrompt.SetActive(true);
		else
			PlacePrompt.SetActive(false);


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
		ItemID item = mPerson.CurrentlyEquiped != null ? mPerson.CurrentlyEquiped.ID : ItemID.None;

		Vector3 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);

		int HitX = Mathf.RoundToInt(mouse.x / WorldController.BLOCK_SIZE);
		int HitY = Mathf.RoundToInt(mouse.y / WorldController.BLOCK_SIZE);

		Block block = WorldController.Main.GetBlock(HitX, HitY);

		if (block != null)
			block.AttemptHit(item);

		mPerson.mWeaponSlot.Use(item, mPerson);
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
