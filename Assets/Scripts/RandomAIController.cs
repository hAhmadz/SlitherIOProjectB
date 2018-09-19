/*
 * A very simple AI snake that makes random moves.
 * 
 * 
 * 
 */


using System.Collections.Generic;
using UnityEngine;

public class RandomAIController : SnakeController

{
    // TODO: figure out constants, and make them instance variables
    private int boardMin = -25; 
    private int boardMax = 25;
    private Vector2 currentTarget;


    public new void Awake()
    {
        base.Awake();
        currentTarget = RandomPosition();
    }

    public override void RotateAndMove()
    {
        var chance = Random.value;
        if (chance < 0.01) {
            currentTarget = RandomPosition();

        }

        // ensure target is not too close to the current position
        // TODO: constrain the angles so snakes don't do 180s
        if (Mathf.Abs(currentTarget.x - transform.position.x) < 5 &&
            Mathf.Abs(currentTarget.y - transform.position.y) < 5)
        {
            currentTarget = RandomPosition();
        }

        transform.position = Vector2.MoveTowards(transform.position, currentTarget, GetSpeed());
    }


    Vector2 RandomPosition() {
        return new Vector2(Random.Range(boardMin, boardMax), Random.Range(boardMin, boardMax));
    }


    public override void KillSnake()
    {
        Destroy(gameObject);
    }
}