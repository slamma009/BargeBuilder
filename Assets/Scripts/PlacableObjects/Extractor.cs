using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Extractor : PlacableObject
{
    public Transform OreSpawn;
    public float SpawnRatePerMinute = 20;
    public GameObject SpawnObject;
    public Transform OutputAnchor;

    private float TimeSinceLastSpawn;
    private float SecondsToSpawn;

    private ConveyorBelt OutputObject;


    public void Start()
    {
    }
    public void AnchorChanged(AnchorObject obj)
    {
        if (obj.ConnectAnchor == null)
            OutputObject = null;
        else
        {
            ConveyorBelt belt = obj.ConnectAnchor.transform.GetTopParent().GetComponent<ConveyorBelt>();
            Debug.Log(transform.GetTopParent().name + ": " + belt);
            if(belt != null)
                OutputObject = belt;
            else
                OutputObject = null;
        }
    }
    public override void ObjectPlaced()
    {
        Anchors[0].AnchorChanged = AnchorChanged;
        base.ObjectPlaced();
    }
    public void FixedUpdate()
    {
        if (Placed && OutputObject != null && OutputObject.ActiveRigidBodies.Count < OutputObject.MaxItemsOnBelt)
        {
            TimeSinceLastSpawn += Time.deltaTime;
            if (TimeSinceLastSpawn > 60 / SpawnRatePerMinute)
            {
                TimeSinceLastSpawn = 0;
                Instantiate(SpawnObject, OreSpawn.position, Quaternion.identity);
            }
        }
    }

}
