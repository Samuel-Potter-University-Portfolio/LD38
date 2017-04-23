using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIInput : MonoBehaviour
{
	public Person mPerson { get; private set; }
	public int PreferredWeapon = 2;


	[SerializeField]
	protected float FocusPlayerRange = 15.0f;
	[SerializeField]
	protected float AttackPlayerRange = 4.0f;

	private float StuckTime;
	protected const float NerdPoolWait = 2.0f;
	[SerializeField]
	protected float NerdPoolBuildTime = 2.0f;
	public bool CanNerdPool = true;

	protected const float DigWait = 0.5f;
	public bool CanDig = true;
	protected bool IsDigging;
	protected Block DigTarget;


	private Vector2 lastLocation;
	private Vector2 desiredInput;

	private float tickTime;
	public const float DeltaTime = 1.0f / 10.0f;

	public ItemID[] Inventory;

	protected virtual void Start()
	{
		mPerson = GetComponent<Person>();

		for (int i = 0; i< Inventory.Length; ++i)
			mPerson.HotBar[i].SetID(Inventory[i]);
		
		mPerson.HotBar[3].SetID(ItemID.Pickaxe);
		mPerson.HotBar[4].SetID(ItemID.Hammer);
		mPerson.Equip(PreferredWeapon);
	}

	void Update()
	{
		tickTime -= Time.deltaTime;

		//Run update every 5th of a second
		if (tickTime <= 0)
		{
			RunUpdate();
			tickTime += DeltaTime;
		}

		mPerson.AddInput(desiredInput);
    }

	protected virtual void RunUpdate()
	{
		UpdateDesiredInput();
		UpdateAttack();
		lastLocation = transform.position;
	}

	void UpdateAttack()
	{
		if (IsDigging)
		{
			mPerson.Equip(3);
			mPerson.mAnimator.Swing(ItemController.Library[mPerson.CurrentlyEquiped.ID].SwingTime, OnFinishDig);
			return;
		}
		else if (CanNerdPool && StuckTime >= NerdPoolWait)
		{
			mPerson.Equip(4);
			mPerson.mAnimator.Swing(ItemController.Library[mPerson.CurrentlyEquiped.ID].SwingTime, OnFinishBuild);
			return;
		}
		else
			mPerson.Equip(PreferredWeapon);


		PlayerInput player = PlayerInput.Main;

		if (player == null)
			return;

		Vector2 playerDif = player.transform.position - transform.position;

		//Attack player
		if (playerDif.sqrMagnitude <= AttackPlayerRange * AttackPlayerRange)
		{
			if (mPerson.CurrentlyEquiped != null && mPerson.CurrentlyEquiped.ID != ItemID.None)
				mPerson.mAnimator.Swing(ItemController.Library[mPerson.CurrentlyEquiped.ID].SwingTime, OnFinishSwing);
			else
				mPerson.mAnimator.Swing(0.20f, OnFinishSwing);
		}
    }

	void OnFinishSwing()
	{
		ItemID item = mPerson.CurrentlyEquiped != null ? mPerson.CurrentlyEquiped.ID : ItemID.None;
        mPerson.mWeaponSlot.Use(item, mPerson);
	}

	void OnFinishBuild()
	{
	}

	void OnFinishDig()
	{
		DigTarget.AttemptHit(ItemID.Pickaxe, true);
		DigTarget = null;

		IsDigging = false;
		StuckTime = 0.0f;
	}

	void UpdateDesiredInput()
	{
		Vector2 Accuracy = new Vector2(2.0f, 0.1f);
		desiredInput = Vector2.zero;
		VillageDoor village = VillageDoor.Main;
		PlayerInput player = PlayerInput.Main;


		if (village == null || player == null)
			return;


		Vector2 difference;
		Vector2 villageDif = village.transform.position - transform.position;
		Vector2 playerDif = player.transform.position - transform.position;


		if (!player.mPerson.IsDead && playerDif.sqrMagnitude <= FocusPlayerRange * FocusPlayerRange)
			difference = playerDif;
		else
			difference = villageDif;


		//On spot
		if (Mathf.Abs(difference.x) < Accuracy.x && Mathf.Abs(difference.y) < Accuracy.y)
            return;


		Vector2 input = new Vector2(Mathf.Abs(difference.x) >= Accuracy.x ? (int)Mathf.Sign(difference.x) * Mathf.Clamp(Mathf.Abs(difference.x), 0.0f, 1.0f) : 0, Mathf.Abs(difference.y) >= Accuracy.y ? (int)Mathf.Sign(difference.y) : 0);
		desiredInput = input;
		

		//If in same position, check if stuck
		if (transform.position.x >= lastLocation.x - Accuracy.x * DeltaTime && transform.position.x <= lastLocation.x + Accuracy.x * DeltaTime)
		{
			StuckTime += DeltaTime;


			//Above
			if (input.y > 0.0f)
			{
				Block BlockAboveHead = WorldController.Main.GetBlock(mPerson.WorldX, mPerson.WorldY + 2);

				//Attempt to nerdpool
				if (CanNerdPool && BlockAboveHead == null && StuckTime >= NerdPoolWait)
				{
					desiredInput.y = 0;
					if (StuckTime >= NerdPoolWait + NerdPoolBuildTime && mPerson.TouchingGround)
					{
						WorldController.Main.Place(BlockID.Cloth, mPerson.WorldX, mPerson.WorldY);
						StuckTime = 0.0f;
					}
				}

				//Attemp to dig
				else if (CanDig && BlockAboveHead != null && StuckTime >= DigWait)
				{
					IsDigging = true;
					DigTarget = BlockAboveHead;
				}

				//Try jump, if cannot get there
				else
					desiredInput.y = 1;
			}

			//Below
			else if (input.y < 0.0f)
			{
				Block BlockBellow = WorldController.Main.GetBlock(mPerson.WorldX, mPerson.WorldY - 1);

				//Attemp to dig
				if (CanDig && BlockBellow != null && StuckTime >= DigWait)
				{
					IsDigging = true;
					DigTarget = BlockBellow;
				}

				//Try jump, if cannot get there
				else if(StuckTime >= 0.5f)
					desiredInput.y = 1;
			}

			//Above
			else
			{
				Block BottomBlock = WorldController.Main.GetBlock(mPerson.WorldX + Mathf.CeilToInt(input.x), mPerson.WorldY + 1);
				Block TopBlock = WorldController.Main.GetBlock(mPerson.WorldX + Mathf.CeilToInt(input.x), mPerson.WorldY);

				//Attemp to dig
				if (CanDig && (BottomBlock != null || TopBlock != null) && StuckTime >= DigWait)
				{
					IsDigging = true;
					DigTarget = TopBlock == null ? BottomBlock : TopBlock;
				}

				//Try jump, if cannot get there
				else if (StuckTime >= 0.5f)
					desiredInput.y = 1;
			}
		}
		else
			StuckTime = 0.0f;
	}
}
