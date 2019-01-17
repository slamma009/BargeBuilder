﻿using System.Linq;
using UnityEngine;

public class DraggableObject : MonoBehaviour
{
    public Transform[] BezeirPoints;

    [HideInInspector]
    public GameObject[] Anchors;

    private GameObject GhostObject;
    private MeshRenderer Renderer;
    private MeshCollider Collider;

    private GameObject FirstAnchor;

    private bool Placed;

    public void Initialize(GameObject anchor, GameObject ghostObject)
    {
        Collider = gameObject.GetComponent<MeshCollider>();
        Renderer = gameObject.GetComponent<MeshRenderer>();
        Renderer.enabled = true;
        Collider.enabled = false;

        Anchors = new GameObject[2];
        Anchors[0] = anchor;
        Anchors[1] = null;
        this.GhostObject = ghostObject;
        BezeirPoints[0].position = anchor.transform.position + Anchors[0].transform.forward * 0.01f; 
        BezeirPoints[1].position = anchor.transform.position + anchor.transform.forward * -2;
        FirstAnchor = anchor;
        Debug.Log(anchor.name);
    }

    public void Update()
    {
        if (!Placed && GhostObject != null)
        {
            if (Anchors[1] == null)
            {
                BezeirPoints[3].position = GhostObject.transform.position + GhostObject.transform.forward * -1;
                BezeirPoints[2].position = GhostObject.transform.position + GhostObject.transform.forward * -5;
            }
            else
            {
                SnapToSecondAnchor();
            }
        }

        
    }

    public void ObjectPlaced(ConveyorBelt Anchor)
    {
        ObjectPlaced(Anchor.Anchors[2].Anchor);
    }

    public virtual void ObjectPlaced(GameObject anchor)
    {
        Placed = true;
        Anchors[1] = anchor;
        AnchorObject obj = anchor.transform.GetTopParent().GetComponent<PlacableObject>().Anchors.SingleOrDefault(x => x.Anchor.name == anchor.name);
        obj.ConnectAnchor = this.gameObject;
        AnchorObject firstAnchor = FirstAnchor.transform.GetTopParent().GetComponent<PlacableObject>().Anchors.SingleOrDefault(x => x.Anchor.name == FirstAnchor.name);
        firstAnchor.ConnectAnchor = this.gameObject;
        SnapToSecondAnchor();
    }

    public void SnapToSecondAnchor()
    {
        BezeirPoints[3].position = Anchors[1].transform.position + Anchors[1].transform.forward * 0.01f;
        BezeirPoints[2].position = Anchors[1].transform.position + Anchors[1].transform.forward * -5;
    }
    


}

