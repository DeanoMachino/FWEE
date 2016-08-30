using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class ChunkDesignerWindow : EditorWindow {

    public GUISkin chunkTileToggleSkin;

    bool chunkSettingsGroup;
    int gridSizeX = 8;
    int gridSizeY = 8;
    GameObject tileGameObject;

    bool chunkDesignerGroup;
    bool drawTool;
    bool[,] chunkGrid;
    bool leftExit;
    bool rightExit;
    bool topExit;
    bool bottomExit;
    bool clearAfter;

    bool createdChunksGroup;
    List<GameObject> createdChunks;

    public void OnEnable() {
        InitialiseGrid();
        chunkTileToggleSkin = Resources.Load("ChunkDesigner/ChunkTileToggle") as GUISkin;

        createdChunks = new List<GameObject>();
    }

    [MenuItem("Window/Chunk Designer")]
    public static void ShowWindow() {
        EditorWindow.GetWindow(typeof(ChunkDesignerWindow));
    }

    void OnGUI() {
        GUI.skin = chunkTileToggleSkin;

        // Window title
        GUILayout.Label("Create New Chunk", EditorStyles.boldLabel);

        // Chunk settings foldout
        chunkSettingsGroup = EditorGUILayout.Foldout(chunkSettingsGroup, "Chunk Settings");
        if (chunkSettingsGroup) {
            ChunkSettings();
        }

        EditorGUILayout.Separator();

        // Chunk designer foldout
        chunkDesignerGroup = EditorGUILayout.Foldout(chunkDesignerGroup, "Chunk Designer");
        if (chunkDesignerGroup) {
            ChunkDesigner();
        }

        EditorGUILayout.Separator();

        // Created chunks foldout
        createdChunksGroup = EditorGUILayout.Foldout(createdChunksGroup, "Created Chunks");
        if (createdChunksGroup) {
            CreatedChunksList();
        }
    }

    void ChunkSettings() {
        // Grid Size
        EditorGUILayout.BeginHorizontal();
        {
            EditorGUILayout.BeginVertical();
            {
                gridSizeX = EditorGUILayout.IntField("Grid Size X", gridSizeX);
                gridSizeY = EditorGUILayout.IntField("Grid Size Y", gridSizeY);
            }
            EditorGUILayout.EndVertical();

            if (GUILayout.Button("Set Grid Size")) {
                // Set the grid size
                InitialiseGrid();
            }
        }
        EditorGUILayout.EndHorizontal();

        // TileGameobject
        tileGameObject = (GameObject)EditorGUILayout.ObjectField("Tile GameObject", tileGameObject, typeof(GameObject));
    }

    void ChunkDesigner() {
        // Prefab details
        EditorGUILayout.BeginVertical();
        {
            leftExit = EditorGUILayout.Toggle("Left Exit", leftExit);
            rightExit = EditorGUILayout.Toggle("Right Exit", rightExit);
            topExit = EditorGUILayout.Toggle("Top Exit", topExit);
            bottomExit = EditorGUILayout.Toggle("Bottom Exit", bottomExit);
            EditorGUILayout.Space();
        }
        EditorGUILayout.EndVertical();

        // Chunk Designer Tools
        EditorGUILayout.BeginHorizontal();
        {
            drawTool = GUILayout.Toggle(drawTool, "Draw tool");
        }
        EditorGUILayout.EndHorizontal();
        // Chunk Tile grid
        EditorGUILayout.Space();

        Rect baseline = GUILayoutUtility.GetLastRect();

        for (int i = 0; i < chunkGrid.GetLength(0); i++) {
            EditorGUILayout.BeginHorizontal();
            {
                for (int j = chunkGrid.GetLength(1) - 1; j >= 0; j--) {
                    Rect toggleRect = new Rect(baseline.xMin + 8 + 32 * i, baseline.yMax + 32 * j, 32, 32);
                    chunkGrid[i, chunkGrid.GetLength(1) - 1 - j] = EditorGUI.Toggle(toggleRect, chunkGrid[i, chunkGrid.GetLength(1) - 1 - j], chunkTileToggleSkin.customStyles[0]);
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        for (int i = 0; i < chunkGrid.GetLength(1) * 5.5; i++) {
            EditorGUILayout.Space();
        }
        EditorGUILayout.Space();

        clearAfter = EditorGUILayout.Toggle("Clear grid after use?", clearAfter);

        EditorGUILayout.BeginHorizontal();
        {
            if (GUILayout.Button("Create chunk")) {
                if (tileGameObject != null) {
                    CreateChunk();
                }
            }

            if (GUILayout.Button("Save chunks as prefabs")) {
                SavePrefabs();
            }

            if (GUILayout.Button("Delete created chunks")) {
                createdChunks.Clear();
            }
        }
        EditorGUILayout.EndHorizontal();

    }

    void CreatedChunksList() {
        List<GameObject> toRemove = new List<GameObject>();

        foreach (GameObject go in createdChunks) {
            if (go != null) {
                GUILayout.Label(go.name, EditorStyles.miniLabel);
            } else {
                toRemove.Add(go);
            }
        }

        foreach (GameObject go in toRemove) {
            createdChunks.Remove(go);
        }

        toRemove.Clear();
    }

    void InitialiseGrid() {
        chunkGrid = new bool[gridSizeX, gridSizeY];
        for (int i = 0; i < chunkGrid.GetLength(0); i++) {
            for (int j = 0; j < chunkGrid.GetLength(1); j++) {
                chunkGrid[i, j] = false;
            }
        }

        leftExit = false;
        rightExit = false;
        topExit = false;
        bottomExit = false;
    }

    void CreateChunk() {
        string name = GetName();
        GameObject chunk = new GameObject(name);

        for (int i = 0; i < chunkGrid.GetLength(0); i++) {
            for (int j = 0; j < chunkGrid.GetLength(1); j++) {

                if (chunkGrid[i, j]) {
                    GameObject tile = Instantiate(tileGameObject, new Vector3(i, j, 0), Quaternion.identity) as GameObject;
                    tile.transform.parent = chunk.transform;
                    tile.name = "Tile (" + i.ToString() + "," + j.ToString() + ")";
                }

            }
        }

        createdChunks.Add(chunk);

        if (clearAfter) {
            InitialiseGrid();
        }
    }

    void SavePrefabs() {
        foreach (GameObject go in createdChunks) {
            Object prefab = PrefabUtility.CreateEmptyPrefab(GetPath(go.name));

            PrefabUtility.ReplacePrefab(go, prefab);
        }
    }

    string GetName() {
        string name = "";

        if (leftExit) {
            name += "L";
        }
        if (rightExit) {
            name += "R";
        }
        if (topExit) {
            name += "U";
        }
        if (bottomExit) {
            name += "D";
        }

        name += (createdChunks.Count + 1).ToString("D2");

        return name;
    }

    string GetPath(string name) {
        string path = "Assets/Assets/Prefabs/Level/ChunkDesigns/";

        int connections = 0;

        bool left = false;
        bool right = false;
        bool up = false;
        bool down = false;

        if (name.Contains("L")) {
            left = true;
            connections++;
        }
        if (name.Contains("R")) {
            right = true;
            connections++;
        }
        if (name.Contains("U")) {
            up = true;
            connections++;
        }
        if (name.Contains("D")) {
            down = true;
            connections++;
        }

        switch (connections) {
            case 0:
                //error
                break;
            case 1:
                path += "1 Connection/";
                break;
            case 2:
                path += "2 Connections/";
                break;
            case 3:
                path += "3 Connections/";
                break;
            case 4:
                path += "4 Connections/";
                break;
        }

        if (left) {
            path += "L";
        }if (right) {
            path += "R";
        }if (up) {
            path += "U";
        }if (down) {
            path += "D";
        }

        path += "/" + name + ".prefab";

        return path;
    }
}
