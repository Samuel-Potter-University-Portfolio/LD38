using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Point
{
	public int x;
	public int y;
}

public class ReferenceObject : MonoBehaviour
{
	public BlockID ID;
	public float MaxHealth = 1.0f;
	protected List<Block> Blocks = new List<Block>();
	[SerializeField]
	protected List<Point> Signature;
	public Destroyable destroyable { get; private set; }


	public void PlaceInWorld(int x, int y)
	{
		destroyable = new Destroyable(MaxHealth, OnHealthChange, OnKilled);

		transform.localPosition = new Vector3(x, y) * WorldController.BLOCK_SIZE;

		foreach (Point point in Signature)
		{
			Block block = WorldController.Main.SpawnBlock(ID, x + point.x, y + point.y);
			block.RefObject = this;
			Blocks.Add(block);
        }
    }

	protected virtual void Update()
	{
		destroyable.Update(Time.deltaTime);
	}

	public virtual void OnKilled()
	{
		foreach (Block block in Blocks)
			WorldController.Main.RemoveBlock(block.x, block.y);

		Destroy(gameObject);
	}

	public virtual void OnHealthChange()
	{
	}
}
