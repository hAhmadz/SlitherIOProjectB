using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour

{
    // TODO: figure out constants, and make them instance variables

    public float speed = 0.1f;
    public int startingLength = 5;
    public List<Transform> tail = new List<Transform>();
    public Transform tailLink;
    private SpriteRenderer sprRend;
    private CircleCollider2D hitBox;
    private float scaleFactor = 0.05f;
    private Vector2 scaleVector;
    public int boardMin, boardMax;
    private Vector2 currentTarget;


    private void Awake()
    {
        sprRend = gameObject.GetComponent<SpriteRenderer>() as SpriteRenderer;
        hitBox = gameObject.GetComponent<CircleCollider2D>() as CircleCollider2D;
        currentTarget = RandomPosition();
    }

    void Start()
    {
        scaleVector = new Vector2(scaleFactor, scaleFactor);
        for (int i = 0; i < startingLength - 1; i++)
            GrowTail();
    }

    public void FixedUpdate()
    {
        // for now the snake moves randomly
        var chance = Random.value;
        if (chance < 0.01) {
            currentTarget = RandomPosition();

        }

        // ensure target is not too close to the current position
        if (Mathf.Abs(currentTarget.x - transform.position.x) < 5 &&
            Mathf.Abs(currentTarget.y - transform.position.y) < 5)
        {
            currentTarget = RandomPosition();
        }

        transform.position = Vector2.MoveTowards(transform.position, currentTarget, speed);
    }


    void OnTriggerEnter2D(Collider2D other)
    {
        // eat a food and grow a link
        if (other.gameObject.CompareTag("Food"))
        {
            Destroy(other.gameObject);
            GrowTail();
        }

        // hit a wall and game over
        else if (other.gameObject.CompareTag("Wall"))
        {
            CrashAndBurn();
        }

        // hit the player snake tail, you die
        else if (other.gameObject.CompareTag("Tail"))
        {
            CrashAndBurn();
        }
    }



    // when a snake dies it releases food
    void CrashAndBurn()
    {
        FoodController foodSpawner = GameObject.Find("Food").GetComponent<FoodController>() as FoodController;

        foreach (Transform trans in tail)
        {
            Vector2 deltaPos = new Vector2(Random.Range(-0.5f, 0.5f),
                                           Random.Range(-0.5f, 0.5f));
            Vector2 tailPos = trans.position;
            foodSpawner.MakeFood(tailPos + deltaPos);
            trans.gameObject.SetActive(false);
            //Destroy(trans.gameObject); // TODO : issue with destroying in the list?
        }

        foodSpawner.MakeFood(transform.position);

        Destroy(gameObject);
    }



    void GrowTail()
    {

        var tailPos = tail[tail.Count - 1].position; // starting with one tail link in the snake (so don't need the if check)


        Transform newLink = Instantiate(tailLink, tailPos, Quaternion.identity) as Transform;
        newLink.parent = transform.parent;
        tail.Add(newLink);

        // scale up (very broken at the moment)
        if (tail.Count > 5)
        {
            Vector2 newSize = sprRend.size + scaleVector;
            float newRadius = hitBox.radius + scaleFactor / 2; // divided by two because radius not diameter
            // bigger snake links spread out more
            // TODO: figure out proper timeSteps / deltas
            float newFollowTime = tail[0].gameObject.GetComponent<AITailController>().followTime + 0.001f;

           
            sprRend.size = newSize;
            hitBox.radius = newRadius;

            // map scaling up changes to the rest of the snake (i.e., its tail)
            foreach (Transform trans in tail)
            {
                trans.gameObject.GetComponent<AITailController>().ScaleUp(newSize.x, newFollowTime, newRadius);
            }

        }

    }


    Vector2 RandomPosition() {
        return new Vector2(Random.Range(boardMin, boardMax), Random.Range(boardMin, boardMax));
    }
}