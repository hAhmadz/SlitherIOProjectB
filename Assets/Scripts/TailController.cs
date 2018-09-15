using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TailController : MonoBehaviour {

    public float speed = 0.1f;
    //private GameObject parent;

    private Transform head;
    private List<Transform> tail;
    private int tailNumber;

    void Start () {
        head = GameObject.FindGameObjectWithTag("Player").gameObject.transform;
        // is this just an object reference, or does it make a copy of the list
        tail = head.GetComponent<PlayerController>().tail;
        tailNumber = tail.IndexOf(transform);
	}

    private Vector2 movementSpeed;
    public float followTime = 0.5f;
	void FixedUpdate () {

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

}
