using System.Linq;
using UnityEngine;

public class DraggableObject : MonoBehaviour
{
    public Transform[] BezeirPoints;

    [HideInInspector]
    public GameObject[] Anchors;

    public GameObject GhostObject;
    protected MeshRenderer Renderer;
    protected MeshCollider Collider;
    protected MeshFilter Mesh;

    protected GameObject FirstAnchor;

    protected bool Placed;

    public virtual void Initialize(GameObject anchor, GameObject ghostObject)
    {
        Collider = gameObject.GetComponent<MeshCollider>();
        Renderer = gameObject.GetComponent<MeshRenderer>();
        Mesh = gameObject.GetComponent<MeshFilter>();
        Anchors = new GameObject[2];
        Anchors[0] = anchor;
        Anchors[1] = null;
        this.GhostObject = ghostObject;
        FirstAnchor = anchor;
    }


    public void ObjectPlaced(ConveyorBelt Anchor)
    {
        ObjectPlaced(Anchor.Anchors[2].Anchor);
    }

    public virtual void ObjectPlaced(GameObject anchor)
    {
        Placed = true;
    }

    public virtual bool ObjectIsValid()
    {
        return true;
    }

    


}

