using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GodViewMovement : MonoBehaviour {
    public float Speed = 10;
    public float RunSpeed = 20;
    public float RotateSpeed = 2;
    public float minHeight = 1;


    private float ForwardInput;
    private float SidewaysInput;
    private float TurnInput;
    private float HeightInput;

    // Start is called before the first frame update
    void Start()
    {

    }

    private void Update()
    {
        ForwardInput = 0 + (Input.GetKey(KeyCode.W) ? 1 : 0) - (Input.GetKey(KeyCode.S) ? 1 : 0);
        SidewaysInput = 0 + (Input.GetKey(KeyCode.D) ? 1 : 0) - (Input.GetKey(KeyCode.A) ? 1 : 0);
        HeightInput = 0 + (Input.GetKey(KeyCode.Space) ? 1 : 0) - (Input.GetKey(KeyCode.C) ? 1 : 0);

        //TurnInput = 0 + (Input.GetKey(KeyCode.E) ? 1 : 0) - (Input.GetKey(KeyCode.Q) ? 1 : 0);

    }
    // Update is called once per frame
    void FixedUpdate()
    {

        //Vector3 inputDirection = transform.right * SidewaysInput + transform.up * HeightInput + transform.forward * ForwardInput; // new Vector3(SidewaysInput, HeightInput, ForwardInput);
        Vector3 inputDirection = new Vector3(SidewaysInput, HeightInput, ForwardInput);

        if (transform.position.y < minHeight)
        {
            inputDirection.y = 0;
        }
        inputDirection.Normalize();


        transform.Translate(inputDirection * (Input.GetKey(KeyCode.LeftShift) ? RunSpeed : Speed) * Time.deltaTime);

        //this.gameObject.transform.Rotate(Vector3.up * RotateSpeed * TurnInput * Time.deltaTime);


        if (transform.position.y < minHeight)
        {
            transform.position = new Vector3(transform.position.x, minHeight, transform.position.z);
        }

    }
}
