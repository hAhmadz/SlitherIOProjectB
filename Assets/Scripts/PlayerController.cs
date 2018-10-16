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


    // TODO: i'm not entirely sure if respawning the player is strictly necessary?
    //used to restart the game (i.e. revive the player)
    public void Restart()
    {
        restartBtn.gameObject.SetActive(false);
        transform.gameObject.SetActive(true);
        transform.position = new Vector2(10, 10);

        //tentative fix to scaling and colission radius
        //scaling fine, and radius may be too (but not sure how accurate)
        sprRend.size = Vector2.one;
        hitBox.radius = 0.5f;

        // bit of a hack to reorient the camera
        while (mainCam.orthographicSize > 10.0f)
        {
            ZoomCamera(-1.0f);
        }

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
        gameOverText.text = "YOU LOSE";


        AdvertisementController ads = gameObject.GetComponentInParent<AdvertisementController>();
        ads.WaitAndDisplayAd();

        // deactivate the head
        transform.gameObject.SetActive(false);

        //Functionality to jump to start menu when game Over
        //UnityEngine.SceneManagement.SceneManager.LoadScene("Menu");


         restartBtn.gameObject.SetActive(true);
    }


    // Player snake growth needs to handle zooming out the camera, and updating the score text
    public override void GrowSnake() 
    {
        base.GrowSnake();
        // zoom out
        if (tail.Count > GetStartingLength())
        {
            ZoomCamera(1.0f);
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
            StartCoroutine(DropTailLinks());
        }

        // if the mouse is not pressed or the snake is not longer than the 
        // minimum length, stop dropping links
        if (tail.Count <= startingLength || !(Input.GetMouseButton(0)))
        {
            boosted = false;
            SetSpeed(0.1f);
            ChangeTailFollowBoost(1.0f);
        }

        if (!boosted)
            StopCoroutine(DropTailLinks());

        SetGlow(boosted);
    }

    void SetGlow(bool isBoosted) {
        ParticleSystem glow = gameObject.GetComponent<ParticleSystem>();
        var em = glow.emission;
        em.enabled = isBoosted;

        foreach (Transform link in tail)
        {
            link.gameObject.GetComponent<TailController>().SetGlow(isBoosted);
        }
            
    }

    // a thread to drop tail links while boosted
    IEnumerator DropTailLinks()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.9f);

            if (boosted && tail.Count > startingLength)
            {
                ShrinkSnake();
                ZoomCamera(-1.0f);
                lengthText.text = "Length: " + tail.Count.ToString();
            }
            else
            {
                break;
            }
        }
        StopCoroutine(DropTailLinks());
    }



    void ChangeTailFollowBoost(float boost)
    {
        foreach (Transform t in tail)
        {
            t.gameObject.GetComponent<TailController>().followBoost = boost;
        }
    }



    /* method to adjust the main camera's orthographic size */
    void ZoomCamera(float zoomFactor) 
    {
        float newZoom = mainCam.orthographicSize + zoomFactor;
        mainCam.orthographicSize = Mathf.Lerp(mainCam.orthographicSize, newZoom, 2.0f * Time.deltaTime);
    }

}
