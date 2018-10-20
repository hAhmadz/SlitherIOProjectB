using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerNetworkController : NetworkBehaviour {

    public GameObject player;
    public Camera mainCam;

    //leave camera in player class for the moment, but should move here?
    //use find player tag object instead of get child?
    public override void OnStartLocalPlayer() {
        //Debug.Log("log");
        player = gameObject.transform.GetChild(0).gameObject;
        // mainCam = GetComponentInChildren<Camera>();
        //Debug.Log("befoer cam");
        Camera.main.GetComponent<CameraController>().setTarget(gameObject.transform);
        mainCam = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();

        //player.gameObject.setCamera(mainCam);
        //player.mainCam = mainCam;

    }

    // Use this for initialization
    /*void Start () {
        Debug.Log("stlog");
        player = gameObject.transform.GetChild(0).gameObject;
        // mainCam = GetComponentInChildren<Camera>();
        Debug.Log("stbefoer cam");
        Camera.main.GetComponent<CameraController>().setTarget(gameObject.transform);
        mainCam = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
    }*/
	
	// Update is called once per frame
	void Update () {
        if (!isLocalPlayer)
        {
            return;
        }
        RotateAndMove();
    }

    public void RotateAndMove()
    {
        // rotate head (... TODO: something not right with this)
        Vector3 mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);
        float angle = Mathf.Atan2(mousePos.x, mousePos.y) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward * -1); // -1 for inverted z-axis

        PlayerController childScript = this.transform.GetChild(0).GetComponent<PlayerController>();
        float speed = childScript.GetSpeed();
        //Debug.Log(speed);
        // move head
        //transform.position = Vector2.MoveTowards(transform.position, mousePos, GetSpeed());
        transform.position = Vector2.MoveTowards(transform.position, mousePos, speed);

    }

    public void ZoomCamera(float zoomFactor)
    {
        Debug.Log("zooooom");
        float newZoom = mainCam.orthographicSize + zoomFactor;
        mainCam.orthographicSize = Mathf.Lerp(mainCam.orthographicSize, newZoom, 2.0f * Time.deltaTime);
    }


}
