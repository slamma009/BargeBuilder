using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnItem : MonoBehaviour {
    public GameObject[] prefab;
    public Transform spawnpoint;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyUp(KeyCode.A))
        {
            Instantiate(prefab[0], spawnpoint.position, Quaternion.identity);
        }
        if (Input.GetKeyUp(KeyCode.D))
        {
            Instantiate(prefab[1], spawnpoint.position, Quaternion.identity);
        }
    }
}
