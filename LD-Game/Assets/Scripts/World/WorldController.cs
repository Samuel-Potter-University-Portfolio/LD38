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
	public static uint WORLD_HEIGHT { get { return 64; } }
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
		const float bumps = 5.0f;
		const float height = 5.0f;
		const float start_height = 20.0f;

		//Sin wave
		for (uint x = 0; x < WORLD_WIDTH; ++x)
		{
			float v = (float)x / (float)WORLD_WIDTH;
			float normalized_height = Mathf.Sin(v * Mathf.PI * 2.0f * bumps) * 0.5f + 0.5f;
			int current_height = Mathf.FloorToInt(start_height + normalized_height * height);

			if (current_height >= WORLD_HEIGHT - 1)
				current_height = (int)WORLD_HEIGHT - 1;

			for (uint y = 0; y <= current_height; ++y)
			{
				if (y == current_height)
					SpawnBlock(BlockID.Grass, x, y);
				else if (y >= current_height - 4.0f)
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
