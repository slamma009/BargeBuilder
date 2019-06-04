using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Chest : PlacableObject, IPushableObject {

    public int MaxItemsOnBelt = 3;
    public IPushableObject AttachechedObject;
    public GameObject[] Walls;

    private Inventory _inventory;

    [HideInInspector]
    public List<ConveyorItemInfo> ItemsOnBelt = new List<ConveyorItemInfo>();

    public override void ObjectPlaced()
    {
        // Set the anchor changed delegate to handle disabling walls
        for (int i = 0; i < Anchors.Length; ++i)
        {
            Anchors[i].AnchorChanged = AnchorChanged;
        }

        _inventory = gameObject.GetComponent<Inventory>();
        if (_inventory == null)
            throw new MissingComponentException("The Inventory component was not found on the Generator object");

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

    private void FixedUpdate()
    {
        for (var i = ItemsOnBelt.Count - 1; i >= 0; --i)
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
                    int amountOfItemBefore = ItemsOnBelt[i].Item.Amount;
                    int amountOfItemAfter = _inventory.Add(ItemsOnBelt[i].Item);
                    if (amountOfItemAfter == 0)
                    {
                        Destroy(ItemsOnBelt[i].Item.gameObject);
                        ItemsOnBelt.RemoveAt(i);
                    }
                    else
                    { 
                        if(amountOfItemBefore != amountOfItemAfter)
                            ItemsOnBelt[i].Item.Amount = amountOfItemAfter;

                        if (AttachechedObject != null && ItemsOnBelt.Where(x => x.State == 1).Count() == 0)
                        {
                            ItemsOnBelt[i].TravelTime = 0;
                            ItemsOnBelt[i].State++;
                            ItemsOnBelt[i].Target = ItemsOnBelt[i].Target + transform.forward;
                            ItemsOnBelt[i].Start = ItemsOnBelt[i].Item.transform.position;
                        }
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

    public bool CanTakeItem(ItemInstance item)
    {
        return ItemsOnBelt.Where(x => x.State == 0).Count() == 0;
    }

    public void PushItem(ItemInstance item)
    {
        if (item == null)
            throw new ArgumentNullException("item", "A null value was passed into PushItem");
        item.transform.parent = this.transform;
        ItemsOnBelt.Add(new ConveyorItemInfo(item, transform.position + Vector3.up * 0.5f, item.transform.position));

    }
}
