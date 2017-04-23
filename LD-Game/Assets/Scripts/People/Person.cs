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


	public float Health = 1.0f;
	private float DeadCoolDown = 0.0f;
	private float MaxHealth;
	public float HealRate = 0.02f;


	public float NormalizedHealth { get { return Mathf.Clamp(Health/ MaxHealth, 0.0f, 1.0f); } }
	public WeaponSlot mWeaponSlot;

	public ItemSlot[] HotBar;
	public ItemSlot CurrentlyEquiped;

	public bool IsDead { get { return Health <= 0.0f; } }


	void Start ()
	{
		PlayerInput playerInput = GetComponent<PlayerInput>();
		IsPlayer = (playerInput != null);

		if (Health <= 0.0f)
			Health = 0.01f;
		MaxHealth = Health;

		Body = GetComponent<Rigidbody2D>();
		mAnimator = GetComponentInChildren<PersonAnimator>();
    }

	public void AddInput(Vector2 input)
	{
		InputVector += input;
    }

	public bool Attack(ItemID what, Person who)
	{
		if (IsPlayer == who.IsPlayer)
			return false;

		float damage = ItemController.Library.ContainsKey(what) ? ItemController.Library[what].Damage : 0.2f;

		if (IsPlayer)
			damage *= 0.1f;

		Health -= damage;
		if (Health <= 0)
		{
			Health = 0;

			//Revive player
			if (IsPlayer)
				DeadCoolDown = 5.0f;
			else
				Destroy(gameObject);
        }

		Vector2 hitDirection = who.transform.position - transform.position;
		Vector2 knockback = new Vector2(-Mathf.Sign(hitDirection.x) * 20.0f, 10.0f);
        Body.velocity += knockback;


		return true;
    }

	void Update()
	{
		UpdateMovement();

		//Dead player, so revive
		if (IsPlayer && IsDead)
		{
			DeadCoolDown -= Time.deltaTime;

			if (DeadCoolDown <= 0.0f)
				Health = 0.4f;
		}
		else
		{
			Health += HealRate * Time.deltaTime;
			Health = Mathf.Clamp(Health, 0.0f, MaxHealth);
		}
	}
	
	void UpdateMovement()
	{
		if (IsDead)
			InputVector = Vector2.zero;

		if (InputVector.x != 0.0f)
			mAnimator.transform.localScale = new Vector2(Mathf.Sign(InputVector.x), 1.0f);

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
				if (barSlot != null)
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
