using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Xml;


public enum BlockID
{
	None = 0,
	Dirt,
	Stone,
	Grass,
	Tree,
	Brick,
	BrickFloor,
	Post,
	Anvil,
	Chest,
	QuestBoard,
    VillageDoor,
	GoblinDoor,
	RedBrick,
	RedBrickFloor,
	Scaffold,
	Bush,
	Wheat,
	Twigs,
	Cloth,
	Spikes
}


public struct BlockMeta
{
	public int TextureID;
	public int ObjectID;

	public bool IsRefBlock { get { return ObjectID != -1; } }

	public bool Destructable;
	public int Health;
	public bool Solid;

	public bool FlipX;
	public bool FlipY;

	public ToolID DestroyTool;
	public ItemID DroppedItem;
}


public class Destroyable
{
	private float MaxHealth;
    public float Health { get; private set; }
	public float NormalizedHealth { get { return Health / MaxHealth; } }

	private float HitCoolDown;
	private float ResetTime;

	public Person LastPerson { get; private set; }
	public delegate void ChangeDelegate();

	public ChangeDelegate OnHealthChange;
	public ChangeDelegate OnKilled;

	public Destroyable(float MaxHealth, ChangeDelegate OnHealthChange, ChangeDelegate OnKilled)
	{
		this.MaxHealth = MaxHealth;
        Health = MaxHealth;
		HitCoolDown = 0.0f;
		ResetTime = 0.0f;

		this.OnHealthChange = OnHealthChange;
		this.OnKilled = OnKilled;
	}

	public void Update(float deltaTime)
	{
		if (Health <= 0.0f)
			return;
		
		if (HitCoolDown > 0.0f)
			HitCoolDown -= deltaTime;
		
		if (ResetTime > 0.0f)
			ResetTime -= deltaTime;
		
		else if (Health != MaxHealth)
		{
			Health += 0.25f * deltaTime;

			if (Health >= MaxHealth)
				Health = MaxHealth;

			OnHealthChange();
		}
    }

	public bool AttemptDamage(float amount, Person who)
	{
		if (Health <= 0.0f || HitCoolDown > 0.0f)
			return false;

		LastPerson = who;
		//HitCoolDown = 0.5f;
		ResetTime = 2.5f;
		Health -= amount;
		OnHealthChange();


		if (Health <= 0.0f)
			OnKilled();
		
		return true;
	}
}


public class Block : MonoBehaviour
{
	public static Dictionary<BlockID, BlockMeta> Library { get; private set; }

	protected BlockMeta mMeta;
	protected SpriteRenderer mSprite;

	protected bool MouseIsOver = false;
	protected Destroyable destroyable;

	[System.NonSerialized]
	public BlockID id;
	[System.NonSerialized]
	public int x;
	[System.NonSerialized]
	public int y;

	[System.NonSerialized]
	public ReferenceObject RefObject;


	public virtual void WorldInit(WorldController worldController)
	{
		mSprite = GetComponentInChildren<SpriteRenderer>();
		mMeta = Library[id];

		if (!mMeta.IsRefBlock)
		{
			mSprite.sprite = WorldController.Main.TileSheet[mMeta.TextureID];

			if(mMeta.Destructable)
				destroyable = new Destroyable((float)mMeta.Health/100.0f, OnHealthChange, OnKilled);
        }
		else
			mSprite.enabled = false;

		if (mMeta.FlipX)
			mSprite.flipX = Random.value >= 0.5f;
		if (mMeta.FlipY)
			mSprite.flipY = Random.value >= 0.5f;
		
		GetComponent<Collider2D>().enabled = mMeta.Solid;
	}

	public static void LibInit(WorldController worldController)
	{
		Library = new Dictionary<BlockID, BlockMeta>();

        XmlDocument blocksDoc = new XmlDocument();
		blocksDoc.Load("Assets/blocks.xml");

		foreach (XmlNode node in blocksDoc.DocumentElement.ChildNodes)
		{
			BlockID id = (BlockID)XML.GetInt(node.Attributes["ID"]);

			BlockMeta meta = new BlockMeta();

			meta.TextureID = XML.GetInt(node.Attributes["TextureID"], -1);
			meta.ObjectID = XML.GetInt(node.Attributes["ObjectID"], -1);

			meta.Health = XML.GetInt(node.Attributes["Health"], 100);
			meta.Destructable = XML.GetBool(node.Attributes["Destructable"]);
			meta.Solid = XML.GetBool(node.Attributes["Solid"]);

			meta.FlipX = XML.GetBool(node.Attributes["FlipX"]);
			meta.FlipY = XML.GetBool(node.Attributes["FlipY"]);
			
            meta.DestroyTool = (ToolID)XML.GetUInt(node.Attributes["DestroyTool"]);
			meta.DroppedItem = (ItemID)XML.GetUInt(node.Attributes["DroppedItem"]);
			
			Library[id] = meta;
		}

		Debug.Log("Loaded " + Library.Count + " block meta");
    }
	
	protected virtual void Update()
	{
		if (destroyable != null)
			destroyable.Update(Time.deltaTime);
    }

	public bool AttemptHit(ItemID item, bool overrideItem = false)
	{
		if (mMeta.DestroyTool != ToolID.None && (item == ItemID.None || ItemController.Library[item].ToolType != mMeta.DestroyTool))
			return false;

		float damage = item != ItemID.None ? ItemController.Library[item].Damage : 0.10f;

		if (RefObject != null)
		{
			if (RefObject.destroyable != null)
				return RefObject.destroyable.AttemptDamage(damage, PlayerInput.Main.mPerson);
		}
		else if (destroyable != null)
			return destroyable.AttemptDamage(damage, PlayerInput.Main.mPerson);

		return false;
	}
	
	void OnHealthChange()
	{
		float Health = destroyable.NormalizedHealth;

		mSprite.color = Color.white * Health + Color.gray * (1.0f - Health);
		//mSprite.transform.rotation = Quaternion.AngleAxis(90.0f * (1.0f - Health), Vector3.forward);
		mSprite.transform.localScale = Vector3.one * Health + new Vector3(0.7f, 0.7f) * (1.0f - Health);
	}

	void OnKilled()
	{
		gameObject.SetActive(false);
		WorldController.Main.RemoveBlock(x, y);

		if (mMeta.DroppedItem != ItemID.None)
			destroyable.LastPerson.GiveItem(mMeta.DroppedItem);
	}
}
