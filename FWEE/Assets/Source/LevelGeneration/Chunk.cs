using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

[Flags]
public enum ChunkExit {
    None,
    Left,
    Right,
    Top,
    Bottom
}

[Flags]
public enum ChunkType {
    None,               // A chunk without a type (should never be the case).
    Path,               // A chunk which is part of the main path.
    NonPath,            // A chunk which is not part of the main path.
    PlayerStart,        // The chunk in which the player starts.
    ItemFountain,       // A chunk which contains an item fountain.
    EnemySpawner,       // A chunk where enemies will continuously spawn.
    EndGoal          // The chunk which contains the player's end goal.
}

public class Chunk {

    public Flag<ChunkExit> exits;
    public Flag<ChunkType> types;

    public Vector2i position { get; private set; }

    public Chunk(Vector2i pos) {
        position = pos;

        exits = new Flag<ChunkExit>();
        types = new Flag<ChunkType>();
    }
}
