using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Chunk
{
    public Array3d<ChunkObject> ChunkObjects;
    public ChunkHandler ChunkController;
    public Chunk(int chunkSize, ChunkHandler chunkHandler)
    {
        ChunkObjects = new Array3d<ChunkObject>(chunkSize);
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
        if (IsReference && ChunkHolder.ChunkController.Chunks[ReferenceChunkPosition].ChunkObjects[ReferenceObjectPosition] == null)
        {
            ChunkHolder.ChunkObjects[SelfObjectPosition] = null;
            //Debug.Log("Removing reference in chunk " + ReferenceChunkPosition + " from chunkGridPosition " + ReferenceObjectPosition);
        }

        if (Object != null)
        {
            Object.UpdateBlock(gridController, ChunkPos, ChunkGridPos);
        }
    }

}

public class Array3d<T>
{
    private T[,,] Objects;
    public Array3d()
    {
        Objects = new T[16, 16, 16];
    }

    public Array3d(int chunkSize)
    {
        Objects = new T[chunkSize, chunkSize, chunkSize];
    }

    public T this[Vector3Int vector]
    {
        get { return this.Objects[vector.x, vector.y, vector.z]; }
        set { this.Objects[vector.x, vector.y, vector.z] = value; }
    }
}