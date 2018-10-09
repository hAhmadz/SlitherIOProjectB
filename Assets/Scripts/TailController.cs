using System.Collections.Generic;
using UnityEngine;

public class TailController : MonoBehaviour {

    public float speed = 0.1f;
    public float followTime = 0.1f;
    public float followBoost = 1.0f;
    private Transform head;
    private List<Transform> tail;
    private int tailNumber;
    private SpriteRenderer sprRend;
    private CircleCollider2D hitBox;




    private void Awake()
    {
        sprRend = gameObject.GetComponent<SpriteRenderer>() as SpriteRenderer;
        hitBox = gameObject.GetComponent<CircleCollider2D>() as CircleCollider2D;

    }

    void Start () 
    {
        if (transform.parent.tag == "Player")
        {
            head = transform.parent;
            tail = head.GetComponent<PlayerSnakeController>().tail;
        } else
        {
            head = transform.parent.GetChild(0);
            tail = head.GetComponent<SnakeController>().tail;
        }


/*        head = transform.parent.GetChild(0);
        try {
            tail = head.GetComponent<SnakeController>().tail;
        } catch
        {
            tail = head.GetComponent<PlayerSnakeController>().tail;
        }*/
        tailNumber = tail.IndexOf(transform);
    }


    public Transform GetHead()
    {
        return head;
    }


    public void SetHead(Transform head) 
    {
        this.head = head;
    }


    private Vector2 movementSpeed;
    void FixedUpdate () 
    {
        // if it is the first tail object, follow the head.
        // otherwise follow the tail object before it in the tail list
        Transform target = head;

        if (tailNumber > 0) 
        {
            target = tail[tailNumber - 1];
        }
            

        transform.position = Vector2.SmoothDamp(transform.position,
                                                target.position,
                                                ref movementSpeed,
                                                followTime*followBoost);
        transform.right = target.position - transform.position;

    }
    
    public void Scale(float newSize, float newFollowTime, float newRadius) 
    {
        sprRend.size = new Vector2(newSize, newSize);
        followTime = newFollowTime;
        hitBox.radius = newRadius;
    }

}
