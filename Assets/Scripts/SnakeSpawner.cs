using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeSpawner : MonoBehaviour 
{
    public Transform[] snakePrefabs;
    public int boardMin, boardMax;
    public float spawnProbability;

    void FixedUpdate()
    {
        MakeSnake();
    }


    // with spawnProbability make a new food in a random location within the board
    void MakeSnake()
    {
        print("spawn prob met");
        Transform prefabToSpawn = snakePrefabs[Random.Range(0, snakePrefabs.Length - 1)];

        float chance = Random.value;
        if (chance < spawnProbability)
        {
            float xPoint = Random.Range(boardMin, boardMax);
            float yPoint = Random.Range(boardMin, boardMax);
            Vector2 spawnLocation = new Vector2(xPoint, yPoint);
            MakeSnake(prefabToSpawn, spawnLocation);
        }
    }

    // make a food at a given x,y location
    public void MakeSnake(Transform prefab, Vector2 spawnLocation)
    {
        Transform newSnake = Instantiate(prefab, spawnLocation, Quaternion.identity);
        newSnake.parent = GameObject.Find("AI Snakes").transform;
    }
}