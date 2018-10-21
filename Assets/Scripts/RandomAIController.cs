using UnityEngine;

/*
 * A very simple AI snake that makes random moves.
 */

public class RandomAIController : AIController
{
    // A Random snake's target is a random position not too close to its head
    public override Vector2 FindTarget()
    {
        Vector2 target = GetCurrentTarget();
        if (target.Equals(null))
            target = RandomPosition();
        
        var chance = Random.value;
        if (chance < 0.01)
            target = RandomPosition();
        
        if (Mathf.Abs(target.x - transform.position.x) < 5 &&
            Mathf.Abs(target.y - transform.position.y) < 5)
        {
            target = RandomPosition();
        }

        return target;
    }
}