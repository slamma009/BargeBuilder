﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandGrabbing : MonoBehaviour {

    public bool RightHand = true;
    public float HitRadius = 1;
    public LayerMask LayerMask;

    [HideInInspector]
    public bool Grabbed = false;

    private GameObject GrabbedObject;
    private GrabbableObject GrabbedScript;


    void Grab()
    {
        Grabbed = true;
        RaycastHit[] hits;

        hits = Physics.SphereCastAll(transform.position, HitRadius, transform.forward, 0, LayerMask);

        if(hits.Length > 0)
        {
            int index = 0;
            if(hits.Length > 1)
            {
                for(int i=1; i<hits.Length; ++i)
                {
                    if(hits[i].distance < hits[index].distance)
                    {
                        index = i;
                    }
                }
            }

            GrabbedObject = hits[index].transform.gameObject;
            GrabbedScript = GrabbedObject.GetComponent<GrabbableObject>();

            if(GrabbedScript != null)
            {
                GrabbedScript.Grabbed(gameObject, RightHand);
            }

            Rigidbody rb = GrabbedObject.transform.GetComponent<Rigidbody>();
            if(rb != null)
            {
                rb.isKinematic = true;
            }
        }
    }

    void Drop()
    {
        Grabbed = false;

        if(GrabbedObject != null)
        {
            Rigidbody rb = GrabbedObject.transform.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = false;
            }
            if (GrabbedScript != null)
            {
                GrabbedScript.Dropped();
            }
            GrabbedObject = null;
            GrabbedScript = null;
        }
    }

    // Update is called once per frame
    void Update ()
    {

        if (!RightHand)
        {
            if (!Grabbed && Input.GetAxis("Oculus_CrossPlatform_PrimaryHandTrigger") == 1)
            {
                Grab();
            }
            if (Grabbed && Input.GetAxis("Oculus_CrossPlatform_PrimaryHandTrigger") < 1)
            {
                Drop();
            }
        }

        if (RightHand)
        {
            if (!Grabbed && Input.GetAxis("Oculus_CrossPlatform_SecondaryHandTrigger") == 1)
            {
                Grab();
            }
            if (Grabbed && Input.GetAxis("Oculus_CrossPlatform_SecondaryHandTrigger") < 1)
            {
                Drop();
            }
        }
    }

}

