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
            ItemsOnBelt[i].TravelTime += Time.deltaTime;
            ItemsOnBelt[i].Item.transform.position = Vector3.Lerp(
                ItemsOnBelt[i].Start,
                ItemsOnBelt[i].Target,
                ItemsOnBelt[i].TravelTime);

            if (ItemsOnBelt[i].TravelTime >= 1)
            {
                ItemsOnBelt[i].Item.transform.position = ItemsOnBelt[i].Target;
                if (ItemsOnBelt[i].State == 0)
                {
                    if (AttachechedObject != null && ItemsOnBelt.Where(x => x.State == 1).Count() == 0)
                    {
                        ItemsOnBelt[i].TravelTime = 0;
                        ItemsOnBelt[i].State++;
                        ItemsOnBelt[i].Target = ItemsOnBelt[i].Target + transform.forward;
                        ItemsOnBelt[i].Start = ItemsOnBelt[i].Item.transform.position;
                    }
                }
                else
                {
                    if (AttachechedObject != null && AttachechedObject.CanTakeItem(ItemsOnBelt[i].Item))
                    {
                        AttachechedObject.PushItem(ItemsOnBelt[i].Item);
                        ItemsOnBelt.RemoveAt(i);
                    }
                }
            }
            
        }
    }

    public void PushItem(Item item)
    {
        item.transform.parent = this.transform;
        ItemsOnBelt.Add(new ConveyorItemInfo(item, transform.position + Vector3.up * 0.5f, item.transform.position));
    }

    public bool CanTakeItem(Item item)
    {
        return ItemsOnBelt.Where(x => x.State == 0).Count() == 0;
    }
}

[Serializable]
public class ConveyorItemInfo
{
    public Item Item;

    public int State = 0;

    public Vector3 Start;
    public Vector3 Target;

    public float TravelTime = 0;

    public ConveyorItemInfo(Item item, Vector3 target, Vector3 start)
    {
        Start = start;
        Item = item;
        Target = target;
    }
}
