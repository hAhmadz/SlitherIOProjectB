using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class FoodSpawner : NetworkBehaviour
{

    public GameObject foodPrefab;
    public int intitialFoodCount;
    public float spawnProbability;
    public float xRange;
    public float yRange;

    public override void OnStartServer()
    {
        //float xRange = 25.0f;
        //float yRange = 25.0f;
        for (int i = 0; i < intitialFoodCount; i++)
        {
            var spawnPosition = new Vector3(
                Random.Range(-xRange, xRange),
                Random.Range(-yRange, yRange),
                0.0f);

            var spawnRotation = Quaternion.Euler(
                0.0f,
                0.0f,
                0.0f);

            var food = (GameObject)Instantiate(foodPrefab, spawnPosition, spawnRotation);
            NetworkServer.Spawn(food);
        }
    }
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        float chance = Random.value;
        if (chance < spawnProbability)
        {
            //float xPoint = Random.Range(boardMin, boardMax);
            //float yPoint = Random.Range(boardMin, boardMax);
            var spawnPosition = new Vector2(
                Random.Range(-xRange, xRange),
                Random.Range(-yRange, yRange));
            //Vector2 spawnLocation = new Vector2(xPoint, yPoint);
            MakeFood(spawnPosition);
        }
    }

    public void MakeFood(Vector2 spawnLocation)
    {
        //Transform newFood = Instantiate(foodPrefab, spawnLocation, Quaternion.identity);
        var food = (GameObject)Instantiate(foodPrefab, spawnLocation, Quaternion.identity);
        NetworkServer.Spawn(food);
        //newFood.parent = GameObject.Find("Food").transform;
    }
}