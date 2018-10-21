﻿using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;



public class MultiPlayerController : SnakeController
{
    public Text lengthText;
    public Text gameOverText;
    //public Camera mainCam;
    public Canvas controlCanvas;
    public Canvas miniMapCanvas;
    public GameObject scorePanel;
    public Controls controls;
    public Joystick joystick;
    public Button boostButton;
    private bool boosted = false;
    public bool boostButtonPressed;
    public bool boostButtonDown;
    public bool touchBoostPressed;
    public bool touchBoostDown;

    public new void Start()
    {

        gameOverText = GameObject.FindWithTag("GameOver").GetComponent<Text>();
        lengthText = GameObject.FindWithTag("Length").GetComponent<Text>();
        controlCanvas = GameObject.FindWithTag("Control").GetComponent<Canvas>();
        miniMapCanvas = GameObject.FindWithTag("MiniMap").GetComponent<Canvas>();
        scorePanel = GameObject.FindWithTag("Score").GetComponent<GameObject>();
        joystick = GameObject.FindWithTag("Fixed").GetComponent<Joystick>();
        boostButton = GameObject.FindWithTag("Boost").GetComponent<Button>();

        // set up player controls and GUI elements
        controls = PersistenceController.persistence.controls;
        SetUpGUI();

        // assign the players chosen skin
        Sprite skinToApply = PersistenceController.persistence.skin;
        sprRend.sprite = skinToApply;
        Debug.Log(tail[0] == null);
        Debug.Log("hi");
        tail[0].gameObject.GetComponent<SpriteRenderer>().sprite = skinToApply;

        // set boost color
        SetGlowColor(skinToApply.texture.GetPixel(40, 40));

        base.Start();

        // override randomly assigned name if user has chosen their own
        if (PersistenceController.persistence.snakename != "")
        {
            RemoveScore(); // remove the random name from the leader board
            snakename = PersistenceController.persistence.snakename;
            if (snakename.Length > 9) // truncate too long snake names
                snakename = snakename.Substring(0, 9) + "...";
            SubmitScore(); // add your newly assigned name
        }

        gameOverText.text = "";
        lengthText.text = "Length: " + GetStartingLength().ToString();
    }



    public void SetUpGUI()
    {
        if (controls == Controls.Touch)
            controlCanvas.enabled = false;
        else if (controls == Controls.Joystick)
        {
            controlCanvas.transform.GetChild(0).gameObject.SetActive(true);
        }
        else // controls == Controls.Accelerometer
        {
            controlCanvas.transform.GetChild(0).gameObject.SetActive(false);
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


    // Unity doesn't like it when the call to mainCam.ScreenPointToWorld is put 
    // into the above function, so it geths a method of its own.
    public void TouchMovement()
    {
        TouchBoost();
        // we're not clicking on a UI object, so do your normal movement stuff here
        /*Vector3 mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);
        transform.rotation = Quaternion.LookRotation(Vector3.forward, mousePos);
        transform.position = Vector2.MoveTowards(transform.position, mousePos, GetSpeed());*/
        transform.rotation = transform.parent.rotation;
        transform.position = transform.parent.position;
    }


    // for a player snake, death is slighlty more prolonged
    public override void KillSnake()
    {
        print(tail.Count);
        PersistenceController.persistence.lastScore = tail.Count;
        print(PersistenceController.persistence.lastScore);
        //clears the tail list
        tail.Clear();

        // deactivate heads up display canvases / panels
        controlCanvas.enabled = false;
        miniMapCanvas.enabled = false;
        scorePanel.GetComponent<CanvasGroup>().alpha = 0;

        gameOverText.text = "YOU LOSE";

        GameOverController ads = gameObject.GetComponentInParent<GameOverController>();
        ads.WaitAndDisplayAd();

        // deactivate the head
        transform.gameObject.SetActive(false);
    }



    // Player snake growth needs to handle zooming out the camera, and updating the score text
    public override void GrowSnake()
    {
        base.GrowSnake();
        // zoom out
        if (tail.Count > GetStartingLength())
        {
            PlayerNetworkController parentScript = this.transform.parent.GetComponent<PlayerNetworkController>();
            parentScript.ZoomCamera(1.0f);
            //ZoomCamera(1.0f);
        }
        lengthText.text = "Length: " + tail.Count.ToString();
    }



    // joystick and accelerometer boost function is called by a button press
    public void PressBoost(bool isPressed)
    {
        boostButtonPressed = isPressed;
        boostButtonDown = isPressed;
    }


    // touch control boost is activated by a double tap
    public void TouchBoost()
    {
        foreach (Touch t in Input.touches)
        {
            if (t.tapCount > 1)
            {
                boostButtonPressed = (t.phase == TouchPhase.Began);
                boostButtonDown = true;
                return;
            }
        }
        boostButtonPressed = false;
        boostButtonDown = false;
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
                PlayerNetworkController parentScript = this.transform.parent.GetComponent<PlayerNetworkController>();
                parentScript.ZoomCamera(-1.0f);
                //ZoomCamera(-1.0f);
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
    /*void ZoomCamera(float zoomFactor)
    {
        float newZoom = mainCam.orthographicSize + zoomFactor;
        mainCam.orthographicSize = Mathf.Lerp(mainCam.orthographicSize, newZoom, 2.0f * Time.deltaTime);
    }*/

}