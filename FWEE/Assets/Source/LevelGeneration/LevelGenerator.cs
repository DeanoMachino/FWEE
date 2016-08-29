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

    public GameObject[] LeftChunk;                //1
    public GameObject[] RightChunk;               //1
    public GameObject[] DownChunk;                //1
    public GameObject[] UpChunk;                  //1
    public GameObject[] LeftRightChunk;           //2
    public GameObject[] LeftDownChunk;            //2
    public GameObject[] LeftUpChunk;              //2
    public GameObject[] RightDownChunk;           //2
    public GameObject[] RightUpChunk;             //2
    public GameObject[] DownUpChunk;              //2
    public GameObject[] LeftRightDownChunk;       //3
    public GameObject[] LeftRightUpChunk;         //3
    public GameObject[] LeftDownUpChunk;          //3
    public GameObject[] RightDownUpChunk;         //3
    public GameObject[] LeftRightDownUpChunk;     //4
    public GameObject[] NonPathChunk;
    public GameObject[] StartChunk;
    public GameObject[] EndChunk;

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
                    InstantiateFromArray(StartChunk, position);
                } else if (current.types.HasFlag(ChunkType.EndGoal)) {
                    // If the chunk is the end goal.
                    InstantiateFromArray(EndChunk, position);
                } else {
                    // Place chunks based on their exits.
                    if (current.exits.Is(ChunkExit.Left)) {
                        // Left exit only.
                        InstantiateFromArray(LeftChunk, position);
                    } else if (current.exits.Is(ChunkExit.Right)) {
                        // Right exit only.
                        InstantiateFromArray(RightChunk, position);
                    } else if (current.exits.Is(ChunkExit.Bottom)) {
                        // Bottom exit only.
                        InstantiateFromArray(DownChunk, position);
                    } else if (current.exits.Is(ChunkExit.Top)) {
                        // Top exit only.
                        InstantiateFromArray(UpChunk, position);
                    } else if (current.exits.Is(ChunkExit.Left, ChunkExit.Right)) {
                        // Left and right exits.
                        InstantiateFromArray(LeftRightChunk, position);
                    } else if (current.exits.Is(ChunkExit.Left, ChunkExit.Bottom)) {
                        // Left and bottom exits.
                        InstantiateFromArray(LeftDownChunk, position);
                    } else if (current.exits.Is(ChunkExit.Left, ChunkExit.Top)) {
                        // Left and top exits.
                        InstantiateFromArray(LeftUpChunk, position);
                    } else if (current.exits.Is(ChunkExit.Right, ChunkExit.Bottom)) {
                        // Right and bottom exits.
                        InstantiateFromArray(RightDownChunk, position);
                    } else if (current.exits.Is(ChunkExit.Right, ChunkExit.Top)) {
                        // Right and top exits.
                        InstantiateFromArray(RightUpChunk, position);
                    } else if (current.exits.Is(ChunkExit.Bottom, ChunkExit.Top)) {
                        // Bottom and top exits.
                        InstantiateFromArray(DownUpChunk, position);
                    } else if (current.exits.Is(ChunkExit.Left, ChunkExit.Right, ChunkExit.Bottom)) {
                        // Left, right and bottom exits.
                        InstantiateFromArray(LeftRightDownChunk, position);
                    } else if (current.exits.Is(ChunkExit.Left, ChunkExit.Right, ChunkExit.Top)) {
                        // Left, right and top exits.
                        InstantiateFromArray(LeftRightUpChunk, position);
                    } else if (current.exits.Is(ChunkExit.Left, ChunkExit.Bottom, ChunkExit.Top)) {
                        // Left, bottom and top exits.
                        InstantiateFromArray(LeftDownUpChunk, position);
                    } else if (current.exits.Is(ChunkExit.Right, ChunkExit.Bottom, ChunkExit.Top)) {
                        // Right, bottom and top exits.
                        InstantiateFromArray(RightDownUpChunk, position);
                    } else if (current.exits.Is(ChunkExit.Left, ChunkExit.Right, ChunkExit.Bottom, ChunkExit.Top)) {
                        // Left, right, bottom and top exits.
                        InstantiateFromArray(LeftRightDownUpChunk, position);
                    } else {
                        // Non-path chunks (currently impassible).
                        InstantiateFromArray(NonPathChunk, position);
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

    // Instantiates a random prefab from a given array.
    void InstantiateFromArray(GameObject[] prefabs, Vector3 position) {
        int randomIndex = Random.Range(0, prefabs.Length);

        GameObject go = Instantiate(prefabs[randomIndex], position, Quaternion.identity) as GameObject;
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
