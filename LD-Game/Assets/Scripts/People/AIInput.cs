﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIInput : MonoBehaviour
{
	public Person mPerson { get; private set; }
	public int PreferredWeapon;


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
	public const float DeltaTime = 1.0f / 5.0f;


	protected virtual void Start()
	{
		mPerson = GetComponent<Person>();
		mPerson.Equip(PreferredWeapon);
		mPerson.HotBar[0].SetID(ItemID.Pickaxe);
		mPerson.HotBar[1].SetID(ItemID.Hammer);
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
		//Building Nerd poll
		if (CanNerdPool && StuckTime >= NerdPoolWait)
		{
			mPerson.Equip(0);
			mPerson.mAnimator.Swing(ItemController.Library[mPerson.CurrentlyEquiped.ID].SwingTime, OnFinishBuild);
			return;
		}
		else if (IsDigging)
		{
			mPerson.Equip(1);
			mPerson.mAnimator.Swing(ItemController.Library[mPerson.CurrentlyEquiped.ID].SwingTime, OnFinishDig);
			return;
		}
		else
			mPerson.Equip(PreferredWeapon);


		PlayerInput player = PlayerInput.Main;

		if (player == null)
			return;

		Vector2 playerDif = player.transform.position - transform.position;
		
		if (playerDif.sqrMagnitude <= AttackPlayerRange * AttackPlayerRange)
			mPerson.mAnimator.Swing(ItemController.Library[mPerson.CurrentlyEquiped.ID].SwingTime, OnFinishSwing);
    }

	void OnFinishSwing()
	{
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
		const float Accuracy = 1.5f;
		desiredInput = Vector2.zero;
		VillageDoor village = VillageDoor.Main;
		PlayerInput player = PlayerInput.Main;


		if (village == null || player == null)
			return;


		Vector2 difference;
		Vector2 villageDif = village.transform.position - transform.position;
		Vector2 playerDif = player.transform.position - transform.position;


		if (playerDif.sqrMagnitude <= FocusPlayerRange * FocusPlayerRange)
			difference = playerDif;
		else
			difference = villageDif;


		if (difference.sqrMagnitude >= Accuracy * Accuracy)
		{
			Vector2 input = new Vector2(Mathf.Sign(difference.x), difference.y >= Accuracy ? Mathf.Sign(difference.y) : 0);
			desiredInput = input;


			//If in same position, check if stuck
			if (transform.position.x >= lastLocation.x - Accuracy * DeltaTime && transform.position.x <= lastLocation.x + Accuracy * DeltaTime)
			{
				StuckTime += DeltaTime;

				Block BlockAboveHead = WorldController.Main.GetBlock(mPerson.WorldX, mPerson.WorldY + 2);
				

				//Attempt to nerdpool
				if (CanNerdPool && BlockAboveHead == null && StuckTime >= NerdPoolWait)
				{
					desiredInput.y = 0;
					if (StuckTime >= NerdPoolWait + NerdPoolBuildTime && mPerson.TouchingGround)
					{
						WorldController.Main.Place(BlockID.Scaffold, mPerson.WorldX, mPerson.WorldY);
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
			else
				StuckTime = 0.0f;
			
        }
	}
}
