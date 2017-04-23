using System.Collections;
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


	private Vector2 lastLocation;
	private Vector2 desiredInput;

	private float tickTime;
	public const float DeltaTime = 1.0f / 5.0f;


	void Start()
	{
		mPerson = GetComponent<Person>();
		mPerson.Equip(PreferredWeapon);
		mPerson.HotBar[1].SetID(ItemID.Hammer);
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
		if (StuckTime >= NerdPoolWait)
		{
			mPerson.Equip(1);
			mPerson.mAnimator.Swing(ItemController.Library[mPerson.CurrentlyEquiped.ID].SwingTime, OnFinishBuild);
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

				//Try jump, if cannot get there
				if (StuckTime != 0.0f && StuckTime <= NerdPoolWait)
					desiredInput.y = 1;

				//Attempt to nerdpool
				else
				{
					desiredInput.y = 0;
					if (StuckTime >= NerdPoolWait + NerdPoolBuildTime && mPerson.TouchingGround)
					{
						WorldController.Main.Place(BlockID.Scaffold, mPerson.WorldX, mPerson.WorldY);
						StuckTime = 0.0f;
					}
				}
			}
			else
				StuckTime = 0.0f;
			
        }
	}
}
