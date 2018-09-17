using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AITailController : MonoBehaviour
{

    public float speed = 0.1f;
    //private GameObject parent;

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

    void Start()
    {
        head = GameObject.FindGameObjectWithTag("AI").gameObject.transform;
        // is this just an object reference, or does it make a copy of the list
        tail = head.GetComponent<AIController>().tail;
        tailNumber = tail.IndexOf(transform);
        sprRend = gameObject.GetComponent<SpriteRenderer>() as SpriteRenderer;
        hitBox = gameObject.GetComponent<CircleCollider2D>() as CircleCollider2D;

    }

    private Vector2 movementSpeed;
    public float followTime = 0.1f;
    void FixedUpdate()
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
                                                followTime);
        transform.right = target.position - transform.position;
    }


    public void ScaleUp(float newSize, float newFollowTime, float newRadius)
    {
        sprRend.size = new Vector2(newSize, newSize);
        followTime = newFollowTime;
        hitBox.radius = newRadius;
    }

}
