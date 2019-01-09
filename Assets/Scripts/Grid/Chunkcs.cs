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

    public ChunkObject(Vector2 referenceChunkPos, Vector3Int referenceChunkObjPos, Vector3Int selfObjPos, Chunk chunk)
    {
        IsReference = true;
        SelfObjectPosition = selfObjPos;
        ReferenceChunkPosition = referenceChunkPos;
        ReferenceObjectPosition = referenceChunkObjPos;
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
        if (IsReference && ChunkHolder.ChunkController.Chunks[ReferenceChunkPosition].ChunkObjects[ReferenceObjectPosition] == null)
        {
            ChunkHolder.ChunkObjects[SelfObjectPosition] = null;
            return;
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