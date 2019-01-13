using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Average : MonoBehaviour {
    public Transform first;
    public Transform second;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        transform.position = (first.position + second.position) * 0.5f;
	}
}
