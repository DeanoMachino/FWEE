using UnityEngine;
using System.Collections;

enum Direction {
    Left,
    Right,
    Up,
    Down
}

public class LevelGenerator : MonoBehaviour {

    public Level level = new Level();

    /*public Block[] standardBlocks;
    public Block[] playerStartBlocks;
    public Block[] fountainBlocks;
    public Block[] spawnerBlocks;*/

    public GameObject LeftChunk;
    public GameObject RightChunk;
    public GameObject DownChunk;
    public GameObject LeftRightChunk;
    public GameObject LeftDownChunk;
    public GameObject RightDownChunk;
    public GameObject LeftRightDownChunk;
    public GameObject NonPathChunk;
    public GameObject StartChunk;
    public GameObject EndChunk;

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

            // Loop while the end goal chunk has not been placed.
            while (!endReached) {
                // Get a random number between 0 and 4 (inclusive).
                int randVal = Random.Range(0, 5);

                switch (randVal) {
                    case 0:
                    case 1:
                        // Move the path left.
                        if (CheckAdjacentIsValid(Direction.Left)) {
                            level.chunkGrid[currentChunk.x, currentChunk.y].exits.Add(ChunkExit.Left);
                            currentChunk = new Vector2i(currentChunk.x - 1, currentChunk.y);
                            level.chunkGrid[currentChunk.x, currentChunk.y].types.Add(ChunkType.Path);
                        }
                        break;
                    case 2:
                    case 3:
                        // Move the path right.
                        if (CheckAdjacentIsValid(Direction.Right)) {
                            level.chunkGrid[currentChunk.x, currentChunk.y].exits.Add(ChunkExit.Right);
                            currentChunk = new Vector2i(currentChunk.x + 1, currentChunk.y);
                            level.chunkGrid[currentChunk.x, currentChunk.y].types.Add(ChunkType.Path);
                        }
                        break;
                    case 4:
                        // Drop the path down.
                        if (CheckAdjacentIsValid(Direction.Down)) {
                            level.chunkGrid[currentChunk.x, currentChunk.y].exits.Add(ChunkExit.Bottom);
                            currentChunk = new Vector2i(currentChunk.x, currentChunk.y - 1);
                            level.chunkGrid[currentChunk.x, currentChunk.y].types.Add(ChunkType.Path);
                        } else {
                            // Set end goal chunk.
                            if (i == 0) {
                                level.chunkGrid[currentChunk.x, currentChunk.y].types.Add(ChunkType.Path, ChunkType.EndGoal);
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
                Vector3 position = new Vector3(gX * 8, gY * 8, 0);

                Chunk current = level.chunkGrid[gX, gY];

                if (current.types.HasFlag(ChunkType.PlayerStart)) {
                    // If the chunk is the player start.
                    GameObject go = Instantiate(StartChunk, position, Quaternion.identity) as GameObject;
                } else if (current.types.HasFlag(ChunkType.EndGoal)) {
                    // If the chunk is the end goal.
                    GameObject go = Instantiate(EndChunk, position, Quaternion.identity) as GameObject;
                } else {
                    // Place chunks based on their exits.
                    if (current.exits.Is(ChunkExit.Left)) {
                        // Left exit only.
                        GameObject go = Instantiate(LeftChunk, position, Quaternion.identity) as GameObject;
                    } else if (current.exits.Is(ChunkExit.Right)) {
                        // Right exit only.
                        GameObject go = Instantiate(RightChunk, position, Quaternion.identity) as GameObject;
                    } else if (current.exits.Is(ChunkExit.Bottom)) {
                        // Bottom exit only.
                        GameObject go = Instantiate(DownChunk, position, Quaternion.identity) as GameObject;
                    } else if (current.exits.Is(ChunkExit.Left, ChunkExit.Right)) {
                        // Left and right exits.
                        GameObject go = Instantiate(LeftRightChunk, position, Quaternion.identity) as GameObject;
                    } else if (current.exits.Is(ChunkExit.Left, ChunkExit.Bottom)) {
                        // Left and bottom exits.
                        GameObject go = Instantiate(LeftDownChunk, position, Quaternion.identity) as GameObject;
                    } else if (current.exits.Is(ChunkExit.Right, ChunkExit.Bottom)) {
                        // Right and bottom exits.
                        GameObject go = Instantiate(RightDownChunk, position, Quaternion.identity) as GameObject;
                    } else if (current.exits.Is(ChunkExit.Left, ChunkExit.Right, ChunkExit.Bottom)) {
                        // Left, right and bottom exits.
                        GameObject go = Instantiate(LeftRightDownChunk, position, Quaternion.identity) as GameObject;
                    } else {
                        // Non-path chunks (currently impassible).
                        GameObject go = Instantiate(NonPathChunk, position, Quaternion.identity) as GameObject;
                    }
                }
            }
        }

        return level;
    }

    void SetUpArray() {

        level.chunkGrid = new Chunk[level.gridSize.x, level.gridSize.y];
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
