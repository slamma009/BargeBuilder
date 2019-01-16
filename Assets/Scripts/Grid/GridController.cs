using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridController : MonoBehaviour {

    public int GridSize = 2;
    
    public ChunkHandler ChunkController;
    private Transform PlayerObject;
    private float Scale;
    private float ReverseScale;

    private Vector3 PlayersLastGridPosition;

    public void Awake()
    {
        Scale = 1f / (float)GridSize;
        ReverseScale = (float)GridSize / 1f;
        PlayerObject = GameObject.FindGameObjectWithTag("Player").transform;

        ChunkController = new ChunkHandler();
        PlayersLastGridPosition = GetGridPosition(PlayerObject.position).GridPosition;
        ChunkController.LoadChunks(PlayersLastGridPosition);

        //Vector3 forward = new Vector3(1,1,1);
        //for (var i = 0; i < 4; ++i)
        //{
        //    Debug.Log(forward);
        //    forward = Quaternion.AngleAxis(90, Vector3.up) * forward;
        //}
    }

    // Places the object in the world
    public void PlaceObject(GameObject obj, GridPositionObject gridPositionObject, Quaternion rotation)
    {
        // Convert the position to the grid position, and instantiate the object centered on the grid position
        GameObject newObj = Instantiate(obj, gridPositionObject.Position, rotation) as GameObject;
        newObj.name = gridPositionObject.GridPosition.x + ", " + gridPositionObject.GridPosition.y + ", " + gridPositionObject.GridPosition.z;

        // Find the chunk and chunkgridposition of the new object.
        Vector2 chunkPosition = ChunkController.ConvertGridPositionToChunkPosition(gridPositionObject.GridPosition);
        Vector3Int chunkGridPosition = ChunkController.ConvertWorldGridPositionToChunkGridPosition(chunkPosition, gridPositionObject.GridPosition);

        // Add the object to the chunkobjects in the appropiate chunk
        PlacableObject placableObject = newObj.GetComponent<PlacableObject>();
        ChunkController.Chunks[chunkPosition].ChunkObjects[chunkGridPosition] = new ChunkObject(placableObject, ChunkController.Chunks[chunkPosition]);
        //Debug.Log("Object placed in chunk " + chunkPosition + "at ChunkGridPosition " + chunkGridPosition);

        // For each anchor reference that chunk to the main chunk object.Used for multi - block objects
        //foreach (Transform anchor in placableObject.GridAnchors)
        //{
        //    GridPositionObject anchorGridPositionObject = GetGridPosition(anchor.position);
        //    Vector2 anchorChunkPosition = ChunkController.ConvertGridPositionToChunkPosition(anchorGridPositionObject.GridPosition);
        //    Vector3Int anchorChunkGridPosition = ChunkController.ConvertWorldGridPositionToChunkGridPosition(anchorChunkPosition, anchorGridPositionObject.GridPosition);
            
        //    ChunkController.Chunks[anchorChunkPosition].ChunkObjects[anchorChunkGridPosition] = new ChunkObject(chunkPosition, chunkGridPosition, anchorChunkGridPosition, ChunkController.Chunks[anchorChunkPosition]);
        //    //Debug.Log(" > Object referenced in chunk " + anchorChunkPosition + "at ChunkGridPosition " + anchorChunkGridPosition);
        //}

        UpdateBlocks(chunkPosition, chunkGridPosition);
        


    }

    public void DeleteObject(Vector3 worldPosition)
    {
        // Convert the world position to the chunk position and chunkgridposition
        Vector3 gridPosition = GetGridPosition(worldPosition).GridPosition;
        Vector2 chunkPosition = ChunkController.ConvertGridPositionToChunkPosition(gridPosition);
        Vector3Int chunkGridPosition = ChunkController.ConvertWorldGridPositionToChunkGridPosition(chunkPosition, gridPosition);
        //Debug.Log("Object removed in chunk " + chunkPosition + "at ChunkGridPosition " + chunkGridPosition);
        // Destroy the object in question, and remove it from the chunkobjects list.
        if (ChunkController.Chunks[chunkPosition].ChunkObjects[chunkGridPosition].IsReference)
        {
            Vector3 newChunkPos = ChunkController.Chunks[chunkPosition].ChunkObjects[chunkGridPosition].ReferenceChunkPosition;
            Vector3Int newChunkGridPos = ChunkController.Chunks[chunkPosition].ChunkObjects[chunkGridPosition].ReferenceObjectPosition;

            chunkPosition = newChunkPos;
            chunkGridPosition = newChunkGridPos;
        }
        Destroy(ChunkController.Chunks[chunkPosition].ChunkObjects[chunkGridPosition].Object.gameObject);
        ChunkController.Chunks[chunkPosition].ChunkObjects[chunkGridPosition] = null;
        UpdateBlocks(chunkPosition, chunkGridPosition);
    }

    public void UpdateBlocks(Vector2 chunkPosition, Vector3Int chunkGridPosition)
    {
        // grid positions of all side objects to the block that's being updated
        List<Vector3> sideChunkObjects = new List<Vector3>();
        sideChunkObjects.Add(chunkGridPosition - Vector3.right);
        sideChunkObjects.Add(chunkGridPosition + Vector3.right);
        sideChunkObjects.Add(chunkGridPosition - Vector3.forward);
        sideChunkObjects.Add(chunkGridPosition + Vector3.forward);
        sideChunkObjects.Add(chunkGridPosition - Vector3.up);
        sideChunkObjects.Add(chunkGridPosition + Vector3.up);
        
        foreach(Vector3 sideChunkGridPosition in sideChunkObjects)
        {
            // Since chunks dont stack vertially, if the block we want to update is outside the y constraints, it won't exist.
            if (sideChunkGridPosition.y >= 0 && sideChunkGridPosition.y < ChunkController.ChunkSize)
            {
                Vector2 newChunkPosition = chunkPosition + Vector2.zero;
                Vector3Int newSideChunkGridPosition = new Vector3Int(Mathf.RoundToInt(sideChunkGridPosition.x), Mathf.RoundToInt(sideChunkGridPosition.y), Mathf.RoundToInt(sideChunkGridPosition.z));
                // If the block we want to update is outside the x or z bounds of our chunk, find the new chunk and update it's chunkGridPosition accordingly.
                if (sideChunkGridPosition.x < 0)
                {
                    newChunkPosition -= new Vector2(1, 0);
                    newSideChunkGridPosition += new Vector3Int(ChunkController.ChunkSize, 0, 0);
                }
                else if(sideChunkGridPosition.x >= ChunkController.ChunkSize)
                {
                    newChunkPosition += new Vector2(1, 0);
                    newSideChunkGridPosition -= new Vector3Int(ChunkController.ChunkSize, 0, 0);
                }
                if (sideChunkGridPosition.z < 0)
                {
                    newChunkPosition += new Vector2(0, 1);
                    newSideChunkGridPosition += new Vector3Int(0, 0, ChunkController.ChunkSize);
                }
                else if (sideChunkGridPosition.z >= ChunkController.ChunkSize)
                {
                    newChunkPosition -= new Vector2(0, 1);
                    newSideChunkGridPosition -= new Vector3Int(0, 0, ChunkController.ChunkSize);
                }
                //Debug.Log("Original > Chunk: " + chunkPosition + ", grid: " + chunkGridPosition);
                //Debug.Log("New > Chunk: " + newChunkPosition + ", grid: " + newSideChunkGridPosition);

                // make sure the chunk and object exists before updating other blocks. Save a timestamp so we don't create an infinite loop of blocks updating each other.
                if (ChunkController.Chunks.ContainsKey(newChunkPosition))
                {
                    ChunkObject foundChunk = ChunkController.Chunks[newChunkPosition].ChunkObjects[newSideChunkGridPosition];
                    if (foundChunk != null && foundChunk.LastUpdateTimestamp != Time.timeSinceLevelLoad)
                    {
                        foundChunk.LastUpdateTimestamp = Time.timeSinceLevelLoad;
                        UpdateBlocks(newChunkPosition, newSideChunkGridPosition);
                    }
                }
            }
        }

        // now that we've called every side block, update ourselves.
        if (ChunkController.Chunks[chunkPosition].ChunkObjects[chunkGridPosition] != null)
        {
            ChunkController.Chunks[chunkPosition].ChunkObjects[chunkGridPosition].UpdateBlock(this, chunkPosition, chunkGridPosition);
        }
    }

    public void DeleteReference(Vector2 chunkPosition, Vector3Int chunkGridPosition)
    {
        ChunkController.Chunks[chunkPosition].ChunkObjects[chunkGridPosition] = null;
    }

    // Changes the given position to the correct grid position
    public GridPositionObject GetGridPosition(Vector3 position)
    {
        position *= Scale;
        Vector3 gridPosition = new Vector3(Mathf.Floor(position.x + 0.5f), Mathf.Floor(position.y), Mathf.Floor(position.z + 0.5f));
        position = gridPosition * ReverseScale;
        position.y += 0.5f * GridSize;

        return new GridPositionObject () { Position = position, GridPosition = gridPosition};
    }

    private void FixedUpdate()
    {
        GridPositionObject playerPositionOnGrid = GetGridPosition(PlayerObject.transform.position);
        if(PlayersLastGridPosition != playerPositionOnGrid.GridPosition)
        {
            PlayersLastGridPosition = playerPositionOnGrid.GridPosition;
            ChunkController.LoadChunks(PlayersLastGridPosition);
        }
    }
}
