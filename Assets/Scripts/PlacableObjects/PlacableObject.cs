﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacableObject : MonoBehaviour {

    public Transform[] GridAnchors;

	public void MakeGhost(Material ghostMaterial)
    {
        disableChildren(transform, ghostMaterial);
    }

    void disableChildren(Transform childTransform, Material ghostMaterial)
    {

        if(ghostMaterial != null)
        {
            Renderer renderer = childTransform.GetComponent<Renderer>();
            if (renderer != null)
                renderer.material = ghostMaterial;
        }

        Collider collider = childTransform.GetComponent<Collider>();

        if(collider != null)
            collider.enabled = false;
      
        for(var i=0; i< childTransform.childCount; ++i)
        {
            disableChildren(childTransform.GetChild(i), ghostMaterial);
        }
    }
}
