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
        SecondsToSpawn = 60 / SpawnRatePerMinute;
    }

    public void FixedUpdate()
    {
        if (Placed && OutputObject != null && OutputObject.ActiveRigidBodies.Count < OutputObject.MaxItemsOnBelt)
        {
            TimeSinceLastSpawn += Time.deltaTime;
            if (TimeSinceLastSpawn > SecondsToSpawn)
            {
                TimeSinceLastSpawn = 0;
                Instantiate(SpawnObject, OreSpawn.position, Quaternion.identity);
            }
        }
    }

    public override void UpdateBlock(GridController gridController, Vector2 ChunkPos, Vector3 ChunkGridPos)
    {
        FindOutputObject(gridController);

        base.UpdateBlock(gridController, ChunkPos, ChunkGridPos);
    }

    private void FindOutputObject(GridController gridController)
    {
        // Get the chunkPos and chunkGridPos of the object in the output position
        GridPositionObject outputWorldGridPosition = gridController.GetGridPosition(OutputAnchor.position);
        Vector2 outputChunkPosition = gridController.ChunkController.ConvertGridPositionToChunkPosition(outputWorldGridPosition.GridPosition);
        Vector3 outputChunkGridPosition = gridController.ChunkController.ConvertWorldGridPositionToChunkGridPosition(outputChunkPosition, outputWorldGridPosition.GridPosition);

        // Find the output object, and make sure it exists and is a conveyor belt.
        if (gridController.ChunkController.Chunks.ContainsKey(outputChunkPosition))
        {
            ChunkObject chunkObject = gridController.ChunkController.Chunks[outputChunkPosition].ChunkObjects[(int)outputChunkGridPosition.x, (int)outputChunkGridPosition.y, (int)outputChunkGridPosition.z];
            if (chunkObject == null || chunkObject.Object == null)
            {
                Debug.Log("Nothing found, removing reference.");
                OutputObject = null;
            }
            else
            {
                ConveyorBelt newBelt = chunkObject.Object.GetComponent<ConveyorBelt>();
                if (newBelt == null)
                {
                    Debug.Log("Nothing found, removing reference.");
                    OutputObject = null;
                }
                else
                {
                    Debug.Log("Belt found, adding reference.");
                    OutputObject = newBelt;
                }
            }

        }
    }

}
