using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    public Transform player;

    private Vector3 offset;


	void Start () 
    {
        if (player != null)
        {
            offset = transform.position - player.transform.position;
        }
	}
	
	void LateUpdate () 
    {
        if (player != null)
        {
            transform.position = player.transform.position + offset;
        }
    }

    public void setTarget(Transform target)
    {
        player =  target;
        offset = transform.position - player.transform.position;
    }
}
