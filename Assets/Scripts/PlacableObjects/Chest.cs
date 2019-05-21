using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Chest : PlacableObject, IPushableObject {

    public int MaxItemsOnBelt = 3;
    public IPushableObject AttachechedObject;
    public GameObject[] Walls;

    [HideInInspector]
    public List<Rigidbody> ActiveRigidBodies = new List<Rigidbody>();

    public override void ObjectPlaced()
    {
        // Set the anchor changed delegate to handle disabling walls
        for (int i = 0; i < Anchors.Length; ++i)
        {
            Anchors[i].AnchorChanged = AnchorChanged;
        }
        base.ObjectPlaced();
    }

    public void AnchorChanged(AnchorObject obj)
    {
        switch (obj.Anchor.name)
        {
            case "AnchorPoint F":
                Walls[0].SetActive(obj.ConnectAnchor == null);

                if (obj.ConnectAnchor == null)
                    AttachechedObject = null;
                else
                {
                    AttachechedObject = obj.ConnectAnchor.transform.GetTopParent().GetComponent<IPushableObject>();
                    
                }
                break;
            case "AnchorPoint B":
                Walls[1].SetActive(obj.ConnectAnchor == null);
                
                break;
        }
    }

    public void PushObject(GameObject other)
    {
    }
    

    private void FixedUpdate()
    {
        bool canPushObject = AttachechedObject == null || (AttachechedObject.ObjectIsFull());
        for (var i = ActiveRigidBodies.Count - 1; i >= 0; --i)
        {
            if (ActiveRigidBodies[i] != null)
            {
                Vector3 targetForce = canPushObject ? Vector3.zero : transform.forward;
                ActiveRigidBodies[i].velocity = Vector3.Lerp(ActiveRigidBodies[i].velocity, targetForce, Time.deltaTime);

            }
            else
            {
                ActiveRigidBodies.RemoveAt(i);
            }
        }
    }

    public bool ObjectIsFull(List<IPushableObject> CheckedObjects = null)
    {
        if (Anchors.Where(x => x.ConnectAnchor != null).Count<AnchorObject>() > 1)
        {
            if (CheckedObjects == null)
                CheckedObjects = new List<IPushableObject>();
            else if (CheckedObjects.Contains(this))
            {
                return ActiveRigidBodies.Count >= MaxItemsOnBelt;
            }
            CheckedObjects.Add(this);
        }

        return ActiveRigidBodies.Count >= MaxItemsOnBelt && (AttachechedObject == null || AttachechedObject.ObjectIsFull(CheckedObjects));
    }
}
