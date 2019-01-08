using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Chunk
{
    public ChunkObject[,,] ChunkObjects;
    public ChunkHandler ChunkController;
    public Chunk(int chunkSize, ChunkHandler chunkHandler)
    {
        ChunkObjects = new ChunkObject[chunkSize, chunkSize, chunkSize];
        ChunkController = chunkHandler;
    }
}

public class ChunkObject
{
    public ChunkObject(PlacableObject obj, Chunk chunk)
    {
        Object = obj;
        ChunkHolder = chunk;
    }

    public ChunkObject(Vector2 chunkPos, Vector3Int objPos, Vector3Int selfObjPos, Chunk chunk)
    {
        IsReference = true;
        SelfObjectPosition = selfObjPos;
        ReferenceChunkPosition = chunkPos;
        ReferenceObjectPosition = objPos;
        ChunkHolder = chunk;
    }

    public PlacableObject Object;
    public Chunk ChunkHolder;
    public bool IsReference;
    public Vector2 ReferenceChunkPosition;
    public Vector3Int ReferenceObjectPosition;
    public Vector3Int SelfObjectPosition;

    public float LastUpdateTimestamp;

    public void UpdateBlock(GridController gridController, Vector2 ChunkPos, Vector3 ChunkGridPos)
    {
        // TODO: Check if referenced chunkObject still exists before removing
        if (IsReference && ChunkHolder.ChunkController.Chunks[ReferenceChunkPosition].ChunkObjects[ReferenceObjectPosition.x, ReferenceObjectPosition.y, ReferenceObjectPosition.z] == null)
        {
            ChunkHolder.ChunkObjects[SelfObjectPosition.x, SelfObjectPosition.y, SelfObjectPosition.z] = null;
            //Debug.Log("Removing reference in chunk " + ReferenceChunkPosition + " from chunkGridPosition " + ReferenceObjectPosition);
        }

        if (Object != null)
        {
            Object.UpdateBlock(gridController, ChunkPos, ChunkGridPos);
        }
    }

}