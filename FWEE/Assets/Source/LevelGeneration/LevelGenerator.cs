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

        // Instantiate chunks based on their type and their exits.
        // TODO: Move into its own method. -Dean
        for (int gX = 0; gX < level.chunkGrid.GetLength(0); gX++) {
            for (int gY = 0; gY < level.chunkGrid.GetLength(1); gY++) {
                Vector3 position = new Vector3(gX * chunkSize.x, gY * chunkSize.y, 0);

                Chunk current = level.chunkGrid[gX, gY];

                if (current.types.HasFlag(ChunkType.PlayerStart)) {
                    // If the chunk is the player start.

                    GameObject startChunk = new GameObject();

                    if (current.exits.Is(ChunkExit.Left)) {
                        // Left exit only.
                        startChunk = InstantiateFromArray(StartLeftChunks, position);
                    } else if (current.exits.Is(ChunkExit.Right)) {
                        // Right exit only.
                        startChunk = InstantiateFromArray(StartRightChunks, position);
                    } else if (current.exits.Is(ChunkExit.Bottom)) {
                        // Bottom exit only.
                        startChunk = InstantiateFromArray(StartDownChunks, position);
                    } else if (current.exits.Is(ChunkExit.Left, ChunkExit.Right)) {
                        // Left and right exits.
                        startChunk = InstantiateFromArray(StartLeftRightChunks, position);
                    } else if (current.exits.Is(ChunkExit.Left, ChunkExit.Bottom)) {
                        // Left and bottom exits.
                        startChunk = InstantiateFromArray(StartLeftDownChunks, position);
                    } else if (current.exits.Is(ChunkExit.Right, ChunkExit.Bottom)) {
                        // Right and bottom exits.
                        startChunk = InstantiateFromArray(StartRightDownChunks, position);
                    } else if (current.exits.Is(ChunkExit.Left, ChunkExit.Right, ChunkExit.Bottom)) {
                        // Left, right and bottom exits.
                        startChunk = InstantiateFromArray(StartLeftRightDownChunks, position);
                    } else {
                        Debug.Log("Start chunk error");
                    }

                    // Spawn player
                    if(startChunk != null){
                        GameObject player = Instantiate(Player, startChunk.transform.FindChild("StartPoint").transform.position, Quaternion.identity) as GameObject;
                    }
                } else if (current.types.HasFlag(ChunkType.EndGoal)) {
                    // If the chunk is the end goal.
                    // If the chunk is the player start.
                    if (current.exits.Is(ChunkExit.Left)) {
                        // Left exit only.
                        InstantiateFromArray(EndLeftChunks, position);
                    } else if (current.exits.Is(ChunkExit.Right)) {
                        // Right exit only.
                        InstantiateFromArray(EndRightChunks, position);
                    } else if (current.exits.Is(ChunkExit.Top)) {
                        // Bottom exit only.
                        InstantiateFromArray(EndUpChunks, position);
                    } else if (current.exits.Is(ChunkExit.Left, ChunkExit.Right)) {
                        // Left and right exits.
                        InstantiateFromArray(EndLeftRightChunks, position);
                    } else if (current.exits.Is(ChunkExit.Left, ChunkExit.Top)) {
                        // Left and bottom exits.
                        InstantiateFromArray(EndLeftUpChunks, position);
                    } else if (current.exits.Is(ChunkExit.Right, ChunkExit.Top)) {
                        // Right and bottom exits.
                        InstantiateFromArray(EndRightUpChunks, position);
                    } else if (current.exits.Is(ChunkExit.Left, ChunkExit.Right, ChunkExit.Top)) {
                        // Left, right and bottom exits.
                        InstantiateFromArray(EndLeftRightUpChunks, position);
                    } else {
                        Debug.Log("End chunk error");
                    }
                } else {
                    // Place chunks based on their exits.
                    if (current.exits.Is(ChunkExit.Left)) {
                        // Left exit only.
                        InstantiateFromArray(LeftChunks, position);
                    } else if (current.exits.Is(ChunkExit.Right)) {
                        // Right exit only.
                        InstantiateFromArray(RightChunks, position);
                    } else if (current.exits.Is(ChunkExit.Bottom)) {
                        // Bottom exit only.
                        InstantiateFromArray(DownChunks, position);
                    } else if (current.exits.Is(ChunkExit.Top)) {
                        // Top exit only.
                        InstantiateFromArray(UpChunks, position);
                    } else if (current.exits.Is(ChunkExit.Left, ChunkExit.Right)) {
                        // Left and right exits.
                        InstantiateFromArray(LeftRightChunks, position);
                    } else if (current.exits.Is(ChunkExit.Left, ChunkExit.Bottom)) {
                        // Left and bottom exits.
                        InstantiateFromArray(LeftDownChunks, position);
                    } else if (current.exits.Is(ChunkExit.Left, ChunkExit.Top)) {
                        // Left and top exits.
                        InstantiateFromArray(LeftUpChunks, position);
                    } else if (current.exits.Is(ChunkExit.Right, ChunkExit.Bottom)) {
                        // Right and bottom exits.
                        InstantiateFromArray(RightDownChunks, position);
                    } else if (current.exits.Is(ChunkExit.Right, ChunkExit.Top)) {
                        // Right and top exits.
                        InstantiateFromArray(RightUpChunks, position);
                    } else if (current.exits.Is(ChunkExit.Bottom, ChunkExit.Top)) {
                        // Bottom and top exits.
                        InstantiateFromArray(UpDownChunks, position);
                    } else if (current.exits.Is(ChunkExit.Left, ChunkExit.Right, ChunkExit.Bottom)) {
                        // Left, right and bottom exits.
                        InstantiateFromArray(LeftRightDownChunks, position);
                    } else if (current.exits.Is(ChunkExit.Left, ChunkExit.Right, ChunkExit.Top)) {
                        // Left, right and top exits.
                        InstantiateFromArray(LeftRightUpChunks, position);
                    } else if (current.exits.Is(ChunkExit.Left, ChunkExit.Bottom, ChunkExit.Top)) {
                        // Left, bottom and top exits.
                        InstantiateFromArray(LeftUpDownChunks, position);
                    } else if (current.exits.Is(ChunkExit.Right, ChunkExit.Bottom, ChunkExit.Top)) {
                        // Right, bottom and top exits.
                        InstantiateFromArray(RightUpDownChunks, position);
                    } else if (current.exits.Is(ChunkExit.Left, ChunkExit.Right, ChunkExit.Bottom, ChunkExit.Top)) {
                        // Left, right, bottom and top exits.
                        InstantiateFromArray(LeftRightUpDownChunks, position);
                    } else {
                        // Non-path chunks (currently impassible).
                        InstantiateFromArray(ImpassibleChunks, position);
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
