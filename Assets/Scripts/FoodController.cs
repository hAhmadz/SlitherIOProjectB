using UnityEngine;

public class FoodController : MonoBehaviour {
    public Transform deliciousFood;
    public int boardMin, boardMax;
    public float spawnProbability;

	// Use this for initialization
	void Start () {
		
	}
	
	void FixedUpdate () {
        MakeFood();
	}


    // with spawnProbability make a new food in a random location within the board
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

    // make a food at a given x,y location
    public void MakeFood(Vector2 spawnLocation) {
        Transform newFood = Instantiate(deliciousFood, spawnLocation, Quaternion.identity);
        newFood.parent = GameObject.Find("Food").transform;
    }
}
