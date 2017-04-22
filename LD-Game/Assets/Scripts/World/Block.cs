using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Block : MonoBehaviour {
	
	private WorldController WorldController;

	[System.NonSerialized]
	public uint id;
	[System.NonSerialized]
	public uint x;
	[System.NonSerialized]
	public uint y;
	
	public void WorldInit(WorldController worldController)
	{
		WorldController = worldController;
		SpriteRenderer renderer = GetComponent<SpriteRenderer>();
		renderer.sprite = WorldController.TileSheet[id];
    }
	
	void Update ()
	{
		
	}
}
