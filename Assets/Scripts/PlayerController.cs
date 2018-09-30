using System.Collections;
using UnityEngine;
using UnityEngine.UI;


public class PlayerController : SnakeController
{
    public Text lengthText;
    public Text gameOverText;
    public Button restartBtn;
    public Camera mainCam;

    private bool boosted = false;


    public new void Start()
    {
        base.Start();
        gameOverText.text = "";
        lengthText.text = "Length: " + GetStartingLength().ToString();
        restartBtn.gameObject.SetActive(false);
    }


    //used to restart the game (i.e. revive the player)
    public void Restart()
    {
        restartBtn.gameObject.SetActive(false);
        transform.gameObject.SetActive(true);
        transform.position = new Vector2(10, 10);
        
        //tentative fix to scaling and colission radius
        //scaling fine, and radius may be too (but not sure how accurate)
        sprRend.size = new Vector2(1,1);
        hitBox.radius = 0.5f;

        //add first tail link...
        Vector2 tailPos = transform.position;

        // make a new tail link object and add it to the tail list
        Transform newLink;
        newLink = Instantiate(tailLink, tailPos, Quaternion.identity) as Transform;
        tail.Add(newLink);
        newLink.SetParent(transform.parent);
        newLink.GetComponent<TailController>().SetHead(transform);
        newLink.gameObject.SetActive(true);

        base.Start();
        gameOverText.text = "";
        lengthText.text = "Length: " + GetStartingLength().ToString();

    }



    public override void RotateAndMove()
    {
        // rotate head (... TODO: something not right with this)
        Vector3 mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);
        float angle = Mathf.Atan2(mousePos.x, mousePos.y) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward * -1); // -1 for inverted z-axis

        // move head
        transform.position = Vector2.MoveTowards(transform.position, mousePos, GetSpeed());
    }


    // for a player snake, death is slighlty more prolonged
    public override void KillSnake() 
    {
        // TODO: game over
        // TODO: ADVERTISEMENT
        gameOverText.text = "YOU LOSE";
        transform.gameObject.SetActive(false);
        restartBtn.gameObject.SetActive(true);
    }
            

    // Player snake growth needs to handle zooming out the camera, and updating the score text
    public override void GrowSnake() 
    {
        base.GrowSnake();
        if (tail.Count > GetStartingLength())
        {
            // zoom out
            var newZoom = mainCam.orthographicSize + 1.0f;
            mainCam.orthographicSize = Mathf.Lerp(mainCam.orthographicSize, newZoom, 2.0f * Time.deltaTime); ;
        }

        lengthText.text = "Length: " + tail.Count.ToString();
    }


    public override void IsBoosted()
    {

        // if the snake is greater than the minimum length and the mouse was 
        // pressed this frame, initiate dropping links
        if (tail.Count > startingLength && Input.GetMouseButtonDown(0))
        {
            boosted = true;
            SetSpeed(0.15f);
            ChangeTailFollowBoost(0.75f);
            StartCoroutine("DropTailLinks");
        }

        // if the mouse was released this frame or the snake is not longer 
        // than the minimum length, stop dropping links
        if (tail.Count <= startingLength || Input.GetMouseButtonUp(0))
        {
            boosted = false;
            SetSpeed(0.1f);
            ChangeTailFollowBoost(1.0f);
        }

        if (!boosted)
            StopCoroutine("DropTailLinks");
    }

    IEnumerator DropTailLinks()
    {
        yield return new WaitForSeconds(0.66f);
        if (tail.Count > startingLength)
            ShrinkSnake();
    }

    void ChangeTailFollowBoost(float boost)
    {
        foreach (Transform t in tail)
        {
            t.gameObject.GetComponent<TailController>().followBoost = boost;
        }
    }


}
