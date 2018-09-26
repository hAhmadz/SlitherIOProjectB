using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillerAIController : AIController
{
    Transform targetHead;

    public override Vector2 FindTarget()
    {
        Vector2 target = GetCurrentTarget();
        if (target.Equals(Vector2.zero) || transform.position.Equals(target))
        {
            var chance = Random.value;
            print("chance : " + chance.ToString());
            if (chance < 0.33f) // hunt player
            {
                targetHead = GameObject.FindGameObjectWithTag("Player").transform;
                if (targetHead.Equals(null)) { return FindTarget(); }
            }
            else // hunt ai
            {
                GameObject[] AIHeads = GameObject.FindGameObjectsWithTag("AI");
                foreach (GameObject head in AIHeads)
                {
                    if (!(head.transform.Equals(transform)))
                    {
                        targetHead = head.transform;
                        break;
                    }
                }
                if (targetHead.Equals(null)) { return FindTarget(); }
            }

        }
        Vector2 myPos = transform.position;
        Vector2 diff = myPos - target;

        if (Mathf.Abs(diff.x) < 1 && Mathf.Abs(diff.x) < 1)
            return diff * -1;
        return targetHead.position;
    }
}
       
