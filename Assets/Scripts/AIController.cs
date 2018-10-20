/*
 * Abstract base for different AI strategies
 * 
 * 
 * 
 */


using System.Collections.Generic;
using UnityEngine;

public abstract class AIController : SnakeController
{
    // board dimensions to pick targets within
    private int boardMin = -25;
    private int boardMax = 25;
    private Vector2 currentTarget;

    public new void Awake()
    {
        base.Awake();
    }

    // call find target and move there
    public override void RotateAndMove()
    {
        currentTarget = FindTarget();
        //transform.rotation = Quaternion.LookRotation(Vector3.forward, currentTarget);
        transform.position = Vector2.MoveTowards(transform.position, currentTarget, GetSpeed());
    }

    public Vector2 GetCurrentTarget()
    {
        return currentTarget;
    }

    // how an AI snake determines where it is moving
    public abstract Vector2 FindTarget();

    // return a random position within the board dimensions,
    // a useful fall back when other strategies fail.
    public Vector2 RandomPosition()
    {
        return new Vector2(Random.Range(boardMin, boardMax),
                           Random.Range(boardMin, boardMax));
    }

    public override void KillSnake()
    {
        Destroy(gameObject);
    }
}