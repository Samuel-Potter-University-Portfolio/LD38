using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class WorldController : MonoBehaviour {

	public static WorldController Main { get; private set; }

	[SerializeField]
	private Person BasePerson;

	[SerializeField]
	private Block BaseBlock;
	[SerializeField]
	private Background BackgroundBlock;
	public Sprite[] TileSheet;
	public ReferenceObject[] ObjectSheet;

	public static int WORLD_WIDTH { get { return 128; } }
	public static int WORLD_HEIGHT { get { return 32; } }
	public static float BLOCK_SIZE { get { return 2.0f; } }

	private Block[,] Blocks;
	private bool WorldInit = false;


	void Start ()
	{
		Main = this;
		Block.LibInit(this);

		Blocks = new Block[WORLD_WIDTH, WORLD_HEIGHT];
		GenerateWorld();
		SpawnPerson(true, WORLD_WIDTH/2, 10);
    }

	void GenerateWorld()
	{
		const float flatHeight = 10.0f;

		//Flat plain
		{
			for (int x = 0; x < WORLD_WIDTH; ++x)
				for (int y = 0; y < flatHeight; ++y)
				{
					if (y == flatHeight - 1)
						SpawnBlock(BlockID.Grass, x, y);
					else if (y >= flatHeight - 4)
						SpawnBlock(BlockID.Dirt, x, y);
					else
						SpawnBlock(BlockID.Stone, x, y);
				}
		}

		//Fort
		{ 
			int fortX = Mathf.FloorToInt(WORLD_WIDTH / 2.0f);
			int fortY = Mathf.FloorToInt(flatHeight);

			const int fortRadius = 6;
			const int fortHeight = 4;

			for (int y = 0; y < fortHeight; ++y)
			{
				SpawnBlock(BlockID.Brick, fortX - fortRadius, fortY + y);
				SpawnBlock(BlockID.Brick, fortX + fortRadius, fortY + y);
			}

			for (int x = 0; x <= fortRadius; ++x)
				for (int y = 0; y < fortHeight; ++y)
				{
					SpawnBackground(BlockID.Brick, fortX - x, fortY + y);
					SpawnBackground(BlockID.Brick, fortX + x, fortY + y);
				}

			SpawnBlock(BlockID.Brick, fortX - fortRadius - 1, fortY + fortHeight - 1);
			SpawnBlock(BlockID.Brick, fortX + fortRadius + 1, fortY + fortHeight - 1);
			SpawnBlock(BlockID.Brick, fortX - fortRadius - 1, fortY + fortHeight);
			SpawnBlock(BlockID.Brick, fortX + fortRadius + 1, fortY + fortHeight);

			for (int x = 0; x <= fortRadius; ++x)
			{
				SpawnBlock(BlockID.BrickFloor, fortX - x, fortY - 1);
				SpawnBlock(BlockID.BrickFloor, fortX + x, fortY - 1);
			}

			//Starting items
			Place(BlockID.VillageDoor, fortX, fortY);
			Place(BlockID.Chest, fortX - 1, fortY);
			Place(BlockID.Anvil, fortX - 3, fortY);
			Place(BlockID.QuestBoard, fortX + 2, fortY);

			//Create line of trees
			for (int i = 0; i<6; ++i)
			{
				int x0 = fortX - fortRadius - 4 - 6 * i;
				int x1 = fortX + fortRadius + 4 + 6 * i;
				Place(BlockID.Tree, x0, FindHeight(x0));
				Place(BlockID.Tree, x1, FindHeight(x1));
			}
		}

		SpawnGoblinDoor(2, (int)flatHeight);
		SpawnGoblinDoor(WORLD_WIDTH - 4, (int)flatHeight);



		//Init blocks
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

	public Background SpawnBackground(BlockID id, int x, int y)
	{
		Background block = Instantiate(BackgroundBlock, transform);
		block.gameObject.transform.localPosition = new Vector3(x * BLOCK_SIZE, y * BLOCK_SIZE, 0);

		block.id = id;
		block.x = x;
		block.y = y;
		
		block.WorldInit(this);
		return block;
	}

	public void RemoveBlock(int x, int y)
	{
		if (Blocks[x, y] != null)
			Destroy(Blocks[x, y].gameObject);
		Blocks[x, y] = null;
	}

	public bool HasBlock(int x, int y)
	{
		return !(Blocks[x,y] == null || Blocks[x,y].id == BlockID.None);
	}

	void SpawnPerson(bool isPlayer, int x, int y)
	{
		Debug.Log("Spawn Player (" + x + "," + y + ")[" + (isPlayer ? "Ply" : "Bot") + "]");

		Person person = Instantiate(BasePerson);
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

	void SpawnGoblinDoor(int x, int y)
	{
		Place(BlockID.GoblinDoor, x, y);

		for (int i = 0; i < 4; ++i)
			Place(BlockID.RedBrickFloor, x - 1 + i, y - 1);

		for (int i = 0; i < 4; ++i)
			for (int n = 0; n < 3; ++n)
				SpawnBackground(BlockID.RedBrick, x - 1 + i, y + n);

		for (int i = 0; i < 4; ++i)
			SpawnBlock(BlockID.RedBrick, x - 1 + i, y + 3);
	}

void Update ()
	{
		
	}
}
