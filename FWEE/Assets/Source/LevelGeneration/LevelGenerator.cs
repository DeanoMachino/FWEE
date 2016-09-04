using UnityEngine;
using System.Collections;

enum Direction {
    None,
    Left,
    Right,
    Up,
    Down
}

public class LevelGenerator : MonoBehaviour {

    public Level level = new Level();

    public Vector2i gridSize = new Vector2i(8, 8);
    public Vector2i chunkSize = new Vector2i(8, 8);

    public GameObject Player;

    public GameObject[] LeftChunks;                //1
    public GameObject[] RightChunks;               //1
    public GameObject[] UpChunks;                  //1
    public GameObject[] DownChunks;                //1
    public GameObject[] LeftRightChunks;           //2
    public GameObject[] LeftUpChunks;              //2
    public GameObject[] LeftDownChunks;            //2
    public GameObject[] RightUpChunks;             //2
    public GameObject[] RightDownChunks;           //2
    public GameObject[] UpDownChunks;              //2
    public GameObject[] LeftRightUpChunks;         //3
    public GameObject[] LeftRightDownChunks;       //3
    public GameObject[] LeftUpDownChunks;          //3
    public GameObject[] RightUpDownChunks;         //3
    public GameObject[] LeftRightUpDownChunks;     //4

    public GameObject[] ImpassibleChunks;

    public GameObject[] StartLeftChunks;
    public GameObject[] StartRightChunks;
    public GameObject[] StartDownChunks;
    public GameObject[] StartLeftRightChunks;
    public GameObject[] StartLeftDownChunks;
    public GameObject[] StartRightDownChunks;
    public GameObject[] StartLeftRightDownChunks;

    public GameObject[] EndLeftChunks;
    public GameObject[] EndRightChunks;
    public GameObject[] EndUpChunks;
    public GameObject[] EndLeftRightChunks;
    public GameObject[] EndLeftUpChunks;
    public GameObject[] EndRightUpChunks;
    public GameObject[] EndLeftRightUpChunks;

    public GameObject[] FountainLeftChunks;                //1
    public GameObject[] FountainRightChunks;               //1
    public GameObject[] FountainUpChunks;                  //1
    public GameObject[] FountainDownChunks;                //1
    public GameObject[] FountainLeftRightChunks;           //2
    public GameObject[] FountainLeftUpChunks;              //2
    public GameObject[] FountainLeftDownChunks;            //2
    public GameObject[] FountainRightUpChunks;             //2
    public GameObject[] FountainRightDownChunks;           //2
    public GameObject[] FountainUpDownChunks;              //2
    public GameObject[] FountainLeftRightUpChunks;         //3
    public GameObject[] FountainLeftRightDownChunks;       //3
    public GameObject[] FountainLeftUpDownChunks;          //3
    public GameObject[] FountainRightUpDownChunks;         //3
    public GameObject[] FountainLeftRightUpDownChunks;     //4

    public GameObject EnemySpawner;

    public SpriteSet EarthSprites;

    Vector2i startChunk;
    Vector2i currentChunk;
    Direction previousMove;
    bool endReached = false;

	// Use this for initialization
	void Start () {
        GenerateLevel();
	}

