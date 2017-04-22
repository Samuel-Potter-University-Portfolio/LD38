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

	public static uint WORLD_WIDTH { get { return 128; } }
	public static uint WORLD_HEIGHT { get { return 16; } }
	public static float BLOCK_SIZE { get { return 2.0f; } }

	public Block[,] Blocks;
	private bool WorldInit = false;


	void Start ()
	{
		Main = this;
		Blocks = new Block[WORLD_WIDTH, WORLD_HEIGHT];
		GenerateWorld();
		SpawnPerson(true);
    }

	void GenerateWorld()
	{
		for (uint x = 0; x < WORLD_WIDTH; ++x)
			for (uint y = 0; y < WORLD_HEIGHT - 8; ++y)
			{
				BlockID id = y <= 4 ? BlockID.Stone : BlockID.Dirt;
				SpawnBlock(id, x, y);
			}

		for (uint x = 0; x < WORLD_WIDTH; ++x)
		{
			SpawnBlock(x % 16 == 0 ? BlockID.Dirt : BlockID.Grass, x, 8);

			if (x % 16 == 0)
				SpawnBlock(BlockID.Grass, x, 9);

		}

		WorldInit = true;

		for (uint x = 0; x < WORLD_WIDTH; ++x)
			for (uint y = 0; y < WORLD_HEIGHT; ++y)
				if(Blocks[x, y] != null)
					Blocks[x, y].WorldInit(this);
	}

	void SpawnBlock(BlockID id, uint x, uint y)
	{
		Block block = Instantiate(BaseBlock, transform);
		block.gameObject.transform.localPosition = new Vector3(x * BLOCK_SIZE, y * BLOCK_SIZE, 0);
		Blocks[x, y] = block;

		block.id = id;
		block.x = x;
		block.y = y;

		if (WorldInit)
			block.WorldInit(this);
    }

	void SpawnPerson(bool isPlayer)
	{
		Person person = Instantiate(BasePerson);
		person.IsPlayer = isPlayer;
		person.transform.position = new Vector3(WORLD_WIDTH / 2.0f * BLOCK_SIZE, WORLD_HEIGHT * BLOCK_SIZE);
    }

	void Update ()
	{
		
	}
}
