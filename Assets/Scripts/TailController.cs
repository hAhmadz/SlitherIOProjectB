using System.Collections.Generic;
using UnityEngine;

public class TailController : MonoBehaviour
{

    public float speed = 0.1f;
    public float followTime = 0.1f;
    public float followBoost = 1.0f;
    private Transform head;
    private List<Transform> tail;
    private int tailNumber;
    private SpriteRenderer sprRend;
    private CircleCollider2D hitBox;
    private ParticleSystem glow;


    private void Awake()
    {
        sprRend = gameObject.GetComponent<SpriteRenderer>() as SpriteRenderer;
        hitBox = gameObject.GetComponent<CircleCollider2D>() as CircleCollider2D;
        glow = gameObject.GetComponent<ParticleSystem>();
    }

    void Start()
    {
        head = transform.parent.GetChild(0);
        tail = head.GetComponent<SnakeController>().tail;
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
                                                followTime * followBoost);
        transform.right = target.position - transform.position;

    }

    public void Scale(float newSize, float newFollowTime, float newRadius, float newGlowMultiplier)
    {
        sprRend.size = new Vector2(newSize, newSize);
        followTime = newFollowTime;
        hitBox.radius = newRadius;
        //ParticleSystem glow = gameObject.GetComponent<ParticleSystem>();
        var glowmain = glow.main;
        glowmain.startSizeMultiplier = newGlowMultiplier;
    }

    public void SetGlow(bool isBoosted)
    {
        //ParticleSystem glow = gameObject.GetComponent<ParticleSystem>();
        var em = glow.emission;
        em.enabled = isBoosted;
    }
}
