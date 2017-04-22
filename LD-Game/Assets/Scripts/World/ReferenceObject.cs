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
	public BlockMeta mMeta;

	protected List<Block> Blocks = new List<Block>();
	private Vector2 worldPosition;

	[SerializeField]
	protected List<Point> Signature;
	public Destroyable destroyable { get; private set; }


	public void PlaceInWorld(int x, int y)
	{
		mMeta = Block.Library[ID];

		if(mMeta.Destructable)
			destroyable = new Destroyable((float)mMeta.Health / 100.0f, OnHealthChange, OnKilled);

		transform.localPosition = new Vector3(x, y) * WorldController.BLOCK_SIZE;
		worldPosition = transform.localPosition;

		foreach (Point point in Signature)
		{
			Block block = WorldController.Main.SpawnBlock(ID, x + point.x, y + point.y);
			block.RefObject = this;
			Blocks.Add(block);
        }
    }

	protected virtual void Update()
	{
		if(destroyable != null)
			destroyable.Update(Time.deltaTime);
	}

	public virtual void OnKilled()
	{
		foreach (Block block in Blocks)
			WorldController.Main.RemoveBlock(block.x, block.y);

		Destroy(gameObject);

		if (mMeta.DroppedItem != ItemID.None)
			destroyable.LastPerson.GiveItem(mMeta.DroppedItem);
	}

	public virtual void OnHealthChange()
	{
		transform.position = worldPosition + new Vector2(Mathf.Sin(destroyable.NormalizedHealth * Mathf.PI*2.0f), 0) * 0.25f;
    }
}
