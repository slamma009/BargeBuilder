using UnityEngine;


public class BezierTrigger : MonoBehaviour
{
    public DraggableObject dObj;
    public AnchorSystem AnchorSys;
    private void OnTriggerEnter(Collider other)
    {
        Transform t = other.transform.GetTopParent();
        if (other.gameObject.layer == LayerMask.NameToLayer("Placable") || t.tag == "Draggable") {
            if (t != dObj.Anchors[0].transform.GetTopParent() 
                && t != dObj.GhostObject.transform.GetTopParent() 
                && (dObj.Anchors[1] == null || t != dObj.Anchors[1].transform.GetTopParent()))
                AnchorSys.DraggableCollidedObjects.Add(other.gameObject);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        Transform t = other.transform.GetTopParent();
        if (other.gameObject.layer == LayerMask.NameToLayer("Placable") || t.tag == "Draggable")
        {
            AnchorSys.DraggableCollidedObjects.Remove(other.gameObject);
        }
    }
}

