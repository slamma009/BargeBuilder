using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlacableObject : MonoBehaviour {


    public AnchorObject[] Anchors;
    public bool Placed { get; private set; }
    public bool IsGhost = false;



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




    public void MakeGhost(Material ghostMaterial, bool turnOfColliders = true)
    {
        disableChildren(transform, ghostMaterial, turnOfColliders);
    }



    void disableChildren(Transform childTransform, Material ghostMaterial, bool turnOfColliders = true)
    {
        // Don't change collider object
        if(childTransform.name == "COLLIDEROBJECT")
        {
            return;
        }

        // If there's no ghost material then we cant change our renderer. If we have a sys then we are a collider object and need to turn off our renderer
        if(ghostMaterial != null || !turnOfColliders)
        {
            Renderer renderer = childTransform.GetComponent<Renderer>();
            if (renderer != null)
            {
                if (turnOfColliders)
                    renderer.material = ghostMaterial;
                else
                    renderer.enabled = false;
            }

        }


        Collider collider = childTransform.GetComponent<Collider>();
        // If we have a sys then we are a collider object and need to do a lot of stuff
        if (collider != null)
        {
            if (!turnOfColliders)
            {
                // If it's not a Placable layer, then we shouldn't check for collisn, so disable it.
                if (collider.gameObject.layer != LayerMask.NameToLayer("PlacementCollider"))
                {
                    collider.enabled = false;
                }
                else
                {
                    // Make sure all mesh colliders are convex to be triggers.
                    MeshCollider mesh = childTransform.GetComponent<MeshCollider>();
                    if (mesh != null)
                        mesh.convex = true;

                    collider.isTrigger = true;
                }
            }
            else
            {
                collider.enabled = false;
            }
        }
            

      
        for(var i=0; i< childTransform.childCount; ++i)
        {
            disableChildren(childTransform.GetChild(i), ghostMaterial, turnOfColliders);
        }
    }
}
