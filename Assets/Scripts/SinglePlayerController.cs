using System.Collections;
using UnityEngine;
using UnityEngine.UI;



public class SinglePlayerController : SnakeController
{
    public Text lengthText;
    public Text gameOverText;
    public Camera mainCam;
    public Canvas controlCanvas;
    public Canvas miniMapCanvas;
    public GameObject scorePanel;
    public Controls controls;
    public Joystick joystick;
    private bool boosted = false;
    public bool boostButtonPressed;
    public bool boostButtonDown;

    public new void Start()
    {
        // set up player controls and GUI elements
        controls = PersistenceController.persistence.controls;
        SetUpGUI();

        // assign the players chosen skin
        Sprite skinToApply = PersistenceController.persistence.skin;
        sprRend.sprite = skinToApply;
        tail[0].gameObject.GetComponent<SpriteRenderer>().sprite = skinToApply;

        // set boost color
        SetGlowColor(skinToApply.texture.GetPixel(40, 40));

        base.Start();
        gameOverText.text = "";
        lengthText.text = "Length: " + GetStartingLength().ToString();
    }

    public void SetUpGUI()
    {
        if (controls == Controls.Joystick)
        {
            controlCanvas.transform.GetChild(0).gameObject.SetActive(true);
        }
        else
        {
            controlCanvas.transform.GetChild(0).gameObject.SetActive(false);
            print(controlCanvas.transform.GetChild(1));
            var boostPos = controlCanvas.transform.GetChild(1).transform.position;
            boostPos.y = 120f;
            controlCanvas.transform.GetChild(1).transform.position = boostPos;
        }
            
    }


    public override void RotateAndMove()
    {
        Vector3 moveVector = Vector3.zero;

        switch (controls)
        {
            case Controls.Touch:
                TouchMovement();
                return;
            case Controls.Joystick:
                moveVector = (Vector3.right * joystick.Horizontal + Vector3.up * joystick.Vertical);
                break;
            case Controls.Accelerometer:
                moveVector = (Vector3.right * Input.acceleration.x + Vector3.up * Input.acceleration.y);
                break;
            default:
                print("fell through to default case in control switch");
                break;
        }

        // ensure the snake keeps moving on zeroed input
        if (moveVector == Vector3.zero)
        {
            moveVector = (transform.up) * 1.5f;
        }

        Vector3 currentPos = transform.position;
        Vector3 targetPos = currentPos + moveVector;

        transform.rotation = Quaternion.LookRotation(Vector3.forward, moveVector);
        transform.position = Vector3.MoveTowards(transform.position, targetPos, GetSpeed());
    }


    // todo: why does this only work if the method is called in the switch statement,
    //       not when moveVector is set to mousePos ???
    public void TouchMovement()
    {
        Vector3 mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);
        transform.rotation = Quaternion.LookRotation(Vector3.forward, mousePos);
        transform.position = Vector2.MoveTowards(transform.position, mousePos, GetSpeed());
    }



    // for a player snake, death is slighlty more prolonged
    public override void KillSnake() 
    {
        // deactivate heads up display canvases / panels
        controlCanvas.enabled = false;
        miniMapCanvas.enabled = false;
        scorePanel.GetComponent<CanvasGroup>().alpha = 0;


        gameOverText.text = "YOU LOSE";


        AdvertisementController ads = gameObject.GetComponentInParent<AdvertisementController>();
        ads.WaitAndDisplayAd();

        // deactivate the head
        transform.gameObject.SetActive(false);

        //Functionality to jump to start menu when game Over
        //UnityEngine.SceneManagement.SceneManager.LoadScene("Menu");
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
        else
            Handheld.Vibrate();

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
