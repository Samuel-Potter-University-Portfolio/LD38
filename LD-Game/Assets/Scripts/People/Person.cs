using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Person : MonoBehaviour
{

	//Movement
	public float Acceleration = 20.0f;
	public float Decceleration = 20.0f;
	public float MaxSpeed = 20.0f;
	public float JumpAcceleration = 20.0f;
	public bool IsPlayer { get; private set; }

	public PersonAnimator mAnimator { get; private set; }
	private Rigidbody2D Body;
	private Vector2 InputVector = new Vector2();

	private List<Collider2D> floors = new List<Collider2D>();
	public bool TouchingGround { get; private set; }
	public bool InJump { get; private set; }

	public int WorldX { get { return Mathf.RoundToInt(transform.position.x / WorldController.BLOCK_SIZE); } }
	public int WorldY { get { return Mathf.RoundToInt(transform.position.y / WorldController.BLOCK_SIZE); } }

	//Resourses
	public ResourceBar HungerBar;
	public ResourceBar ThirstBar;
	public ResourceBar StaminaBar;
	public ResourceBar HealthBar;

	public ItemSlot[] HotBar;
	public ItemSlot CurrentlyEquiped;


	void Start ()
	{
		PlayerInput playerInput = GetComponent<PlayerInput>();
		IsPlayer = (playerInput != null);
		
		Body = GetComponent<Rigidbody2D>();
		mAnimator = GetComponentInChildren<PersonAnimator>();

		if (IsPlayer)
		{
			HungerBar = new ResourceBar(0, 100, 0.3f);
			ThirstBar = new ResourceBar(0, 100, 0.1f);
			StaminaBar = new ResourceBar(0, 100, 0.04f);
			HealthBar = new ResourceBar(0, 100, 0);
		}
	}

	public void AddInput(Vector2 input)
	{
		InputVector += input;
    }

	void Update()
	{
		UpdateMovement();

		if (IsPlayer)
		{
			float deltaTime = Time.deltaTime;
			HungerBar.Update(deltaTime);
			ThirstBar.Update(deltaTime);
			StaminaBar.Update(deltaTime);
			HealthBar.Update(deltaTime);
		}
	}
	
	void UpdateMovement()
	{
		//Handle new input
		if (InputVector.sqrMagnitude > 0.01f)
		{
			Vector2 movement = InputVector * Acceleration * Time.deltaTime;
			InputVector = Vector2.zero;

			if (movement.y > 0 && TouchingGround && !InJump)
			{
				movement.y = JumpAcceleration;
				TouchingGround = false;
				InJump = true;
            }
			else
				movement.y = 0;
			Body.velocity += movement;
		}
		//No input, so slow down
		else
		{
			float actualDecceleration = Decceleration * Time.deltaTime;

			if (Mathf.Abs(Body.velocity.x) <= actualDecceleration)
				Body.velocity = new Vector2(0, Body.velocity.y);
			else
				Body.velocity = new Vector2(Body.velocity.x - actualDecceleration * Mathf.Sign(Body.velocity.x), Body.velocity.y);
        }
		
		//Cap max speed
		if (Mathf.Abs(Body.velocity.x) > MaxSpeed)
			Body.velocity = new Vector2(MaxSpeed * Mathf.Sign(Body.velocity.x), Body.velocity.y);

		//Track jump
		if (InJump && Body.velocity.y <= 0)
			InJump = false;
	}

	void OnCollisionEnter2D(Collision2D collision)
	{
		if (collision.contacts[0].point.y < transform.position.y - 1.5f)
		{
			floors.Add(collision.collider);
			TouchingGround = true;
        }
		
    }

	void OnCollisionExit2D(Collision2D collision)
	{
		if (floors.Contains(collision.collider))
		{
			floors.Remove(collision.collider);

			if (TouchingGround)
				TouchingGround = floors.Count != 0;
		}
	}

	public bool GiveItem(ItemID ID)
	{
		foreach (ItemSlot slot in HotBar)
		{
			if (slot.ID == ItemID.None)
			{
				slot.SetID(ID);
				return true;
			}
		}
		
		return false;
	}

	public void Equip(ItemSlot slot)
	{
		if(slot == null || slot.ID == ItemID.None)
			CurrentlyEquiped = null;
		else
			CurrentlyEquiped = slot;


		//Update hotbar colours
		if (IsPlayer)
		{
			foreach (ItemSlot barSlot in HotBar)
			{
				if (barSlot != null && barSlot.ID != ItemID.None)
					barSlot.UpdateColour(barSlot == CurrentlyEquiped);
			}
		}
	}

	public void Equip(int index)
	{
		ItemSlot slot = HotBar[index];

		if (slot == null || slot.ID == ItemID.None)
			CurrentlyEquiped = null;
		else
			CurrentlyEquiped = slot;


		//Update hotbar colours
		if (IsPlayer)
		{
			foreach (ItemSlot barSlot in HotBar)
			{
				if (barSlot != null && barSlot.ID != ItemID.None)
					barSlot.UpdateColour(barSlot == CurrentlyEquiped);
			}
		}
	}
}
