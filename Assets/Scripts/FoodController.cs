using UnityEngine;

public class FoodController : MonoBehaviour 
{
    public Transform deliciousFood;
    public int boardMin, boardMax;
    public float spawnProbability;

	void FixedUpdate () {
        MakeFood();
	}
    
    void MakeFood() {
        float chance = Random.value;
        if (chance < spawnProbability)
        {
            float xPoint = Random.Range(boardMin, boardMax);
            float yPoint = Random.Range(boardMin, boardMax);
            Vector2 spawnLocation = new Vector2(xPoint, yPoint);
            MakeFood(spawnLocation);
        }
    }

    public void MakeFood(Vector2 spawnLocation) {
        Transform newFood = Instantiate(deliciousFood, spawnLocation, Quaternion.identity);
        newFood.parent = GameObject.Find("Food").transform;
    }
}
