using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{

    public float speed = 0.1f;

    private Vector3 mousePosition;
    //public float moveSpeed = 0.1f;

    public int length;
    public Text lengthText;

    private Rigidbody2D rb;
    public List<Transform> tail = new List<Transform>();

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        /*foreach (Transform child in gameObject.transform)
        {
            if (child.tag == "Tail")
            {
                tail.Add(child);
            }
        }*/
    }

    private void FixedUpdate()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");
        //Vector2 movement = new Vector2(moveHorizontal, moveVertical);


            /*if (Input.GetMouseButton(1))
            {*/
        mousePosition = Input.mousePosition;
        mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
        transform.position = Vector2.MoveTowards(transform.position, mousePosition, speed);
            //}
    }


        //rb.AddForce(movement * speed);

        // MoveTail();
    //}

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Food"))
        {
            other.gameObject.SetActive(false);
            GrowTail();
            //length++;
            //lengthText.text = "Length: " + length.ToString();
        }
    }



    void GrowTail() {
        //tail.Add(new GameObject("tail"));
        length++;
        lengthText.text = "Length: " + length.ToString();
    }

    /*
    void MoveTail() {
        Follow(this.gameObject, tail[0]);
        for (int i = 1; i < length-1; i++) {
            Follow(tail[i], tail[i + 1]);
        }
    }*/


    /*void Follow(GameObject leader, GameObject follower) {
        // The step size is equal to speed times frame time.
        float step = speed * Time.deltaTime;

        // Move follower a step closer to the target.
        follower.transform.position =
            Vector3.MoveTowards(follower.transform.position, leader.transform.position, step);
    }*/
}
