using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GodViewCameraMovement : MonoBehaviour {
    public bool upDown = false;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKey(KeyCode.Mouse1))
        {
            if (upDown)
            {
                transform.Rotate(-Input.GetAxis("Mouse Y") * 10, 0, 0);
            }
            else
            {
                transform.Rotate(0, Input.GetAxis("Mouse X") * 10, 0);
            }
        }
    }
}
