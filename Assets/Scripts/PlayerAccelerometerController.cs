using System.Collections;
using UnityEngine;
using UnityEngine.UI;


public class PlayerAccelerometerController : SnakeController
{
    public Text lengthText;
    public Text gameOverText;
    //public Button restartBtn;
    public Camera mainCam;
    private bool boosted = false;
    public bool boostButtonPressed;
    public bool boostButtonDown;

    public new void Start()
    {
        // assign the players chosen skin
        Sprite skinToApply = PersistenceController.persistence.skin;
        sprRend.sprite = skinToApply;
        tail[0].gameObject.GetComponent<SpriteRenderer>().sprite = skinToApply;

        // set boost color
        SetGlowColor(skinToApply.texture.GetPixel(40, 40));

        base.Start();
        gameOverText.text = "";
        lengthText.text = "Length: " + GetStartingLength().ToString();
        //restartBtn.gameObject.SetActive(false);
    }


    // TODO: i'm not entirely sure if respawning the player is strictly necessary?
    //used to restart the game (i.e. revive the player)
    public void Restart()
    {
        //restartBtn.gameObject.SetActive(false);
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
        //Vector3 moveVector = (Vector3.right * joystick.Horizontal + Vector3.up * joystick.Vertical);
        //if (moveVector == Vector3.zero)
        //{
        //    moveVector = (transform.up) * 1.5f;
        //}

        //Vector3 currentPos = transform.position;
        //Vector3 targetPos = currentPos + moveVector;

        //transform.rotation = Quaternion.LookRotation(Vector3.forward, moveVector);
        //transform.position = Vector3.MoveTowards(transform.position, targetPos, GetSpeed());


        Vector3 moveVector = (Vector3.right * Input.acceleration.x + Vector3.up * Input.acceleration.y);
        if (moveVector == Vector3.zero)
        {
            moveVector = (transform.up) * 1.5f;
        }

        Vector3 currentPos = transform.position;
        Vector3 targetPos = currentPos + moveVector;

        transform.rotation = Quaternion.LookRotation(Vector3.forward, moveVector);
        transform.position = Vector3.MoveTowards(transform.position, targetPos, GetSpeed());

        // transform.Translate(Input.acceleration.x, 0, -Input.acceleration.z);
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


        //restartBtn.gameObject.SetActive(true);
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



    // joystick boost function is called by a button press
    public void PressBoost(bool isPressed)
    {
        boostButtonPressed = isPressed;
        boostButtonDown = isPressed;
    }



    public override void IsBoosted()
    {
        // if the snake is greater than the minimum length and the button was 
        // pressed this frame, initiate dropping links
        if (tail.Count > startingLength && boostButtonPressed)
        {
            boosted = true;
            SetSpeed(0.15f);
            ChangeTailFollowBoost(0.75f);
            StartCoroutine(DropTailLinks());
            boostButtonPressed = false;
        }

        // if the button is not pressed or the snake is not longer than the 
        // minimum length, stop dropping links
        if (tail.Count <= startingLength || !boostButtonDown)
        {
            boosted = false;
            SetSpeed(0.1f);
            ChangeTailFollowBoost(1.0f);
        }

        if (!boosted)
            StopCoroutine(DropTailLinks());

        SetGlow(boosted);
    }

    void SetGlow(bool isBoosted)
    {
        ParticleSystem glow = gameObject.GetComponent<ParticleSystem>();
        var em = glow.emission;
        em.enabled = isBoosted;

        foreach (Transform link in tail)
        {
            link.gameObject.GetComponent<TailController>().SetGlow(isBoosted);
        }
    }

    void SetGlowColor(Color glowColor)
    {
        ParticleSystem glow = gameObject.GetComponent<ParticleSystem>();
        var main = glow.main;
        main.startColor = glowColor;

        foreach (Transform link in tail)
        {
            link.gameObject.GetComponent<TailController>().SetGlowColor(glowColor);
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

