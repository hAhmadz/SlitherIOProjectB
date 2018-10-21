
using System.Collections.Generic;
using UnityEngine;

public class GreedyAIController : AIController
{
    float searchDepth = 5.0f;
    [SerializeField]
    GameObject[] food;
    [SerializeField]
    Queue<Vector2> closestFood;

    // A greedy snake looking for a target just takes the first food it comes across closer than a 
    // certain distance threshold and goes for that
    public override Vector2 FindTarget()
    {
        Vector2 target = GetCurrentTarget();
        Vector3 target3d = new Vector3(target.x, target.y, 0);

        // if targeting at the origin, or you've reached your target, acquire a new one.
        if (target.Equals(Vector2.zero) || transform.position == target3d)
        {
            food = GameObject.FindGameObjectsWithTag("Food");
            if (closestFood == null || closestFood.Count == 0)
                closestFood = GetClosestPositions(food);
            target = closestFood.Dequeue();
        }
        return target;
    }

    // return a queue of object positions within the search depth
    // increase the search depth until at least one can be found.
    Queue<Vector2> GetClosestPositions(GameObject[] objects)
    {
        Queue<Vector2> closest = new Queue<Vector2>();
        while (closest.Count == 0)
        {
            foreach (GameObject obj in objects)
            {
                Vector2 objPos = obj.transform.position;
                if (ManhattanDistance(transform.position, objPos) < searchDepth)
                    closest.Enqueue(objPos);
            }
            searchDepth *= 2.0f;
        }
        searchDepth /= 2.0f;

        closest.Enqueue(RandomPosition());
        return closest;
    }
    
    float ManhattanDistance(Vector2 pos1, Vector2 pos2)
    {
        return Mathf.Abs(pos1.x - pos2.x) + Mathf.Abs(pos1.y - pos2.y);
    }
}