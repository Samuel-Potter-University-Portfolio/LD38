using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Background : Block
{
	public override void WorldInit(WorldController worldController)
	{
		mSprite = GetComponentInChildren<SpriteRenderer>();
		mMeta = Library[id];

		if (!mMeta.IsRefBlock)
			mSprite.sprite = WorldController.Main.TileSheet[mMeta.TextureID];
		else
			mSprite.enabled = false;

		if (mMeta.FlipX)
			mSprite.flipX = Random.value >= 0.5f;
		if (mMeta.FlipY)
			mSprite.flipY = Random.value >= 0.5f;
	}

	protected override void Update()
	{

	}
}
