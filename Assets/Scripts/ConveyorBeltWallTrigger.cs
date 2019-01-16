using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorBeltWallTrigger : MonoBehaviour
{


    //public GameObject FrontWall;
    //public GameObject ActiveWall;

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (ActiveWall == null && other.tag == "ConveyorWall")
    //    {
    //        //Debug.Log(transform.parent.name + ": " + other.gameObject.name + " Enter");
    //        ActiveWall = other.gameObject;
    //        ActiveWall.SetActive(false);
    //        if (other.name != "Wall Front" && FrontWall != null)
    //            FrontWall.SetActive(false);

    //    }
    //}
    //private void OnTriggerExit(Collider other)
    //{
    //    ////Debug.Log(transform.parent.name + ": " + other.gameObject.name + " Left");
    //    //if(other.gameObject == ActiveWall)
    //    //{
    //    //    ActiveWall = null;
    //    //}
    //}

    //void FixedUpdate()
    //{
    //    if (ActiveWall == null && FrontWall != null && !FrontWall.activeSelf)
    //    {
    //        FrontWall.SetActive(true);
    //    }
    //}

    //void OnDestroy()
    //{
    //    if (ActiveWall != null)
    //        ActiveWall.SetActive(true);
    //}
}
