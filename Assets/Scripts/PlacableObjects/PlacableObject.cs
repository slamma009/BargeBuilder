using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlacableObject : MonoBehaviour {


    public AnchorObject[] Anchors;
    protected bool Placed = false;



    public virtual void ObjectPlaced()
    {
        Placed = true;
        // Loop through each anchor and use raycast to find any anchors it's attached to
        for(int i=0; i<Anchors.Length; ++i)
        {
            RaycastHit hit;
            Ray ray = new Ray(Anchors[i].Anchor.transform.position, Anchors[i].Anchor.transform.forward);
            if(Physics.Raycast(ray, out hit, 0.03f, LayerMask.GetMask("Anchor")))
            {
                // Set our anchor to the found anchor
                Anchors[i].ConnectAnchor = hit.transform.gameObject;
                
                
                // Try to find it's PlacableObject code, and set it's anchor to this one
                AnchorObject other = hit.transform.GetTopParent().GetComponent<PlacableObject>().Anchors.SingleOrDefault(x => x.Anchor == hit.transform.gameObject);
                other.ConnectAnchor = Anchors[i].Anchor;
                
            }
        }
    }




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
