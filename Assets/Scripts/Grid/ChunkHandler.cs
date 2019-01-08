
using System.Collections.Generic;
using UnityEngine;

public class ChunkHandler
{
    public int ChunkSize = 16;
    public int ChunkLoadRadius = 1;

    public Dictionary<Vector2, Chunk> Chunks = new Dictionary<Vector2, Chunk>();

    public readonly float ChunkDiviser;

    public ChunkHandler()
    {
        ChunkDiviser = 1f / (float)ChunkSize;
    }

    // Generates chunks if neccesarry
    public void LoadChunks(Vector3 playersGridPosition)
    {
        //get the grid postion
        Vector2 chunkPosition = ConvertGridPositionToChunkPosition(playersGridPosition);

        //Loop through all chunks in the radius around around the player 
        for (var i = chunkPosition.x - ChunkLoadRadius; i <= chunkPosition.x + ChunkLoadRadius; ++i)
        {
            for (var j = chunkPosition.y - ChunkLoadRadius; j <= chunkPosition.y + ChunkLoadRadius; ++j)
            {
                // Check if chunk exists, generate it if it does not
                Vector2 newChunkPos = new Vector2(i, j);
                if (!Chunks.ContainsKey(newChunkPos))
                {
                    Chunks.Add(newChunkPos, new Chunk(ChunkSize, this));
                    //Debug.Log("New Chunk created at (" + newChunkPos.x + ", " + newChunkPos.y + ")");
                }
            }
        }
    }

    public Vector3Int ConvertWorldGridPositionToChunkGridPosition(Vector2 chunkPosition, Vector3 gridPosition)
    {
        return new Vector3Int(
               Mathf.RoundToInt(gridPosition.x - (chunkPosition.x * ChunkSize)),
               Mathf.RoundToInt(gridPosition.y),
               Mathf.RoundToInt((gridPosition.z - (chunkPosition.y * ChunkSize)) * -1)
           );
    }
    // Convert the grid position to the chunk position.
    public Vector2 ConvertGridPositionToChunkPosition(Vector3 gridPosition)
    {
        Vector2 chunkPosition = new Vector2(
            (int)((gridPosition.x - (gridPosition.x < 0 ? (ChunkSize - 1) : 0)) * ChunkDiviser),
            (int)((gridPosition.z + (gridPosition.z > 0 ? (ChunkSize) : 0)) * ChunkDiviser)
        );

        return chunkPosition;
    }
}