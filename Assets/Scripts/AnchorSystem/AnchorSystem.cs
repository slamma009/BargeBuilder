﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnchorSystem : MonoBehaviour {



    public float PlaceDistance = 1000;
    public float RotateSpeed = 50;
    public InventoryItem[] PlacableItems;
    public Inventory HotbarInventory;
    public bool UseHotbar = true;
    public Transform GhostAvatarPoint;
    public Material GhostMaterial;
    public Material GhostErrorMaterial;

    protected GameObject GhostObject;
    protected PlacableObject GhostPlacable;
    protected GameObject GhostAvatar;
    protected Vector3 GhostObjectHiddenPosition;

    protected List<Collider> GhostColliders = new List<Collider>();
    public List<GameObject> DraggableCollidedObjects = new List<GameObject>();

    protected int _placableItemIndex = -1;
    protected int _hotbarItemId;

    protected bool DestructionMode;
    protected bool PlacingModeEnabled = false; // In placing mode
    protected bool DragMode;

    protected delegate void RaycastConnect(RaycastHit hit);
    protected delegate void EmptyMethod();

    protected RaycastConnect RaycastHitTarget;
    protected EmptyMethod RaycastMissTarget;
    protected EmptyMethod PlayingModeChanged;

    private Vector3 SavedGridPosition;
    private int SavedRotation = 0;
    private bool AnchorMode = true;

    private DraggableObject DraggedObject;


    /// <summary>
    /// Initializes the class
    /// </summary>
    protected void Setup ()
    {
        GhostObjectHiddenPosition = Vector3.zero + Vector3.up * -100;
    }

    /// <summary>
    /// Rotates the item the user is trying to place
    /// </summary>
    protected void RotateGhostItem(float rawInput, bool buttonUp)
    {
        if (!AnchorMode)
        {
            GhostObject.transform.Rotate(Vector3.up * Time.deltaTime * rawInput * RotateSpeed);
        }
        else if(buttonUp)
        {
            GhostObject.transform.Rotate(new Vector3(0, 90 * (rawInput > 0 ? 1 : -1), 0));
        }
    }

    /// <summary>
    /// Main Method to handle placing objects in the world.
    /// </summary>
    protected void RaycastLogic(Ray ray, bool buttonUp, bool buttonDown)
    {
        //Raycast out and send the data to the GridController to be placed.
        RaycastHit hit;
        LayerMask layerMask = ~((1 << LayerMask.NameToLayer("Trigger")) | 1 << LayerMask.NameToLayer("PlacementCollider"));
        if (Physics.Raycast(ray, out hit, PlaceDistance, layerMask))
        {
            if (RaycastHitTarget != null)
                RaycastHitTarget(hit);

            //if (DestructionMode)
            //{
            //    if (hit.collider.tag == "PlacementCollider")
            //    {
            //        //GhostDeleteMarker.transform.position = hit.collider.transform.position;
            //        // Check for the right hand trigger
            //        if (placeObject)
            //        {
            //            //gridController.DeleteObject(hit.collider.transform.position);
            //        }
            //    }
            //    else
            //    {
            //        //GhostDeleteMarker.transform.position = GhostObjectHiddenPosition;
            //    }
            //}
            //else

            if (hit.transform.tag == "Placable")
            {
                GhostObject.transform.position = hit.point + Vector3.up * GetSelectedItem().PlaceHeight;

                // Try snapping to nearby anchors first
                GameObject closestAnchor = null;
                float closestDistance = 100;
                foreach(AnchorObject anchor in GhostPlacable.Anchors)
                {
                    // Find all colliders within 1 unit of the center of the anchor
                    Collider[] colliders = Physics.OverlapSphere(anchor.Anchor.transform.position, 1, LayerMask.GetMask("Anchor"));
                    foreach(Collider c in colliders)
                    {
                        // Make sure it's not 1 of our own anchors, should be impossible though
                        if(c.transform.GetTopParent() != GhostObject.transform)
                        {
                            // Calculate the distance between the 2 points
                            // TODO: Use DistanceUnsquared
                            float distance = Vector3.Distance(c.transform.position, anchor.Anchor.transform.position);

                            // If the new anchor is closer then our last one, save it.
                            if(distance < closestDistance)
                            {
                                closestDistance = distance;
                                closestAnchor = c.gameObject;
                            }
                        }
                    }
                }

                if (closestAnchor == null)
                {
                    if (AnchorMode)
                        AnchorMode = !AnchorMode;

                    HitTerrain(hit, buttonUp);
                }
                else
                {

                    HitAnchor(closestAnchor, buttonUp, buttonDown);
                }
                
            } 
            else if(hit.transform.tag == "Anchor")
            {
                HitAnchor(hit.collider.gameObject, buttonUp, buttonDown);
            }
            else if (GhostObject.transform.position != GhostObjectHiddenPosition)
            {
                // Hide the ghost object since we're not hitting a placable surface
                GhostObject.transform.position = GhostObjectHiddenPosition;
            }
        }
        else
        {
            if (RaycastMissTarget != null)
                RaycastMissTarget();

            // Since nothing is hit, hide the ghost object
            if (GhostObject.transform.position != GhostObjectHiddenPosition )//||
                //GhostDeleteMarker.transform.position != GhostObjectHiddenPosition)
            {
                GhostObject.transform.position = GhostObjectHiddenPosition;
                //GhostDeleteMarker.transform.position = GhostObjectHiddenPosition;
            }
        }
    }


    /// <summary>
    /// Raycast has hit an anchor object
    /// </summary>
    private void HitAnchor(GameObject anchor, bool buttonUp, bool buttonDown)
    {

        if (!AnchorMode)
        {
            AnchorMode = !AnchorMode;

            // Snap the ghost objects rotation to our anchors rotation
            float angle = Vector3.SignedAngle(anchor.transform.forward, GhostObject.transform.forward, Vector3.up);
            angle = angle / 90f;
            angle = Mathf.Round(angle) * 90f;
            GhostObject.transform.rotation = Quaternion.LookRotation(Quaternion.AngleAxis(angle, Vector3.up) * anchor.transform.forward, Vector3.up);
        }

        // If we're not dragging something. Otherwise check if it's the same point we're dragging from. Otherwise make sure it's not a conveyor belt.
        if (!DragMode || DraggedObject == null || DraggedObject.Anchors[0] == anchor || anchor.transform.parent.tag != "ConveyorBelt")
        {
            GhostObject.transform.position = anchor.transform.position + anchor.transform.forward * -0.99f;
            if(DragMode && DraggedObject != null && DraggedObject.Anchors[0] == anchor)
            {
                if (DraggedObject.gameObject.activeSelf)
                    DraggedObject.gameObject.SetActive(false);
            }
        }
        else
        {
            if (DragMode && DraggedObject != null)
            {
                if (!DraggedObject.gameObject.activeSelf)
                    DraggedObject.gameObject.SetActive(true);
            }
            // Since we have to be hitting a different conveyor belt then our first anchor, hide the ghost object and snap the dragged object to it.
            GhostObject.transform.position = GhostObjectHiddenPosition;
            DraggedObject.Anchors[1] = anchor;
        }

        // Call collision logic
        bool ObjectCanBePlaced = !GhostObjectIsColliding();

        if (ObjectCanBePlaced && buttonDown)
        {
            //TODO: Replace ConveyorBelt check with id value
            if (GetSelectedItem().Tag == EItemTag.ConveyorBelt && anchor.transform.parent.tag == EItemTag.ConveyorBelt.ToString())
            {
                DragMode = true;
                GameObject curve = Instantiate(GetSelectedItem().SecondaryPrefabs[0], Vector3.zero, Quaternion.identity);
                DraggedObject = curve.GetComponent<ConveyorBeltBezeir>();
                DraggedObject.Initialize(anchor, GhostObject);

                BezierTrigger colliderStuff = DraggedObject.gameObject.AddComponent<BezierTrigger>();
                colliderStuff.dObj = DraggedObject;
                colliderStuff.AnchorSys = this;
            }
        }

        if (buttonUp)
        {
            // Bezier curve logic
            bool placeGhost = true;
            if (DragMode)
            {
                if (DraggedObject != null)
                {
                    if (!ObjectCanBePlaced || DraggedObject.Anchors[0] == anchor)
                    {
                        ExitDragMode(true);
                    }
                    else if (anchor.transform.parent.tag == "ConveyorBelt")
                    {
                        placeGhost = false;
                        DraggedObject.ObjectPlaced(anchor);
                        ExitDragMode(false);
                    }
                }
            }


            if (ObjectCanBePlaced && placeGhost)
            {
                GameObject newObj = Instantiate(GetSelectedItem().Prefab, GhostObject.transform.position, GhostObject.transform.rotation);
                newObj.GetComponent<PlacableObject>().ObjectPlaced();

                HandleInventoryAfterPlace();

                if (DraggedObject != null && DragMode)
                {
                    DraggedObject.ObjectPlaced(newObj.GetComponent<ConveyorBelt>());
                    ExitDragMode(false);
                }
            }
        }
    }

    /// <summary>
    /// Raycast has hit the terrain
    /// </summary>
    private void HitTerrain(RaycastHit hit, bool placeObject)
    {
        // Set the Ghost Object Position
        GhostObject.transform.position = hit.point + Vector3.up * GetSelectedItem().PlaceHeight;
        if(DraggedObject != null && !DraggedObject.gameObject.activeSelf)
                DraggedObject.gameObject.SetActive(true);
        // If the dragged object is still anchoring to the last anchor, switch it to follow the ghost object again.
        if (DraggedObject != null && DraggedObject.Anchors[1] != null)
        {
            
            DraggedObject.Anchors[1] = null;
        }

        // Call collision logic
        bool ObjectCanBePlaced = !GhostObjectIsColliding();

        // Attempt ot place the item.
        if (placeObject)
        {

            if (ObjectCanBePlaced)
            {
                // Place the item we have selected
                GameObject newObj = Instantiate(GetSelectedItem().Prefab, GhostObject.transform.position, GhostObject.transform.rotation);
                newObj.GetComponent<PlacableObject>().ObjectPlaced();

                HandleInventoryAfterPlace();

                // Check if we are draging an item and place it too
                if (DraggedObject != null && DragMode)
                {
                    DraggedObject.ObjectPlaced(newObj.GetComponent<ConveyorBelt>());
                    ExitDragMode(false);
                }
            }
            else
            {
                // Just in case we have an item that's being dragged, lets exit
                ExitDragMode(true);
            }
        }
    }

    /// <summary>
    /// Checks if selected item in hot bar is still available
    /// </summary>
    /// <returns>True if selected item still exists in hotbar</returns>
    protected bool CheckSelectedItem()
    {
        if (!UseHotbar)
            return true;

        return _placableItemIndex >= 0 && HotbarInventory.InventorySlots[_placableItemIndex].Amount > 0;
    }

    /// <summary>
    /// Attempts to remove 1 of the selected items from the hotbar
    /// </summary>
    protected void HandleInventoryAfterPlace()
    {
        // If using hotbar, remove 1 item.
        if (UseHotbar)
        {
            int amount = HotbarInventory.RemoveByIndex(_placableItemIndex, 1);

            // If all items were used, then change prefab
            if (HotbarInventory.InventorySlots[_placableItemIndex].Amount == 0)
            {
                ChangePrefabsWithHotbar();
            }
        }
    }

    private void ExitDragMode(bool destroyObject)
    {
        DraggableCollidedObjects.Clear();
        Destroy(DraggedObject.GetComponent<BezierTrigger>());

        DragMode = false;
        if(destroyObject)
            Destroy(DraggedObject.gameObject);
        else 
            DraggedObject = null;
    }

    protected void EnablePlacingMode(bool enabling)
    {
        if (enabling)
        {
            if(_placableItemIndex >= 0)
                _placableItemIndex -= 1;
            ChangePrefabs();

            if (_placableItemIndex >= 0)
                PlacingModeEnabled = true;

        }
        else
        {
            PlacingModeEnabled = false;
            _hotbarItemId = -1;

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

    /// <summary>
    /// Cycles through placable prefabs
    /// </summary>
    protected void ChangePrefabs()
    {
        if (UseHotbar)
            ChangePrefabsWithHotbar();
        else
            ChangePrefabsWithPlacableItemsArray();
    }

    /// <summary>
    /// Utalized the Hotbar for Survival like placement
    /// </summary>
    protected void ChangePrefabsWithHotbar()
    {
        int valueBefore = _placableItemIndex;
        for (var i = _placableItemIndex + 1; i != _placableItemIndex; ++i)
        {

            if (HotbarInventory.InventorySlots[i].Amount > 0)
            {
                if (HotbarInventory.InventorySlots[i].Item.IsPlacable)
                {
                    // If we don't have any items left in our selected slot
                    // we need to change to the new slot
                    if (_placableItemIndex >= 0
                        && HotbarInventory.InventorySlots[_placableItemIndex].Amount == 0)
                    {
                        _placableItemIndex = i;

                        // If the new item is different then the current one, update the ghost avatar
                        if (HotbarInventory.InventorySlots[i].Item.ID != _hotbarItemId)
                        {
                            _hotbarItemId = HotbarInventory.InventorySlots[i].Item.ID;
                            ChangePrefab(HotbarInventory.InventorySlots[i].Item.Prefab);
                        }

                        break;
                    }
                    // Will trigger if we've found a new item to select
                    else if (HotbarInventory.InventorySlots[i].Item.ID != _hotbarItemId)
                    {
                        _placableItemIndex = i;
                        _hotbarItemId = HotbarInventory.InventorySlots[i].Item.ID;
                        ChangePrefab(HotbarInventory.InventorySlots[i].Item.Prefab);
                        break;
                    }

                }
            }


            if (i == HotbarInventory.InventorySize - 1)
            {
                i = -1;

                if (_placableItemIndex <= -1)
                    break;
            }
        }

        // If we didn't find any new items to place, and we have none left in our current slot
        // reset the slots
        if (_placableItemIndex == valueBefore 
            && _placableItemIndex >= 0 
            && HotbarInventory.InventorySlots[_placableItemIndex].Amount == 0)
        {
            _placableItemIndex = -1;
            _hotbarItemId = -1;
            EnablePlacingMode(false);
        }
    }
    
    /// <summary>
    /// Utalized the PlacableItemsArray for Creative like placement
    /// </summary>
    protected void ChangePrefabsWithPlacableItemsArray()
    {
        _placableItemIndex += 1;
        if (_placableItemIndex >= PlacableItems.Length)
            _placableItemIndex = 0;

        ChangePrefab(GetSelectedItem().Prefab);
    }

    protected void ChangePrefab(GameObject prefab)
    { 
        // Instantiate the ghost avatar above the hand. 
        GameObject tempAvatar = Instantiate(prefab, GhostAvatarPoint.position, (GhostAvatar == null ? GhostAvatarPoint.rotation : GhostAvatar.transform.rotation)) as GameObject;
        PlacableObject poAvatar = tempAvatar.GetComponent<PlacableObject>();
        poAvatar.MakeGhost(null);
        Destroy(poAvatar);
        tempAvatar.transform.localScale = Vector3.one * 0.03f;
        if(GhostAvatarPoint.parent)
        tempAvatar.transform.parent = GhostAvatarPoint;

        if (GhostAvatar != null)
        {
            Destroy(GhostAvatar);
        }

        GhostAvatar = tempAvatar;


        // Instantiate the ghost object
        GameObject tempObject = Instantiate(prefab, (GhostObject == null ? Vector3.zero - Vector3.up * 5 : GhostObject.transform.position), Quaternion.identity) as GameObject;
        
        tempObject.GetComponent<PlacableObject>().MakeGhost(GhostMaterial);
        tempObject.transform.rotation = GhostObject == null ? Quaternion.identity : GhostObject.transform.rotation;


        if (GhostObject != null)
        {
            Destroy(GhostObject);
        }

        GhostObject = tempObject;
        GhostPlacable = GhostObject.GetComponent<PlacableObject>();
        GhostPlacable.IsGhost = true;
        GameObject colliderObject = Instantiate(prefab, GhostObject.transform.position, GhostObject.transform.rotation) as GameObject;
        colliderObject.GetComponent<PlacableObject>().MakeGhost(null, false);
        colliderObject.name = "COLLIDEROBJECT";
        colliderObject.transform.localScale = colliderObject.transform.localScale * 0.95f;
        colliderObject.transform.parent = tempObject.transform;
        GhostColliders = GetAllChildCollider(colliderObject, LayerMask.NameToLayer("PlacementCollider"));

    }

    /// <summary>
    /// Returns all the colliders for the object where their layer matches the provided mask.
    /// </summary>
    public List<Collider> GetAllChildCollider(GameObject obj, LayerMask? mask = null)
    {
        List<Collider> colliders = new List<Collider>();
        if (mask == null || obj.layer == mask.Value)
        {
            Collider c = obj.GetComponent<Collider>();
            if (c != null)
                colliders.Add(c);
        }
        int count = obj.transform.childCount;
        for (var i = 0; i < count; ++i)
        {
            Transform child = obj.transform.GetChild(i);
            colliders.AddRange(GetAllChildCollider(child.gameObject, mask));
        }

        return colliders;
    }

    /// <summary>
    /// Checks the Ghost Object for any collisions to prevent placement
    /// </summary>
    private bool GhostObjectIsColliding()
    {
        PlacableObject placableObject = GhostObject.GetComponent<PlacableObject>();

        if(DragMode && DraggedObject != null)
        {
            if (DraggableIsColliding())
            {
                placableObject.MakeGhost(GhostErrorMaterial);
                return true;
            }
        }
        // Get all colliders within 10 units of our ghost object.
        Collider[] others = Physics.OverlapSphere(GhostObject.transform.position, 10, LayerMask.GetMask("PlacementCollider"));
        foreach (Collider other in others)
        {
            // Make sure it's not a collider on the ghost object.
            if (other.transform.GetTopParent() != GhostObject.transform)
            {
                // Loop through all colliders on the ghost object and determine if anything is intersecting.
                foreach (Collider c in GhostColliders)
                {
                    Vector3 direction;
                    float distance;
                    if (Physics.ComputePenetration(other, other.transform.position, other.transform.rotation, c, c.transform.position, c.transform.rotation, out direction, out distance))
                    {
                        placableObject.MakeGhost(GhostErrorMaterial);
                        return true;
                    }
                }
            }
        }
        placableObject.MakeGhost(GhostMaterial);
        return false;
    }

    /// <summary>
    /// Checks the dragged object for any collisions to prevent placement
    /// </summary>
    private bool DraggableIsColliding()
    {
        if (DraggedObject == null || !DraggedObject.gameObject.activeSelf)
            return false;

        if (!DraggedObject.ObjectIsValid())
        {
            return true;
        }

        if (DraggableCollidedObjects.Count > 0)
        {
            return true;
        } 
        return false;
    }

    /// <summary>
    /// Returns the tag of the item
    /// </summary>
    /// <returns>The Tag of the item</returns>
    protected InventoryItem GetSelectedItem()
    {
        if (!UseHotbar)
            return PlacableItems[_placableItemIndex];
        else
            return HotbarInventory.InventorySlots[_placableItemIndex].Item;
    }
}
