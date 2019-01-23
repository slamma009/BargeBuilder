using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabbableObject : MonoBehaviour {

    private GameObject Grabber;
    private Transform Parent;
    private Vector3 OriginalScale;

    protected bool IsGrabbed = false;
	// Use this for initialization
	void Start () {
        Parent = transform.parent;
        OriginalScale = transform.localScale;
    }

    public virtual void ItemGrabbed(GameObject grabber, bool isRighthand)
    {
        transform.parent = grabber.transform;
        IsGrabbed = true;
    }

    public virtual void ItemDropped()
    {
        Quaternion rot = transform.rotation;
        transform.parent = Parent;
        transform.localScale = OriginalScale;

        Grabber = null;
        IsGrabbed = false;
    }

}
