using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillerAIController : AIController
{
    Transform targetHead;
    //Vector2 targetLastObserved;

    public override Vector2 FindTarget()
    {
        Vector2 target = GetCurrentTarget();
        Vector3 target3d = new Vector3(target.x, target.y, 0);

        Vector2 intercept;

        if (targetHead != null)
        {
            intercept = targetHead.position + targetHead.forward * 2.5f;

            print("intercept : " + intercept);

            return intercept;
        }

        // acquire new target
        if (targetHead == null)
        {
            targetHead = AcquireTarget();
            return RandomPosition();
        }

        // as a last resort, return a random position
        return RandomPosition();
    }




    public Transform AcquireTarget()
    {
        var chance = Random.value;

        if (chance < 0.33f) // hunt player
        {
            targetHead = HuntPlayer();

        }
        else if (chance < 0.66f) // hunt ai
        {
            targetHead = HuntAI();

        }
        else // hunt food
        {
            targetHead = HuntFood();
        }

        return targetHead;

    }


    Transform HuntPlayer()
    {
        GameObject targetGameObj = GameObject.FindGameObjectWithTag("Player");
        // print(targetGameObj);
        if (targetGameObj == null)
        {
            return null;
        }
        return targetGameObj.transform;
    }



    Transform HuntAI()
    {
        Transform target = null;
        GameObject[] AIHeads = GameObject.FindGameObjectsWithTag("AI");
        foreach (GameObject head in AIHeads)
        {
            if (!(head.transform.Equals(transform)))
            {
                target = head.transform;
                break;
            }
        }
        return target;
    }




    Transform HuntFood()
    {
        float searchDepth = 5.0f;
        GameObject[] food = GameObject.FindGameObjectsWithTag("Food");
        Queue<Transform> closest = new Queue<Transform>();
        while (closest.Count == 0)
        {
            foreach (GameObject obj in food)
            {
                Vector2 objPos = obj.transform.position;
                if (ManhattanDistance(transform.position, objPos) < searchDepth)
                    closest.Enqueue(obj.transform);
            }
            searchDepth *= 2.0f;
        }
        searchDepth /= 2.0f;
        return closest.Dequeue();
    }




    float ManhattanDistance(Vector2 pos1, Vector2 pos2)
    {
        return Mathf.Abs(pos1.x - pos2.x) + Mathf.Abs(pos1.y - pos2.y);
    }

}

