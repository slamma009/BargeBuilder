using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Extractor : PlacableObject
{
    /// <summary>
    /// The position to spawn the ore at
    /// </summary>
    public Transform OreSpawn;

    /// <summary>
    /// Modifier for Spawn Rate speed of items
    /// </summary>
    public float SpawnRateModifier = 1;

    /// <summary>
    /// The item to spawn
    /// </summary>
    public ItemInstance SpawnObject;

    /// <summary>
    /// The inventory of the extractor
    /// </summary>
    public Inventory ExtractorInventory;
    
    public Transform OutputAnchor;

    private float _ticksSinceLastSpawn;

    private ConveyorBelt outputObject;

    
    public void AnchorChanged(AnchorObject obj)
    {
        if (obj.ConnectAnchor == null)
            outputObject = null;
        else
        {
            ConveyorBelt belt = obj.ConnectAnchor.transform.GetTopParent().GetComponent<ConveyorBelt>();
            if(belt != null)
                outputObject = belt;
            else
                outputObject = null;
        }
    }

    public override void ObjectPlaced()
    {
        Anchors[0].AnchorChanged = AnchorChanged;

        // Add the TickUpdate to the Tick Event
        TickController.TickEvent += TickUpdate;
        base.ObjectPlaced();
    }


    public void TickUpdate(object sender, TickArgs arg)
    {
        if ( Placed)

        {
            // Update extraction ticks
            bool outputCanTakeItem = OutputCanTakeItem();
            if (outputCanTakeItem || ExtractorInventory.CanAddItem(SpawnObject.ID))
            {
                _ticksSinceLastSpawn++;
                if (_ticksSinceLastSpawn > SpawnRateModifier * SpawnObject.ExtractTime)
                {
                    // TODO: Currently ammount is hardcoded on the ItemInstance. Make a variable on the InventoryItem?
                    _ticksSinceLastSpawn = 0;
                    if (outputCanTakeItem)
                    {
                        ItemInstance newObj = Instantiate(SpawnObject, OreSpawn.position, Quaternion.identity).GetComponent<ItemInstance>();
                        outputObject.PushItem(newObj);
                    }
                    else
                    {
                        ExtractorInventory.Add(SpawnObject.ID, SpawnObject.Amount);
                    }
                }
            }

            // Try to remove any items from inventory
            if (OutputCanTakeItem())
            {
                //TODO: Change hardcoded 64 to item count. 
                //TODO: Change how ItemInstance works. Should have item on it.
                int amount = 64 - ExtractorInventory.RemoveById(SpawnObject.ID, 64);
                if(amount > 0)
                {
                    ItemInstance newObj = Instantiate(SpawnObject, OreSpawn.position, Quaternion.identity).GetComponent<ItemInstance>();
                    newObj.Amount = amount;
                    outputObject.PushItem(newObj);
                }
            }
        }


    }

    private bool OutputCanTakeItem()
    {
        return outputObject != null && outputObject.CanTakeItem(SpawnObject);
    }

    // Remove TickUpdate from the event list if it has been set.
    private void OnDestroy()
    {
        if (Placed)
            TickController.TickEvent -= TickUpdate;
    }

}
