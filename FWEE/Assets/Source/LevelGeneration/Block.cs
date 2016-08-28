using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public enum BlockAttribute {
    LeftExit,           // The player can navigate out the left side of the block.
    TopExit,            // The player can navigate out the top of the block.
    RightExit,          // The player can navigate out the right side of the block.
    OpenBottom,         // The player can fall out the bottom side of the block.
    OpenTop             // The player can fall through the top side of the block.
}

public enum BlockType {
    Normal,             // No special block type.
    PlayerStart,        // The block in which the player starts.
    ItemFountain,       // A block which contains an item fountain.
    EnemySpawner,       // A block where enemies will continuously spawn.
    PlayerGoal          // The block which contains the player's end goal.
}

public class Block : MonoBehaviour {

    public GameObject[][] blockTiles;
    public List<BlockAttribute> attributes = new List<BlockAttribute>();
    public BlockType type;

    public bool HasAttribute(BlockAttribute attribute) {
        foreach (BlockAttribute ba in attributes) {
            if (ba == attribute) {
                return true;
            }
        }

        return false;
    }
}
