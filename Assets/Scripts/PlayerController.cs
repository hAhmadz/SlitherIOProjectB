using System.Collections;
using UnityEngine;
using UnityEngine.UI;


public class PlayerController : SnakeController
{
    public Text lengthText;
    public Text gameOverText;
    public Camera mainCam;

    private bool boosted = false;


    public new void Start()
    {
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
        if (tail.Count > startingLength)
        {
            // check changes to input
            if (Input.GetMouseButtonDown(0))
                boosted = true;
            if (Input.GetMouseButtonUp(0))
                boosted = false;

            // TODO: i really don't think i want to call this every frame
            if (boosted)
            {
                SetSpeed(0.15f);
                ChangeTailSpeed(2.0f);
                //TODO: get the dropping working
                //StartCoroutine("DropTailLinks");
            }
            else
            {
                SetSpeed(0.1f);
                ChangeTailSpeed(1.0f);
                //StopCoroutine("DropTailLinks");
            }
        }
    }

    IEnumerator DropTailLinks()
    {
        yield return new WaitForSeconds(1.0f);
        if (tail.Count > startingLength)
            ShrinkSnake();
    }

    void ChangeTailSpeed(float boost)
    {
        foreach (Transform t in tail)
        {
            t.gameObject.GetComponent<TailController>().followBoost = boost;
        }
    }


}
