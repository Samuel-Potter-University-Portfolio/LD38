using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class WorldController : MonoBehaviour {

	public static WorldController Main { get; private set; }

	[SerializeField]
	private Person BasePerson;

	[SerializeField]
	private Block BaseBlock;
	public Sprite[] TileSheet;
	public ReferenceObject[] ObjectSheet;

	public static int WORLD_WIDTH { get { return 512; } }
	public static int WORLD_HEIGHT { get { return 64; } }
	public static float BLOCK_SIZE { get { return 2.0f; } }

	private Block[,] Blocks;
	private bool WorldInit = false;


	void Start ()
	{
		Main = this;
		Block.LibInit(this);

		Blocks = new Block[WORLD_WIDTH, WORLD_HEIGHT];
		GenerateWorld();
		SpawnPlayers();
    }

	void GenerateWorld()
	{
		//Generate heights
		const float baseHeight = 40.0f;
		const float deviation = 5.0f;
		const int Resolution = 10;
		float[] heights = new float[Resolution];

		heights[0] = baseHeight + (Random.value * 2.0f - 1.0f) * deviation;

		for (int i = 1; i < Resolution; i++)
			heights[i] = heights[i-1] + (Random.value * 2.0f - 1.0f) * deviation;
		

		//Build world
		for (int x = 0; x < WORLD_WIDTH; ++x)
		{
			float v = (float)x / (float)(WORLD_WIDTH - 1);

			int i0 = Mathf.FloorToInt(v * (Resolution - 2));
			int i1 = Mathf.FloorToInt(v * (Resolution - 2)) + 1;

			float l = (float)(x % (Resolution - 1)) / (float)(Resolution - 1);

			float height = heights[i0] * (1.0f - l) + heights[i1] * l;
			int currentHeight = Mathf.FloorToInt(height); 

			if (currentHeight >= WORLD_HEIGHT - 1)
				currentHeight = (int)WORLD_HEIGHT - 1;

			for (int y = 0; y <= currentHeight; ++y)
			{
				if (y == currentHeight)
					SpawnBlock(BlockID.Grass, x, y);
				else if (y >= currentHeight - 4.0f)
					SpawnBlock(BlockID.Dirt, x, y);
				else
					SpawnBlock(BlockID.Stone, x, y);

			}

        }


		WorldInit = true;


		for (uint x = 0; x < WORLD_WIDTH; ++x)
			for (uint y = 0; y < WORLD_HEIGHT; ++y)
				if(Blocks[x, y] != null)
					Blocks[x, y].WorldInit(this);
	}

	public void Place(BlockID id, int x, int y)
	{
		if (!Block.Library[id].IsRefBlock)
			SpawnBlock(id, x, y);
		else
		{
			foreach (ReferenceObject obj in ObjectSheet)
				if (obj.ID == id)
				{
					ReferenceObject NewObject = Instantiate(obj, transform);
					NewObject.PlaceInWorld(x, y);
					return;
				}
		}
    }

	public Block SpawnBlock(BlockID id, int x, int y)
	{
		if (Blocks[x, y] != null)
			RemoveBlock(x, y);

		Block block = Instantiate(BaseBlock, transform);
		block.gameObject.transform.localPosition = new Vector3(x * BLOCK_SIZE, y * BLOCK_SIZE, 0);
		Blocks[x, y] = block;

		block.id = id;
		block.x = x;
		block.y = y;

		if (WorldInit)
			block.WorldInit(this);

		return block;
    }

	public void RemoveBlock(int x, int y)
	{
		if (Blocks[x, y] != null)
			Destroy(Blocks[x, y].gameObject);
		Blocks[x, y] = null;
	}

	void SpawnPerson(bool isPlayer, int x, int y)
	{
		Debug.Log("Spawn Player (" + x + "," + y + ")[" + (isPlayer ? "Ply" : "Bot") + "]");

		Person person = Instantiate(BasePerson);
		person.IsPlayer = isPlayer;
		person.transform.position = new Vector3(x * BLOCK_SIZE, y * BLOCK_SIZE);
	}

	void SpawnPlayers()
	{
		const int playerCount = 8;

		for (int i = 0; i < playerCount; ++i)
		{
			float v = (float)i / (float)(playerCount - 1);

			int x = Mathf.FloorToInt(v * (WORLD_WIDTH - 30)) + 15;
			int y = FindHeight(x);

			SpawnPerson(i == 3, x, y);

			//Spawn nearby tree
			{
				int treeX = x - 5;
				int treeY = FindHeight(treeX);

				Place(BlockID.Tree, treeX, treeY);
            }
		}
	}

	public int FindHeight(int x)
	{
		for (int y = 0; y < WORLD_HEIGHT; ++y)
			if (Blocks[x, y] == null || Blocks[x, y].id == BlockID.None)
				return y;

		return WORLD_HEIGHT - 1;
	}

	void Update ()
	{
		
	}
}
