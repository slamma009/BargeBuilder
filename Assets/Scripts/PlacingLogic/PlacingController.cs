using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacingController : MonoBehaviour {



    public float PlaceDistance = 1000;
    public PlacableItemHolder[] PlacableItems;
    public Transform GhostAvatarPoint;
    public GridController gridController;
    public Material GhostMaterial;
    public Material GhostErrorMaterial;
    public GameObject GhostDeleteMarker;

    protected GameObject GhostObject;
    protected GameObject GhostAvatar;
    protected Vector3 GhostObjectHiddenPosition;

    protected int SelectedPrefab = 0;
    protected int Rotation = 0;

    protected bool DestructionMode;
    protected bool PlacingModeEnabled = false; // In placing mode
    protected bool PlacingHeld;
    protected bool DragMode;

    protected delegate void RaycastConnect(RaycastHit hit);
    protected delegate void EmptyMethod();

    protected RaycastConnect RaycastHitTarget;
    protected EmptyMethod RaycastMissTarget;
    protected EmptyMethod PlayingModeChanged;

    private Vector3 SavedGridPosition;
    private int SavedRotation = 0;
    private bool ObjectCanBePlaced = true;

    private ConveyorBeltBezeir DraggedObject;

    // Called for initialization
    protected void Setup ()
    {
        GhostObjectHiddenPosition = Vector3.zero + Vector3.up * -100;
        GhostDeleteMarker.transform.localScale = Vector3.one * gridController.GridSize;
        GhostDeleteMarker.transform.position = GhostObjectHiddenPosition;
    }

    // Rotates the item the user is trying to place by 90 degrees
    protected void RotateGhostItem(bool right = true)
    {
        Rotation += right? 1 : -1;
        if (right && Rotation >= 4)
            Rotation = 0;

        if (!right && Rotation < 0)
            Rotation = 3;

        GhostObject.transform.rotation = Quaternion.Euler(0, 90 * Rotation, 0);
    }

    protected void RaycastLogic(Ray ray, bool placeObject, bool placeHeld, bool enterDragMode)
    {
        //Raycast out and send the data to the GridController to be placed.
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, PlaceDistance))
        {
            if (RaycastHitTarget != null)
                RaycastHitTarget(hit);

            if (DestructionMode)
            {
                if (hit.collider.tag == "Placable")
                {
                    GhostDeleteMarker.transform.position = hit.collider.transform.position;
                    // Check for the right hand trigger
                    if (placeObject)
                    {
                        gridController.DeleteObject(hit.collider.transform.position);
                    }
                }
                else
                {
                    GhostDeleteMarker.transform.position = GhostObjectHiddenPosition;
                }
            }
            else
            {
                if (hit.transform.tag == "Placable")
                {
                    GhostObject.transform.position = gridController.GetGridPosition(hit.point + ray.direction * -0.1f).Position;
                    GridPositionObject gridPositionObject = gridController.GetGridPosition(GhostObject.transform.position);

                    // Logic for determining if the object can be placed
                    if (SavedGridPosition != gridPositionObject.Position || SavedRotation != Rotation)
                    {
                        SavedRotation = Rotation;
                        SavedGridPosition = gridPositionObject.Position;
                        bool objectCantBePlaced = false;
                        PlacableObject placableObject = GhostObject.GetComponent<PlacableObject>();
                        //foreach (Transform anchor in placableObject.GridAnchors)
                        //{
                        //    if (!objectCantBePlaced)
                        //    {
                        //        GridPositionObject anchorGridPositionObject = gridController.GetGridPosition(anchor.position);
                        //        Vector2 chunkPosition = gridController.ChunkController.ConvertGridPositionToChunkPosition(anchorGridPositionObject.GridPosition);
                        //        Vector3Int chunkGridPosition = gridController.ChunkController.ConvertWorldGridPositionToChunkGridPosition(chunkPosition, anchorGridPositionObject.GridPosition);

                        //        if (gridController.ChunkController.Chunks[chunkPosition].ChunkObjects[chunkGridPosition] != null)
                        //        {
                        //            string output = "Error in chunk " + chunkPosition + " from grid " + chunkGridPosition + "\n > " + gridController.ChunkController.Chunks[chunkPosition].ChunkObjects[chunkGridPosition];
                        //            output += "\n > Reference Chunk: " + gridController.ChunkController.Chunks[chunkPosition].ChunkObjects[chunkGridPosition].ReferenceChunkPosition + ", Reference Grid: " + gridController.ChunkController.Chunks[chunkPosition].ChunkObjects[chunkGridPosition].ReferenceObjectPosition;
                        //            Debug.Log(output);
                        //           /// Debug.Log();
                        //            objectCantBePlaced = true;
                        //        }
                        //    }
                        //}

                        if (DragMode)
                        {
                            bool dragPlacable = DraggedObject.UpdatePosition(gridPositionObject);

                            objectCantBePlaced = objectCantBePlaced || !dragPlacable;
                        }

                        if (objectCantBePlaced)
                        {
                            placableObject.MakeGhost(GhostErrorMaterial);
                        }
                        else
                        {
                            placableObject.MakeGhost(GhostMaterial);
                        }

                        ObjectCanBePlaced = !objectCantBePlaced;
                    }

                    if (placeObject && ObjectCanBePlaced)
                    {
                        gridController.PlaceObject(PlacableItems[SelectedPrefab].Prefabs[0], gridPositionObject, GhostObject.transform.rotation);

                        if(enterDragMode && PlacableItems[SelectedPrefab].id == "ConveyorCurve")
                        {
                            DraggedObject = Instantiate(PlacableItems[SelectedPrefab].Prefabs[2], Vector3.zero, Quaternion.identity).GetComponent<ConveyorBeltBezeir>();
                            DraggedObject.SetFirstPosition(gridPositionObject, GhostObject.transform.forward);
                        }

                        if (!DragMode && enterDragMode && PlacableItems[SelectedPrefab].Prefabs.Length > 1)
                        {
                            DragMode = true;
                        }
                    }

                    if(DragMode && !placeHeld)
                    {
                        DragMode = false;
                        gridController.PlaceObject(PlacableItems[SelectedPrefab].Prefabs[1], gridPositionObject, Quaternion.LookRotation(DraggedObject.SetLastPosition(gridPositionObject)));
                    }
                }
                else if (GhostObject.transform.position != GhostObjectHiddenPosition)
                {
                    // Hide the ghost object since we're not hitting a placable surface
                    GhostObject.transform.position = GhostObjectHiddenPosition;
                }
            }
        }
        else
        {
            if (RaycastMissTarget != null)
                RaycastMissTarget();
            // Since nothing is hit, hide the ghost object
            if (GhostObject.transform.position != GhostObjectHiddenPosition ||
                GhostDeleteMarker.transform.position != GhostObjectHiddenPosition)
            {
                GhostObject.transform.position = GhostObjectHiddenPosition;
                GhostDeleteMarker.transform.position = GhostObjectHiddenPosition;
            }
        }
    }

    // Cycles through placable prefabs
    protected void ChangePrefabs()
    {
        SelectedPrefab += 1;
        if (SelectedPrefab >= PlacableItems.Length)
            SelectedPrefab = 0;

        // Instantiate the ghost avatar above the hand. 
        GameObject tempAvatar = Instantiate(PlacableItems[SelectedPrefab].Prefabs[0], GhostAvatarPoint.position, (GhostAvatar == null ? GhostAvatarPoint.rotation : GhostAvatar.transform.rotation)) as GameObject;
        tempAvatar.GetComponent<PlacableObject>().MakeGhost(null);
        tempAvatar.transform.localScale = Vector3.one * 0.03f;
        tempAvatar.transform.parent = GhostAvatarPoint.parent;

        if (GhostAvatar != null)
        {
            Destroy(GhostAvatar);
        }

        GhostAvatar = tempAvatar;


        // Instantiate the ghost object
        GameObject tempObject = Instantiate(PlacableItems[SelectedPrefab].Prefabs[0], (GhostObject == null ? Vector3.zero - Vector3.up * 5 : GhostObject.transform.position), Quaternion.identity) as GameObject;
        tempObject.GetComponent<PlacableObject>().MakeGhost(GhostMaterial);
        tempObject.transform.rotation = Quaternion.Euler(0, 90 * Rotation, 0);

        if (GhostObject != null)
        {
            Destroy(GhostObject);
        }

        GhostObject = tempObject;

    }

    protected void EnablePlacingMode(bool enabling)
    {
        if (enabling)
        {
            PlacingModeEnabled = true;
            SelectedPrefab -= 1;
            ChangePrefabs();

        }
        else
        {
            PlacingModeEnabled = false;

            if (GhostObject != null)
            {
                Destroy(GhostObject);
            }

            if (GhostAvatar != null)
            {
                Destroy(GhostAvatar);
            }

        }

        if (PlayingModeChanged != null)
            PlayingModeChanged();
        
    }
}
