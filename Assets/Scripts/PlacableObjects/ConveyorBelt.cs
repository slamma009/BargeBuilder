using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ConveyorBelt : PlacableObject, IPushableObject
{
    public int MaxItemsOnBelt = 2;
    public GameObject[] Walls;
    public IPushableObject AttachechedObject;

    public List<ConveyorItemInfo> ItemsOnBelt = new List<ConveyorItemInfo>();



    public override void ObjectPlaced()
    {
        // Set the anchor changed delegate to handle disabling walls
        for (int i = 0; i < Anchors.Length; ++i)
        {
            Anchors[i].AnchorChanged = AnchorChanged;
        }
        base.ObjectPlaced();
    }

    // Disables walls if anchored to an object.
    public void AnchorChanged(AnchorObject obj)
    {
        switch (obj.Anchor.name)
        {
            case "AnchorPoint F":
                Walls[0].SetActive(obj.ConnectAnchor == null);

                if(obj.ConnectAnchor == null)
                    AttachechedObject = null;
                else
                {
                    AttachechedObject = obj.ConnectAnchor.transform.GetTopParent().GetComponent<IPushableObject>();
                }
                break;
            case "AnchorPoint R":
                Walls[1].SetActive(obj.ConnectAnchor == null);
                break;
            case "AnchorPoint B":
                Walls[2].SetActive(obj.ConnectAnchor == null);
                break;
            case "AnchorPoint L":
                Walls[3].SetActive(obj.ConnectAnchor == null);
                break;
        }
    }
    
    
    private void FixedUpdate()
    {
        for(var i=ItemsOnBelt.Count - 1; i>=0; --i)
        {
                
            ItemsOnBelt[i].Item.transform.position = Vector3.MoveTowards(
                ItemsOnBelt[i].Item.transform.position,
                ItemsOnBelt[i].Target,
                Time.deltaTime);

            if (Vector3.Distance(ItemsOnBelt[i].Item.transform.position, ItemsOnBelt[i].Target) < 0.001f)
            {
                ItemsOnBelt[i].Item.transform.position = ItemsOnBelt[i].Target;
                if (ItemsOnBelt[i].State == 0)
                {
                    if (AttachechedObject != null && ItemsOnBelt.Where(x => x.State == 1).Count() == 0)
                    {
                        ItemsOnBelt[i].State++;
                        ItemsOnBelt[i].Target = ItemsOnBelt[i].Target + transform.forward;
                    }
                }
                else
                {
                    if (AttachechedObject != null && !AttachechedObject.ObjectIsFull())
                    {
                        AttachechedObject.PushObject(ItemsOnBelt[i].Item);
                        ItemsOnBelt.RemoveAt(i);
                    }
                }
            }
            
        }
    }

    public void PushObject(GameObject item)
    {
        item.transform.parent = this.transform;
        ItemsOnBelt.Add(new ConveyorItemInfo(item, new Vector3(transform.position.x, item.transform.position.y, transform.position.z)));
    }

    public bool ObjectIsFull(List<IPushableObject> CheckedObjects = null)
    {
        if (Anchors.Where(x => x.ConnectAnchor != null).Count<AnchorObject>() > 1)
        {
            if (CheckedObjects == null)
                CheckedObjects = new List<IPushableObject>();
            else if (CheckedObjects.Contains(this))
            {
                 return ItemsOnBelt.Count >= MaxItemsOnBelt;
            }
            CheckedObjects.Add(this);
        }

        return ItemsOnBelt.Where(x => x.State == 0).Count() > 0;
    }
}

[Serializable]
public class ConveyorItemInfo
{
    public GameObject Item;

    public int State = 0;

    public Vector3 Target;

    public ConveyorItemInfo(GameObject item, Vector3 target)
    {
        Item = item;
        Target = target;
    }
}