    public Level GenerateLevel() {

        SetUpArray();

        InitialiseStartingBlock();

        // Loops several times to add extra/overlapping routes.
        for (int i = 0; i < 3; i++) {
            currentChunk = startChunk;
            endReached = false;
            previousMove = Direction.None;

            // Loop while the end goal chunk has not been placed.
            while (!endReached) {
                // Get a random number between 0 and 4 (inclusive).
                int randVal = Random.Range(0, 5);

                switch (previousMove) {
                    case Direction.Left:
                        level.chunkGrid[currentChunk.x, currentChunk.y].exits.Add(ChunkExit.Right);
                        break;
                    case Direction.Right:
                        level.chunkGrid[currentChunk.x, currentChunk.y].exits.Add(ChunkExit.Left);
                        break;
                    case Direction.Up:
                        level.chunkGrid[currentChunk.x, currentChunk.y].exits.Add(ChunkExit.Bottom);
                        break;
                    case Direction.Down:
                        level.chunkGrid[currentChunk.x, currentChunk.y].exits.Add(ChunkExit.Top);
                        break;
                }

                switch (randVal) {
                    case 0:
                    case 1:
                        // Move the path left.
                        if (CheckAdjacentIsValid(Direction.Left)) {
                            level.chunkGrid[currentChunk.x, currentChunk.y].exits.Add(ChunkExit.Left);
                            currentChunk = new Vector2i(currentChunk.x - 1, currentChunk.y);
                            level.chunkGrid[currentChunk.x, currentChunk.y].types.Add(ChunkType.Path);
                            previousMove = Direction.Left;
                        }
                        break;
                    case 2:
                    case 3:
                        // Move the path right.
                        if (CheckAdjacentIsValid(Direction.Right)) {
                            level.chunkGrid[currentChunk.x, currentChunk.y].exits.Add(ChunkExit.Right);
                            currentChunk = new Vector2i(currentChunk.x + 1, currentChunk.y);
                            level.chunkGrid[currentChunk.x, currentChunk.y].types.Add(ChunkType.Path);
                            previousMove = Direction.Right;
                        }
                        break;
                    case 4:
                        // Drop the path down.
                        if (!currentChunk.Equals(startChunk)) {
                            if (CheckAdjacentIsValid(Direction.Down)) {
                                level.chunkGrid[currentChunk.x, currentChunk.y].exits.Add(ChunkExit.Bottom);
                                currentChunk = new Vector2i(currentChunk.x, currentChunk.y - 1);
                                level.chunkGrid[currentChunk.x, currentChunk.y].types.Add(ChunkType.Path);
                                previousMove = Direction.Down;
                            } else {
                                // Set end goal chunk.
                                if (i == 0) {
                                    level.chunkGrid[currentChunk.x, currentChunk.y].types.Add(ChunkType.Path, ChunkType.EndGoal);
                                }
                            }
                        }
                        break;
                        
                }

                // If the current chunk is the end goal, set the end as having been reached.
                if (level.chunkGrid[currentChunk.x, currentChunk.y].types.HasFlag(ChunkType.EndGoal)) {
                    endReached = true;
                }
            }
        }

        // Set fountains.
        for (int i = 0; i < 4; i++) {
            bool fountainSet = false;
            while (!fountainSet) {
                foreach (Chunk c in level.chunkGrid) {
                    if (c.types.HasFlag(ChunkType.Path) && !c.types.HasFlag(ChunkType.PlayerStart) && !c.types.HasFlag(ChunkType.EndGoal) && !c.types.HasFlag(ChunkType.ItemFountain) && !c.types.HasFlag(ChunkType.EnemySpawner)) {
                        if (Random.value < 0.01f) {
                            c.types.Add(ChunkType.ItemFountain);
                            fountainSet = true;
                            break;
                        }
                    }
                }
            }
        }

        // Set enemy spawners.
        foreach (Chunk c in level.chunkGrid) {
            if (c.types.HasFlag(ChunkType.Path) && !c.types.HasFlag(ChunkType.PlayerStart) && !c.types.HasFlag(ChunkType.EndGoal) && !c.types.HasFlag(ChunkType.ItemFountain) && !c.types.HasFlag(ChunkType.EnemySpawner)) {
                if (Random.value < 0.2f) {
                    c.types.Add(ChunkType.EnemySpawner);
                }
            }
        }

        // Instantiate chunks based on their type and their exits.
        // TODO: Move into its own method. -Dean
        for (int gX = 0; gX < level.chunkGrid.GetLength(0); gX++) {
            for (int gY = 0; gY < level.chunkGrid.GetLength(1); gY++) {
                Vector3 position = new Vector3(gX * chunkSize.x, gY * chunkSize.y, 0);

                Chunk current = level.chunkGrid[gX, gY];

                GameObject chunk = new GameObject();

                if (current.types.HasFlag(ChunkType.PlayerStart)) {
                    // If the chunk is the player start.
                    if (current.exits.Is(ChunkExit.Left)) {
                        // Left exit only.
                        chunk = InstantiateFromArray(StartLeftChunks, position);
                    } else if (current.exits.Is(ChunkExit.Right)) {
                        // Right exit only.
                        chunk = InstantiateFromArray(StartRightChunks, position);
                    } else if (current.exits.Is(ChunkExit.Bottom)) {
                        // Bottom exit only.
                        chunk = InstantiateFromArray(StartDownChunks, position);
                    } else if (current.exits.Is(ChunkExit.Left, ChunkExit.Right)) {
                        // Left and right exits.
                        chunk = InstantiateFromArray(StartLeftRightChunks, position);
                    } else if (current.exits.Is(ChunkExit.Left, ChunkExit.Bottom)) {
                        // Left and bottom exits.
                        chunk = InstantiateFromArray(StartLeftDownChunks, position);
                    } else if (current.exits.Is(ChunkExit.Right, ChunkExit.Bottom)) {
                        // Right and bottom exits.
                        chunk = InstantiateFromArray(StartRightDownChunks, position);
                    } else if (current.exits.Is(ChunkExit.Left, ChunkExit.Right, ChunkExit.Bottom)) {
                        // Left, right and bottom exits.
                        chunk = InstantiateFromArray(StartLeftRightDownChunks, position);
                    } else {
                        Debug.Log("Start chunk error");
                    }

                    // Spawn player
                    GameObject player = Instantiate(Player, chunk.transform.FindChild("StartPoint").transform.position, Quaternion.identity) as GameObject;
                    
                } else if (current.types.HasFlag(ChunkType.EndGoal)) {
                    // If the chunk is the end goal.
                    if (current.exits.Is(ChunkExit.Left)) {
                        // Left exit only.
                        chunk = InstantiateFromArray(EndLeftChunks, position);
                    } else if (current.exits.Is(ChunkExit.Right)) {
                        // Right exit only.
                        chunk = InstantiateFromArray(EndRightChunks, position);
                    } else if (current.exits.Is(ChunkExit.Top)) {
                        // Bottom exit only.
                        chunk = InstantiateFromArray(EndUpChunks, position);
                    } else if (current.exits.Is(ChunkExit.Left, ChunkExit.Right)) {
                        // Left and right exits.
                        chunk = InstantiateFromArray(EndLeftRightChunks, position);
                    } else if (current.exits.Is(ChunkExit.Left, ChunkExit.Top)) {
                        // Left and bottom exits.
                        chunk = InstantiateFromArray(EndLeftUpChunks, position);
                    } else if (current.exits.Is(ChunkExit.Right, ChunkExit.Top)) {
                        // Right and bottom exits.
                        chunk = InstantiateFromArray(EndRightUpChunks, position);
                    } else if (current.exits.Is(ChunkExit.Left, ChunkExit.Right, ChunkExit.Top)) {
                        // Left, right and bottom exits.
                        chunk = InstantiateFromArray(EndLeftRightUpChunks, position);
                    } else {
                        Debug.Log("End chunk error");
                    }
                } else if (current.types.HasFlag(ChunkType.ItemFountain)) {
                    // If chunk is an item fountain.
                    if (current.exits.Is(ChunkExit.Left)) {
                        // Left exit only.
                        chunk = InstantiateFromArray(FountainLeftChunks, position);
                    } else if (current.exits.Is(ChunkExit.Right)) {
                        // Right exit only.
                        chunk = InstantiateFromArray(FountainRightChunks, position);
                    } else if (current.exits.Is(ChunkExit.Bottom)) {
                        // Bottom exit only.
                        chunk = InstantiateFromArray(FountainDownChunks, position);
                    } else if (current.exits.Is(ChunkExit.Top)) {
                        // Top exit only.
                        chunk = InstantiateFromArray(FountainUpChunks, position);
                    } else if (current.exits.Is(ChunkExit.Left, ChunkExit.Right)) {
                        // Left and right exits.
                        chunk = InstantiateFromArray(FountainLeftRightChunks, position);
                    } else if (current.exits.Is(ChunkExit.Left, ChunkExit.Bottom)) {
                        // Left and bottom exits.
                        chunk = InstantiateFromArray(FountainLeftDownChunks, position);
                    } else if (current.exits.Is(ChunkExit.Left, ChunkExit.Top)) {
                        // Left and top exits.
                        chunk = InstantiateFromArray(FountainLeftUpChunks, position);
                    } else if (current.exits.Is(ChunkExit.Right, ChunkExit.Bottom)) {
                        // Right and bottom exits.
                        chunk = InstantiateFromArray(FountainRightDownChunks, position);
                    } else if (current.exits.Is(ChunkExit.Right, ChunkExit.Top)) {
                        // Right and top exits.
                        chunk = InstantiateFromArray(FountainRightUpChunks, position);
                    } else if (current.exits.Is(ChunkExit.Bottom, ChunkExit.Top)) {
                        // Bottom and top exits.
                        chunk = InstantiateFromArray(FountainUpDownChunks, position);
                    } else if (current.exits.Is(ChunkExit.Left, ChunkExit.Right, ChunkExit.Bottom)) {
                        // Left, right and bottom exits.
                        chunk = InstantiateFromArray(FountainLeftRightDownChunks, position);
                    } else if (current.exits.Is(ChunkExit.Left, ChunkExit.Right, ChunkExit.Top)) {
                        // Left, right and top exits.
                        chunk = InstantiateFromArray(FountainLeftRightUpChunks, position);
                    } else if (current.exits.Is(ChunkExit.Left, ChunkExit.Bottom, ChunkExit.Top)) {
                        // Left, bottom and top exits.
                        chunk = InstantiateFromArray(FountainLeftUpDownChunks, position);
                    } else if (current.exits.Is(ChunkExit.Right, ChunkExit.Bottom, ChunkExit.Top)) {
                        // Right, bottom and top exits.
                        chunk = InstantiateFromArray(FountainRightUpDownChunks, position);
                    } else if (current.exits.Is(ChunkExit.Left, ChunkExit.Right, ChunkExit.Bottom, ChunkExit.Top)) {
                        // Left, right, bottom and top exits.
                        chunk = InstantiateFromArray(FountainLeftRightUpDownChunks, position);
                    } else {
                        Debug.Log("Fountain chunk error");
                    }

                    chunk.transform.Find("Fountain").GetComponent<FountainScript>().SetFountainType(FountainType.Earth);
                } else {
                    // Place chunks based on their exits.
                    if (current.exits.Is(ChunkExit.Left)) {
                        // Left exit only.
                        chunk = InstantiateFromArray(LeftChunks, position);
                    } else if (current.exits.Is(ChunkExit.Right)) {
                        // Right exit only.
                        chunk = InstantiateFromArray(RightChunks, position);
                    } else if (current.exits.Is(ChunkExit.Bottom)) {
                        // Bottom exit only.
                        chunk = InstantiateFromArray(DownChunks, position);
                    } else if (current.exits.Is(ChunkExit.Top)) {
                        // Top exit only.
                        chunk = InstantiateFromArray(UpChunks, position);
                    } else if (current.exits.Is(ChunkExit.Left, ChunkExit.Right)) {
                        // Left and right exits.
                        chunk = InstantiateFromArray(LeftRightChunks, position);
                    } else if (current.exits.Is(ChunkExit.Left, ChunkExit.Bottom)) {
                        // Left and bottom exits.
                        chunk = InstantiateFromArray(LeftDownChunks, position);
                    } else if (current.exits.Is(ChunkExit.Left, ChunkExit.Top)) {
                        // Left and top exits.
                        chunk = InstantiateFromArray(LeftUpChunks, position);
                    } else if (current.exits.Is(ChunkExit.Right, ChunkExit.Bottom)) {
                        // Right and bottom exits.
                        chunk = InstantiateFromArray(RightDownChunks, position);
                    } else if (current.exits.Is(ChunkExit.Right, ChunkExit.Top)) {
                        // Right and top exits.
                        chunk = InstantiateFromArray(RightUpChunks, position);
                    } else if (current.exits.Is(ChunkExit.Bottom, ChunkExit.Top)) {
                        // Bottom and top exits.
                        chunk = InstantiateFromArray(UpDownChunks, position);
                    } else if (current.exits.Is(ChunkExit.Left, ChunkExit.Right, ChunkExit.Bottom)) {
                        // Left, right and bottom exits.
                        chunk = InstantiateFromArray(LeftRightDownChunks, position);
                    } else if (current.exits.Is(ChunkExit.Left, ChunkExit.Right, ChunkExit.Top)) {
                        // Left, right and top exits.
                        chunk = InstantiateFromArray(LeftRightUpChunks, position);
                    } else if (current.exits.Is(ChunkExit.Left, ChunkExit.Bottom, ChunkExit.Top)) {
                        // Left, bottom and top exits.
                        chunk = InstantiateFromArray(LeftUpDownChunks, position);
                    } else if (current.exits.Is(ChunkExit.Right, ChunkExit.Bottom, ChunkExit.Top)) {
                        // Right, bottom and top exits.
                        chunk = InstantiateFromArray(RightUpDownChunks, position);
                    } else if (current.exits.Is(ChunkExit.Left, ChunkExit.Right, ChunkExit.Bottom, ChunkExit.Top)) {
                        // Left, right, bottom and top exits.
                        chunk = InstantiateFromArray(LeftRightUpDownChunks, position);
                    } else {
                        // Non-path chunks (currently impassible).
                        chunk = InstantiateFromArray(ImpassibleChunks, position);
                    }
                }

                if (current.types.HasFlag(ChunkType.EnemySpawner)) {
                    chunk.transform.Find("EnemySpawner").GetComponent<EnemySpawner>().Enemy = EnemySpawner.GetComponent<EnemySpawner>().Enemy;
                    chunk.transform.Find("EnemySpawner").GetComponent<EnemySpawner>().spawnChance = EnemySpawner.GetComponent<EnemySpawner>().spawnChance;
                    chunk.transform.Find("EnemySpawner").GetComponent<EnemySpawner>().spawnDelay = EnemySpawner.GetComponent<EnemySpawner>().spawnDelay;
                    chunk.transform.Find("EnemySpawner").GetComponent<EnemySpawner>().spawnLimit = EnemySpawner.GetComponent<EnemySpawner>().spawnLimit;
                } else {
                    if (chunk.transform.Find("EnemySpawner") != null) {
                        chunk.transform.Find("EnemySpawner").gameObject.SetActive(false);
                    }
                }


                chunk.GetComponent<ChunkScript>().tileGrid = new GameObject[16, 16];

                for (int i = 0; i < chunk.transform.childCount; i++) {
                    if (chunk.transform.GetChild(i).tag == "Platform") {
                        int x = (int)chunk.transform.GetChild(i).transform.localPosition.x;
                        int y = (int)chunk.transform.GetChild(i).transform.localPosition.y;

                        chunk.GetComponent<ChunkScript>().tileGrid[x, y] = chunk.transform.GetChild(i).gameObject;
                    }
                }

                for (int tX = 0; tX < 16; tX++) {
                    for (int tY = 0; tY < 16; tY++) {
                        if (chunk.GetComponent<ChunkScript>().tileGrid[tX, tY] != null) {

                            bool up = false;
                            if(tY < 15)
                                up = (chunk.GetComponent<ChunkScript>().tileGrid[tX, tY + 1] != null);
                            bool down = false;
                            if(tY > 0)
                                down = (chunk.GetComponent<ChunkScript>().tileGrid[tX, tY - 1] != null);
                            bool left = false;
                            if(tX > 0)
                                left = (chunk.GetComponent<ChunkScript>().tileGrid[tX - 1, tY] != null);
                            bool right = false;
                            if(tX < 15)
                                right = (chunk.GetComponent<ChunkScript>().tileGrid[tX + 1, tY] != null);

                            if (!left) {
                                if (!right) {
                                    if (!up) {
                                        chunk.GetComponent<ChunkScript>().tileGrid[tX, tY].GetComponent<SpriteRenderer>().sprite = EarthSprites.Top;
                                    } else {
                                        if (!down) {
                                            chunk.GetComponent<ChunkScript>().tileGrid[tX, tY].GetComponent<SpriteRenderer>().sprite = EarthSprites.PlainBottom;
                                        } else {
                                            chunk.GetComponent<ChunkScript>().tileGrid[tX, tY].GetComponent<SpriteRenderer>().sprite = EarthSprites.Plain;
                                        }
                                    }
                                } else {
                                    if (!up) {
                                        chunk.GetComponent<ChunkScript>().tileGrid[tX, tY].GetComponent<SpriteRenderer>().sprite = EarthSprites.LeftTop;
                                    } else {
                                        chunk.GetComponent<ChunkScript>().tileGrid[tX, tY].GetComponent<SpriteRenderer>().sprite = EarthSprites.Left;
                                    }
                                }
                            } else {
                                if (!right) {
                                    if (!up) {
                                        chunk.GetComponent<ChunkScript>().tileGrid[tX, tY].GetComponent<SpriteRenderer>().sprite = EarthSprites.RightTop;
                                    } else {
                                        chunk.GetComponent<ChunkScript>().tileGrid[tX, tY].GetComponent<SpriteRenderer>().sprite = EarthSprites.Right;
                                    }
                                } else {
                                    if (!up) {
                                        chunk.GetComponent<ChunkScript>().tileGrid[tX, tY].GetComponent<SpriteRenderer>().sprite = EarthSprites.Top;
                                    } else {
                                        if (!down) {
                                            chunk.GetComponent<ChunkScript>().tileGrid[tX, tY].GetComponent<SpriteRenderer>().sprite = EarthSprites.PlainBottom;
                                        } else {
                                            chunk.GetComponent<ChunkScript>().tileGrid[tX, tY].GetComponent<SpriteRenderer>().sprite = EarthSprites.Plain;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }


            }
        }

        return level;
    }

    void SetUpArray() {

        level.chunkGrid = new Chunk[gridSize.x, gridSize.y];
        for (int gX = 0; gX < level.chunkGrid.GetLength(0); gX++) {
            for (int gY = 0; gY < level.chunkGrid.GetLength(1); gY++) {
                level.chunkGrid[gX, gY] = new Chunk(new Vector2i(gX, gY));
            }
        }
    }

    // Initialises the start block for the player spawn.
    void InitialiseStartingBlock() {
        // Randomly choose a starting point along the top row.
        startChunk = new Vector2i(Random.Range(0, level.chunkGrid.GetLength(0)), level.chunkGrid.GetLength(1) - 1);
        // Set the chunk type to teh player start.
        level.chunkGrid[startChunk.x, startChunk.y].types.Set(ChunkType.PlayerStart, ChunkType.Path);

        Debug.Log("Player start: " + startChunk.ToString());
    }

    // Instantiates a random prefab from a given array and returns it.
    GameObject InstantiateFromArray(GameObject[] prefabs, Vector3 position) {
        int randomIndex = Random.Range(0, prefabs.Length);

        GameObject go = Instantiate(prefabs[randomIndex], position, Quaternion.identity) as GameObject;

        return go;
    }

    // Checks whether the current chunk is on the edge of the grid.
    bool CheckAdjacentIsValid(Direction moveDirection) {
        switch (moveDirection) {
            case Direction.Left:
                if (currentChunk.x - 1 < 0)
                    return false;
                break;
            case Direction.Right:
                if (currentChunk.x + 1 >= level.chunkGrid.GetLength(0))
                    return false;
                break;
            case Direction.Up:
                if (currentChunk.y + 1 >= level.chunkGrid.GetLength(1))
                    return false;
                break;
            case Direction.Down:
                if (currentChunk.y - 1 < 0)
                    return false;
                break;
        }

        return true;
    }

}
