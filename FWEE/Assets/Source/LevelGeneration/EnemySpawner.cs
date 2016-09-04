using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour {

    public GameObject Enemy;

    public int spawnLimit = 2;
    int spawned = 0;

    public float spawnDelay = 10.0f;
    public float spawnChance = 0.2f;
    float spawnTime;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

        // Spawn enemy
        if (spawned < spawnLimit) {
            spawnTime -= Time.deltaTime;
            if (spawnTime <= 0) {
                if (Random.value <= spawnChance) {
                    SpawnEnemy();
                    Debug.Log("Spawned enemy");
                }
                spawnTime = spawnDelay;
            }
        }
	
	}

    void SpawnEnemy() {
        GameObject enemy = Instantiate(Enemy, transform.position, Quaternion.identity) as GameObject;
    }
}
