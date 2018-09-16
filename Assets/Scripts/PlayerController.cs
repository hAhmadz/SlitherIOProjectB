using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PlayerController : MonoBehaviour
{
    // TODO: figure out constants, and make them instance variables

    public float speed = 0.1f;
    public int startingLength = 5;
    public List<Transform> tail = new List<Transform>();
    public Text lengthText;
    public Camera mainCam;
    public Transform tailLink;
    private Vector3 mousePosition;
    private SpriteRenderer sprRend;
    private CircleCollider2D hitBox;
    private float scaleFactor = 0.05f;
    private Vector2 scaleVector;


    void Start()
    {
        scaleVector = new Vector2(scaleFactor, scaleFactor);
        sprRend = gameObject.GetComponent<SpriteRenderer>() as SpriteRenderer;
        //sprRend.drawMode = SpriteDrawMode.Sliced;

        hitBox = gameObject.GetComponent<CircleCollider2D>() as CircleCollider2D;



        lengthText.text = "Length: ---";
        // players start with 1 link already, so grow until you reach starting length
        for (int i = 0; i < startingLength - 1; i++) {
            GrowTail();
        }
    }

    public void FixedUpdate()
    {
        // rotate head (... something not right with this)
        var mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);
        var angle = Mathf.Atan2(mousePos.x, mousePos.y) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward * -1); // -1 for inverted z-axis

        // move head
        transform.position = Vector2.MoveTowards(transform.position, mousePos, speed);
    }


    void OnTriggerEnter2D(Collider2D other)
    {
        // eat a food and grow a link
        if (other.gameObject.CompareTag("Food"))
        {
            Destroy(other.gameObject);
            GrowTail();
        }

        // hit a wall (... or other snake) and game over

    }



    void GrowTail() {

        var tailPos = tail[tail.Count - 1].position; // starting with one tail link in the snake (so don't need the if check)

        Transform newLink = Instantiate(tailLink, tailPos, Quaternion.identity) as Transform;
        tail.Add(newLink);

        // scale up (very broken at the moment)
        if (tail.Count > 5)
        {
            Vector2 newSize = sprRend.size + scaleVector;
            float newRadius = hitBox.radius + scaleFactor / 2; // divided by two because radius not diameter
            // bigger snake links spread out more
            // TODO: figure out proper timeSteps / deltas
            float newFollowTime = tail[0].gameObject.GetComponent<TailController>().followTime + 0.001f;


            sprRend.size = newSize;
            hitBox.radius = newRadius;

            // map scaling up changes to the rest of the snake (i.e., its tail)
            foreach (Transform trans in tail) {
                trans.gameObject.GetComponent<TailController>().ScaleUp(newSize.x, newFollowTime, newRadius);
                //trans.gameObject.GetComponent<SpriteRenderer>().size = newSize;
                //trans.gameObject.GetComponent<TailController>().followTime = newFollowTime;
                //trans.gameObject.GetComponent<TailController>().hitBox.radius += scaleFactor / 2;
            }

            // zoom out
            var newZoom = mainCam.orthographicSize + 1.0f;
            mainCam.orthographicSize = Mathf.Lerp(mainCam.orthographicSize, newZoom, 2.0f * Time.deltaTime); ;
        }

        lengthText.text = "Length: " + tail.Count.ToString();
    }
}
