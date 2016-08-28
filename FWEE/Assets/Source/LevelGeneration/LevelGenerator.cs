using UnityEngine;
using System.Collections;

public class LevelGenerator : MonoBehaviour {

    public Level gameLevel = new Level();

    public Block[] standardBlocks;
    public Block[] playerStartBlocks;
    public Block[] fountainBlocks;
    public Block[] spawnerBlocks;

	// Use this for initialization
	void Start () {
        GenerateLevel();
	}

    public Level GenerateLevel() {

        SetUpArrays();

        InitialiseStartingBlock();

        
        return gameLevel;
    }

    void SetUpArrays() {

        // Initialise the section columns.
        gameLevel.levelSections = new Section[gameLevel.numberOfSections.x][];
        for (int sX = 0; sX < gameLevel.levelSections.Length; sX++) {
            // Initialise the section rows.
            gameLevel.levelSections[sX] = new Section[gameLevel.numberOfSections.y];
            for (int sY = 0; sY < gameLevel.levelSections[sX].Length; sY++) {
                // Initialise the section.
                Section currentSection = new Section();

                // Initialise the block columns.
                currentSection.sectionBlocks = new Block[currentSection.numberOfBlocks.x][];
                for (int bX = 0; bX < currentSection.sectionBlocks.Length; bX++) {
                    // Initialise the block rows.
                    currentSection.sectionBlocks[bX] = new Block[currentSection.numberOfBlocks.y];
                    for (int bY = 0; bY < currentSection.sectionBlocks[bX].Length; bY++) {
                        // Nullify the blocks so they can be changed later.
                        currentSection.sectionBlocks[bX][bY] = null;
                    }
                }

                gameLevel.levelSections[sX][sY] = currentSection;
            }
        }
    }

    // Initialises the start block for the player spawn.
    void InitialiseStartingBlock() {
        int xStartSection = Random.Range(0, gameLevel.numberOfSections.x);
        int yStartSection = Random.Range(0, gameLevel.numberOfSections.y);
        int xStartBlock = Random.Range(0, gameLevel.levelSections[xStartSection][yStartSection].numberOfBlocks.x);
        int yStartBlock = Random.Range(0, gameLevel.levelSections[xStartSection][yStartSection].numberOfBlocks.y);

        int startBlock = Random.Range(0, playerStartBlocks.Length);

        if(playerStartBlocks[0] == null){
            Debug.LogError("There are no starting blocks to choose from.");
            return;
        }

        gameLevel.levelSections[xStartSection][yStartSection].sectionBlocks[xStartBlock][yStartBlock] = playerStartBlocks[startBlock];

        Debug.Log("Set starting block to be at (" + xStartSection.ToString() + "," + yStartSection.ToString() + ").(" + xStartBlock.ToString() + "," + yStartSection.ToString() + "), with block type '" + playerStartBlocks[startBlock].name + "'.");
    }
}
