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
        if (targetHead == null || 
              target.Equals(Vector2.zero) || 
              transform.position.Equals(target))
        {

            var chance = Random.value;
            print("chance : " + chance.ToString());
            if (chance < 0.33f) // hunt player
            {
                GameObject targetGameObj = GameObject.FindGameObjectWithTag("Player");
                print(targetGameObj);
                if (targetGameObj == null) { return RandomPosition(); }
                targetHead = targetGameObj.transform;
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
                if (targetHead == null) { return RandomPosition(); }

            }

        }

        Vector2 myPos = transform.position;
        Vector2 diff = myPos - target;

        if (Mathf.Abs(diff.x) < 1 && Mathf.Abs(diff.x) < 1)
            return diff * -1;

        Vector2 targetPos = targetHead.position;
        //Vector2 delta =  targetPos - targetLastObserved;
        Vector2 intercept = targetPos * 1.5f;

        //targetLastObserved = intercept;
        return intercept;
        //return targetHead.position;
    }
}
       
