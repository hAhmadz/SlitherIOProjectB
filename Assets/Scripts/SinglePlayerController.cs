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
    public Button boostButton;
    private bool boosted = false;
    public bool boostButtonPressed;
    public bool boostButtonDown;
    public bool touchBoostPressed;
    public bool touchBoostDown;

    public new void Start()
    {
        // set up player controls and GUI elements
        controls = PersistenceController.persistence.controls;
        SetUpGUI();

        // assign the players chosen skin
        Sprite skinToApply = PersistenceController.persistence.skin;
        sprRend.sprite = skinToApply;
        Debug.Log(tail.Count);
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
            moveVector = (transform.up) * 1.5f;
        
        Vector3 currentPos = transform.position;
        Vector3 targetPos = currentPos + moveVector;

        transform.rotation = Quaternion.LookRotation(Vector3.forward, moveVector);
        transform.position = Vector3.MoveTowards(transform.position, targetPos, GetSpeed());
    }
    
    public void TouchMovement()
    {
        TouchBoost();
        Vector3 mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);
        transform.rotation = Quaternion.LookRotation(Vector3.forward, mousePos);
        transform.position = Vector2.MoveTowards(transform.position, mousePos, GetSpeed());
    }

    public override void KillSnake()
    {
        print(tail.Count);
        PersistenceController.persistence.lastScore = tail.Count;
        print(PersistenceController.persistence.lastScore);
        tail.Clear();

        controlCanvas.enabled = false;
        miniMapCanvas.enabled = false;
        scorePanel.GetComponent<CanvasGroup>().alpha = 0;

        gameOverText.text = "YOU LOSE";

        GameOverController ads = gameObject.GetComponentInParent<GameOverController>();
        ads.WaitAndDisplayAd();

        // deactivate the head
        transform.gameObject.SetActive(false);
    }
    
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
            link.gameObject.GetComponent<TailController>().SetGlow(isBoosted);
    }


    void SetGlowColor(Color glowColor)
    {
        ParticleSystem glow = gameObject.GetComponent<ParticleSystem>();
        var main = glow.main;
        main.startColor = glowColor;
        foreach (Transform link in tail)
            link.gameObject.GetComponent<TailController>().SetGlowColor(glowColor);
    }
    
    IEnumerator DropTailLinks() //drop tails while boosted
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
                break;
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
    
    void ZoomCamera(float zoomFactor)
    {
        float newZoom = mainCam.orthographicSize + zoomFactor;
        mainCam.orthographicSize = Mathf.Lerp(mainCam.orthographicSize, newZoom, 2.0f * Time.deltaTime);
    }

}
