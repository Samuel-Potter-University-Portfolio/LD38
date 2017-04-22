using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Xml;


public enum BlockID
{
	Dirt = 0,
	Stone,
	Grass,
	Tree
}


public struct BlockMeta
{
	public int TextureID;
	public int ObjectID;

	public bool IsRefBlock { get { return ObjectID != -1; } }

	public bool Destructable;
	public bool Solid;

	public bool FlipX;
	public bool FlipY;

	public uint DestroyTool;
	public uint DroppedItem;
	public uint DroppedAmount;
}


public class Destroyable
{
	private float MaxHealth;
    public float Health { get; private set; }
	public float NormalizedHealth { get { return Health / MaxHealth; } }

	private float HitCoolDown;
	private float ResetTime;
	
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
			Health = MaxHealth;
			OnHealthChange();
		}
    }

	public bool AttemptDamage(float amount)
	{
		if (Health <= 0.0f || HitCoolDown > 0.0f)
			return false;
		

		HitCoolDown = 0.5f;
		ResetTime = 1.0f;
		Health -= amount;
		OnHealthChange();


		if (Health < 0.0f)
			OnKilled();
		
		return true;
	}
}


public class Block : MonoBehaviour
{
	public static Dictionary<BlockID, BlockMeta> Library { get; private set; }

	private BlockMeta mMeta;
	private SpriteRenderer mSprite;

	private bool MouseIsOver = false;
	private Destroyable destroyable;

	[System.NonSerialized]
	public BlockID id;
	[System.NonSerialized]
	public int x;
	[System.NonSerialized]
	public int y;

	[System.NonSerialized]
	public ReferenceObject RefObject;


	public void WorldInit(WorldController worldController)
	{
		mSprite = GetComponentInChildren<SpriteRenderer>();
		mMeta = Library[id];

		if (!mMeta.IsRefBlock)
		{
			mSprite.sprite = WorldController.Main.TileSheet[mMeta.TextureID];
			destroyable = new Destroyable(1.0f, OnHealthChange, OnKilled);
        }
		else
			mSprite.enabled = false;

		if (mMeta.FlipX)
			mSprite.flipX = Random.value >= 0.5f;
		if (mMeta.FlipY)
			mSprite.flipY = Random.value >= 0.5f;
		
		GetComponent<Collider2D>().isTrigger = !mMeta.Solid;
	}

	public static void LibInit(WorldController worldController)
	{
		Library = new Dictionary<BlockID, BlockMeta>();

        XmlDocument blocksDoc = new XmlDocument();
		blocksDoc.Load("Assets/blocks.xml");

		foreach (XmlNode node in blocksDoc.DocumentElement.ChildNodes)
		{
			BlockID id = (BlockID)GetInt(node.Attributes["ID"]);

			BlockMeta meta = new BlockMeta();

			meta.TextureID = GetInt(node.Attributes["TextureID"], -1);
			meta.ObjectID = GetInt(node.Attributes["ObjectID"], -1);

			meta.Destructable = GetBool(node.Attributes["Destructable"]);
			meta.Solid = GetBool(node.Attributes["Solid"]);

			meta.FlipX = GetBool(node.Attributes["FlipX"]);
			meta.FlipY = GetBool(node.Attributes["FlipY"]);
			
            meta.DestroyTool = GetUInt(node.Attributes["DestroyTool"]);
			meta.DroppedItem = GetUInt(node.Attributes["DroppedItem"]);
			meta.DroppedAmount = GetUInt(node.Attributes["DroppedAmount"]);
			
			Library[id] = meta;
		}

		Debug.Log("Loaded " + Library.Count + " block meta");
    }

	private static int GetInt(XmlAttribute attrib, int defaultValue = 0)
	{
		if (attrib == null)
			return defaultValue;
		else
			return System.Int32.Parse(attrib.Value);
	}

	private static uint GetUInt(XmlAttribute attrib, uint defaultValue = 0)
	{
		if (attrib == null)
			return defaultValue;
		else
			return System.UInt32.Parse(attrib.Value);
	}

	private static bool GetBool(XmlAttribute attrib, bool defaultValue = false)
	{
		if (attrib == null)
			return defaultValue;
		else
			return attrib.Value.ToLower().Equals("true");
	}

	void Update()
	{
		if (destroyable != null)
			destroyable.Update(Time.deltaTime);

		if (MouseIsOver)
		{
			const float damage = 0.34f;

			//Destroy block
			if (Input.GetMouseButton(0) && Vector2.Distance(transform.position, PlayerInput.Main.transform.position) < PlayerInput.Main.InteractRange)
				if (RefObject != null)
					RefObject.destroyable.AttemptDamage(damage);
				else
					destroyable.AttemptDamage(damage);
		}
    }

	void OnMouseEnter()
	{
		MouseIsOver = true;
    }

	void OnMouseExit()
	{
		MouseIsOver = false;
    }

	void OnHealthChange()
	{
		float Health = destroyable.NormalizedHealth;
		mSprite.transform.rotation = Quaternion.AngleAxis(90.0f * (1.0f - Health), Vector3.forward);
		mSprite.transform.localScale = Vector3.one * Health;
	}

	void OnKilled()
	{
		gameObject.SetActive(false);
		WorldController.Main.RemoveBlock(x, y);
	}
}
