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
    /// Base spawn time in ticks
    /// </summary>
    public float SpawnRate = 15;

    /// <summary>
    /// The item to spawn
    /// </summary>
    public Item SpawnObject;
    
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
        if (Placed && outputObject != null && outputObject.CanTakeItem(SpawnObject))
        {
            _ticksSinceLastSpawn++;
            if (_ticksSinceLastSpawn > SpawnRate)
            {
                _ticksSinceLastSpawn = 0;
                Item newObj = Instantiate(SpawnObject, OreSpawn.position, Quaternion.identity).GetComponent<Item>();
                outputObject.PushItem(newObj);
            }
        }
    }

    // Remove TickUpdate from the event list if it has been set.
    private void OnDestroy()
    {
        if (Placed)
            TickController.TickEvent -= TickUpdate;
    }

}
