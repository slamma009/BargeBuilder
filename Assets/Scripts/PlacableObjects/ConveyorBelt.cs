using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ConveyorBelt : PlacableObject
{
    public List<GameObject> Walls;
    public int MaxItemsOnBelt = 3;

    [HideInInspector]
    public List<Rigidbody> ActiveRigidBodies = new List<Rigidbody>();

    private void OnTriggerEnter(Collider other)
    {
        Rigidbody rigid = other.GetComponent<Rigidbody>();
        if(rigid != null && !rigid.isKinematic)
        {
            ActiveRigidBodies.Add(rigid);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Rigidbody rigid = other.GetComponent<Rigidbody>();
        if (rigid != null && ActiveRigidBodies.Contains(rigid))
        {
            ActiveRigidBodies.Remove(rigid);
        }
    }

    private void FixedUpdate()
    {
        for(var i=ActiveRigidBodies.Count - 1; i >= 0; --i)
        {
            if (ActiveRigidBodies[i] != null)
            {
                Vector3 targetForce = transform.forward;
                ActiveRigidBodies[i].velocity = Vector3.Lerp(ActiveRigidBodies[i].velocity, targetForce, Time.deltaTime);
            }
            else
            {
                ActiveRigidBodies.RemoveAt(i);
            }
        }
    }

    public override void UpdateBlock(GridController gridController, Vector2 ChunkPos, Vector3 ChunkGridPos)
    {
        //Debug.Log("Checking block in chunk " + ChunkPos + " in grid position " + ChunkGridPos);
        //Vector3 forward = transform.forward;
        //forward = new Vector3(forward.x, forward.y, forward.z * -1); // for some reason I have to flip the z axis. My grid doesn't line up with unity's world grid apparently.
        //Vector3 sideChunkGridPosition = ChunkGridPos + forward;
        //Vector2 newChunkPosition = ChunkPos + Vector2.zero;
        //Vector3 newSideChunkGridPosition = sideChunkGridPosition;

        //// If the block we want to update is outside the x or z bounds of our chunk, find the new chunk and update it's chunkGridPosition accordingly.
        //if (sideChunkGridPosition.x < 0)
        //{
        //    newChunkPosition -= new Vector2(1, 0);
        //    newSideChunkGridPosition += new Vector3(chunkController.ChunkSize, 0, 0);
        //}
        //else if (sideChunkGridPosition.x >= chunkController.ChunkSize)
        //{
        //    newChunkPosition += new Vector2(1, 0);
        //    newSideChunkGridPosition -= new Vector3(chunkController.ChunkSize, 0, 0);
        //}
        //if (sideChunkGridPosition.z < 0)
        //{
        //    newChunkPosition -= new Vector2(0, 1);
        //    newSideChunkGridPosition += new Vector3(0, 0, chunkController.ChunkSize);
        //}
        //else if (sideChunkGridPosition.z >= chunkController.ChunkSize)
        //{
        //    newChunkPosition += new Vector2(0, 1);
        //    newSideChunkGridPosition -= new Vector3(0, 0, chunkController.ChunkSize);
        //}
        //// make sure the chunk and object exists before updating other blocks. Save a timestamp so we don't create an infinite loop of blocks updating each other.
        //if (chunkController.Chunks.ContainsKey(newChunkPosition))
        //{
        //    ChunkObject foundChunk = chunkController.Chunks[newChunkPosition].ChunkObjects[(int)newSideChunkGridPosition.x, (int)newSideChunkGridPosition.y, (int)newSideChunkGridPosition.z];
        //    if (foundChunk != null)
        //    {
        //        // I'm using a chunk system very similar to minecraft. Above this is a bunch of logic to find the block in front of the conveyor belt you are placing. 
        //        // I now want to get the belt code, disable the wall in front of our current conveyor belt since it's feeding into something, and disable the wall that 
        //        // we are feeding into on the new belt

        //        // Get the belt object
        //        ConveyorBelt belt = foundChunk.Object.GetComponent<ConveyorBelt>();
        //        if (belt != null)
        //        {
        //            // Disable our forward wall since we are feeding into another belt.
        //            Walls.SingleOrDefault(x => x.name == "Wall Front").gameObject.SetActive(false);

        //            // Calculate the direction the conveyor belt is feeding into. I.E. if the conveyor new conveyor belt is left of the existing one, we want the direction to be left so we can disable the left wall.
        //            // The AngleAxis rotates the direction with the object, so the left will be relative to it's rotation. This way left will always line up with the left wall, and right with right, etc.
        //            Vector3 inputDirection = Quaternion.AngleAxis(belt.transform.rotation.y, Vector3.up) * (-1 * transform.forward);

        //            // Checking the forward and back walls to disable them, I need to add left and right still.
        //            inputDirection = new Vector3(Mathf.Round(inputDirection.x), Mathf.Round(inputDirection.y), Mathf.Round(inputDirection.z));
        //            Debug.Log("Found Conveyor from: " + inputDirection + " || " + (belt.transform.forward * -1) + " || " + (inputDirection == belt.transform.forward * -1));

        //            if (inputDirection == belt.transform.forward)
        //            {
        //                belt.Walls.SingleOrDefault(x => x.name == "Wall Front").gameObject.SetActive(false);
        //            }
        //            else if (inputDirection == belt.transform.forward * -1)
        //            {
        //                belt.Walls.SingleOrDefault(x => x.name == "Wall Back").gameObject.SetActive(false);
        //            }
        //            else if (inputDirection == belt.transform.right * -1)
        //            {
        //                belt.Walls.SingleOrDefault(x => x.name == "Wall Left").gameObject.SetActive(false);
        //            }
        //            else if (inputDirection == belt.transform.right)
        //            {
        //                belt.Walls.SingleOrDefault(x => x.name == "Wall Right").gameObject.SetActive(false);
        //            }
        //        }
        //    }
        //}

       // if (chunkController.Chunks[ChunkPos].ChunkObjects[])
        base.UpdateBlock(gridController, ChunkPos, ChunkGridPos);
    }
}

