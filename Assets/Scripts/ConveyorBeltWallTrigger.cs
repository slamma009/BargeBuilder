using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorBeltWallTrigger : MonoBehaviour
{


    public GameObject FrontWall;
    GameObject ActiveWall;

    private void OnTriggerEnter(Collider other)
    {
        if (ActiveWall == null && other.tag == "ConveyorWall")
        {
            ActiveWall = other.gameObject;
            ActiveWall.SetActive(false);
            if (other.name != "Wall Front")
                FrontWall.SetActive(false);

        }
    }

    void FixedUpdate()
    {
        if (ActiveWall == null && !FrontWall.activeSelf)
        {
            FrontWall.SetActive(true);
        }
    }

    void OnDestroy()
    {
        if (ActiveWall != null)
            ActiveWall.SetActive(true);
    }
}
