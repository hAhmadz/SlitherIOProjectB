using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetCameraController : MonoBehaviour {

    public Transform player;

    private Vector3 offset;


	void Start () 
    {
        offset = transform.position - player.transform.position;
	}
	
	void LateUpdate () 
    {
        transform.position = player.transform.position + offset;
    }


    public void setTarget(Transform target)
    {
        player = target;
        //Debug.Log(player == null);
        offset = transform.position - player.transform.position;
    }
}
