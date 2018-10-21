/*
 * Abstract Base Class for all other SnakeControllers, including Player and AI Snakes.
 * 
 * 
 * 
 * 
 * 
 * 
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SnakeController : MonoBehaviour
{
    public List<Transform> tail = new List<Transform>();
    public Transform tailLink;
    public int startingLength = 5;
    // TODO: enable different snakes to have different skins
    private Sprite skin;
    public SpriteRenderer sprRend;
    public CircleCollider2D hitBox;
    // TODO: figure out constants, and make them instance variables
    private float speed = 0.1f;
    private float scaleFactor = 0.05f;
    public string snakename;




    /**************************************************************************
     * INITIALIZATION AND UPDATES
     **************************************************************************/


    public void Awake()
    {
        sprRend = gameObject.GetComponent<SpriteRenderer>() as SpriteRenderer;
        hitBox = gameObject.GetComponent<CircleCollider2D>() as CircleCollider2D;
        // deactivate collider on awake to avoid null references until your own tail
        hitBox.enabled = false;
    }

    public void Start()
    {
        // give a snake a random name
        snakename = "snake" + Random.Range(0, 9999).ToString(); 

        // snakes start with 1 link already, so grow until you reach start length
        for (int i = 0; i < startingLength - 1; i++)
        {
            GrowSnake();
        }

        SubmitScore();
        // reactivate the head, once tail links have had a chance to find their head
        hitBox.enabled = true;
    }


    public void FixedUpdate()
    {
        IsBoosted();
        RotateAndMove();
    }


    /**************************************************************************
     * GETTING AND SETTING
     **************************************************************************/

    public int GetStartingLength()
    {
        return startingLength;
    }

    public float GetSpeed()
    {
        return speed;
    }

    public void SetSpeed(float newSpeed)
    {
        speed = newSpeed;
    }


    // used to submit our current length to the leaderboard
    public void SubmitScore()
    {
        GameObject.Find("ScorePanel").GetComponent<ScoreController>().SubmitScore(snakename, tail.Count);
    }

    public void RemoveScore()
    {
        GameObject.Find("ScorePanel").GetComponent<ScoreController>().RemoveScore(snakename);
    }


    /**************************************************************************
     * SNAKE LOGIC
     **************************************************************************/


    // needs to be implemented in child classes
    // this is where an AI strategy is implmented
    public abstract void RotateAndMove();


    // figure out what you just ran into
    public void OnTriggerEnter2D(Collider2D other)
    {
        // eat a food and grow a link
        if (other.gameObject.CompareTag("Food"))
        {
            Destroy(other.gameObject);
            GrowSnake();
        }

        // hit a wall and game over
        else if (other.gameObject.CompareTag("Wall"))
        {
            CrashAndBurn();
        }

        // hit another snake tail, you die
        else if (other.gameObject.CompareTag("Tail"))
        {
            // don't kill yourself if you've only hit your own tail
            if (!(other.gameObject.GetComponent<TailController>().GetHead().Equals(transform)))
                CrashAndBurn();
        }
    }

    // GrowSnake handles the changes when a snake's head eats a food. It adds a tail link,
    // and it scales up the size of the sprite's and collision detectors of the head and all tail links
    // by the Scale
    public virtual void GrowSnake()
    {
        // snakes always start with one tail link, so this is not an out of bounds issue
        Vector2 tailPos = tail[tail.Count - 1].position;

        // make a new tail link object and add it to the tail list
        Transform newLink;
        newLink = Instantiate(tailLink, tailPos, Quaternion.identity) as Transform;
        tail.Add(newLink);
        newLink.SetParent(transform.parent);
        newLink.GetComponent<TailController>().SetHead(transform);
        newLink.gameObject.SetActive(true);


        // scale up the size of all the links
        // TODO: abstract out the functionality of growing up / shrinking down
        if (tail.Count > startingLength)
        {
            Vector2 scaleVector = new Vector2(scaleFactor, scaleFactor);
            Vector2 newSize = sprRend.size + scaleVector;
            float newRadius = hitBox.radius + scaleFactor / 2; // divided by two because radius not diameter
            float newFollowTime = tail[0].gameObject.GetComponent<TailController>().followTime + 0.001f; // bigger snake links spread out more
            float newGlowMultiplier = tail[0].gameObject.GetComponent<ParticleSystem>().main.startSizeMultiplier + 0.1f;
            // TODO: figure out proper timeSteps / deltas -- and make them CONSTANTs


            // increase head size
            sprRend.size = newSize;
            // increase eye size
            transform.GetChild(0).GetComponent<SpriteRenderer>().size = newSize;
            hitBox.radius = newRadius;
            // map scaling up changes to the rest of the snake (i.e., its tail)
            foreach (Transform trans in tail)
            {
                trans.gameObject.GetComponent<TailController>().Scale(newSize.x, newFollowTime, newRadius, newGlowMultiplier);
            }
        }

        SubmitScore();
    }

    // when all snakes die they release food. (scatterFactor dictates the random spread from a body object)
    // then they call the KillSnake method which can be specific to certain snakes
    private float scatterFactor = 0.5f;
    public void CrashAndBurn()
    {
        FoodController foodSpawner = GameObject.Find("Food").GetComponent<FoodController>() as FoodController;
        // each tail link spawns a food
        foreach (Transform trans in tail)
        {
            Vector2 scatterPos = new Vector2(Random.Range(-scatterFactor, scatterFactor),
                                           Random.Range(-scatterFactor, scatterFactor));
            Vector2 tailPos = trans.position;
            foodSpawner.MakeFood(tailPos + scatterPos);

            // TODO: make sure destroying the tail link doesn't result in any null references
                //tail.Clear() below fixes this?
            Destroy(trans.gameObject);
        }

        //clears the tail list
        tail.Clear();
        // and the head spawns a food
        foodSpawner.MakeFood(transform.position);

        RemoveScore();

        KillSnake();
    }
    
    // a concrete snake needs to override this to handle "game over" functionality
    public abstract void KillSnake();





    // TODO: work most of the boosting logic into the base class except for the triggering condition
    public virtual void IsBoosted() {}



    // method called whilst the snake is speed boosted to drop off tail links
    // and spawn food
    public void ShrinkSnake()
    {
        // get the last link in the tail
        int shrinkIndex = tail.Count - 1;
        Transform tailToShrink = tail[shrinkIndex];

        // find where to spawn the food
        FoodController foodSpawner = GameObject.Find("Food").GetComponent<FoodController>() as FoodController;
        Vector2 spawnPos = tailToShrink.position;
        foodSpawner.MakeFood(spawnPos);

        // drop the tail link
        tail.RemoveAt(shrinkIndex);
        Destroy(tailToShrink.gameObject);

        // scale down the size of all the links
        if (tail.Count > startingLength)
        {
            // TODO: abstract out the functionality of growing up / shrinking down
            Vector2 scaleVector = new Vector2(scaleFactor, scaleFactor);
            Vector2 newSize = sprRend.size - scaleVector;
            float newRadius = hitBox.radius - scaleFactor / 2; // divided by two because radius, not diameter

            float newFollowTime = tail[0].gameObject.GetComponent<TailController>().followTime - 0.001f; // smaller snake links spread out less 
            float newGlowMultiplier = tail[0].gameObject.GetComponent<ParticleSystem>().main.startSizeMultiplier - 0.1f;
            // float newGlowMultiplier = tail.Count * 1.05f;
            // decrease head size
            sprRend.size = newSize;
            // decrease eye size
            transform.GetChild(0).GetComponent<SpriteRenderer>().size = newSize;
            hitBox.radius = newRadius;
            // map scaling up changes to the rest of the snake (i.e., its tail)
            foreach (Transform trans in tail)
            {
                trans.gameObject.GetComponent<TailController>().Scale(newSize.x, newFollowTime, newRadius, newGlowMultiplier);
            }
        }

        SubmitScore();

    }

}
