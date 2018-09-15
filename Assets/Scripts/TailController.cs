using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TailController : MonoBehaviour {

    public float speed = 0.1f;
    //private GameObject parent;

    private Transform head;
    private int tailNumber;



    // Use this for initialization
    void Start () {
        head = GameObject.FindGameObjectWithTag("Player").gameObject.transform;
        // better way to express this?
        // use, find or findIndex...
        for (int i = 0; i < head.GetComponent<PlayerController>().tail.Count; i++) {
            if(gameObject == head.GetComponent<PlayerController>().tail[i].gameObject) {
                tailNumber = i;
            }
        }

        // parent = gameObject.transform.parent.gameObject;
	}

    private Vector2 movementSpeed;
    public float timeStep = 0.5f;
	void FixedUpdate () {
        //Follow(parent, gameObject);

        // if it is the first tail object, follow the head.
        // otherwise follow the tail object before it in the tail list
        Transform target = head;
        if (tailNumber > 0)
        {
            target = head.GetComponent<PlayerController>().tail[tailNumber - 1];
        }

        transform.position = Vector2.SmoothDamp(transform.position, 
                                                target.position, 
                                                ref movementSpeed, 
                                                timeStep);
        // transform.LookAt(target.position);
    }

    /*

    void MoveTail()
    {
        Follow(gameObject, tail[0]);
        for (int i = 1; i < length - 1; i++)
        {
            Follow(tail[i], tail[i + 1]);
        }
    }


    void Follow(GameObject leader, GameObject follower)
    {
        // The step size is equal to speed times frame time.
        float step = speed * Time.deltaTime;

        // Move follower a step closer to the target.
        follower.transform.position =
            Vector3.MoveTowards(follower.transform.position, leader.transform.position, step);
    }
    */


}
