using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnchorSystem : MonoBehaviour {



    public float PlaceDistance = 1000;
    public float RotateSpeed = 50;
    public PlacableItemHolder[] PlacableItems;
    public Transform GhostAvatarPoint;
    public Material GhostMaterial;
    public Material GhostErrorMaterial;

    protected GameObject GhostObject;
    protected GameObject GhostAvatar;
    protected Vector3 GhostObjectHiddenPosition;

    protected int SelectedPrefab = 0;

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
    private bool ObjectCanBePlaced = true;
    private bool AnchorMode = true;

    private DraggableObject DraggedObject;

    // Called for initialization
    protected void Setup ()
    {
        GhostObjectHiddenPosition = Vector3.zero + Vector3.up * -100;
    }

    // Rotates the item the user is trying to place by 90 degrees
    protected void RotateGhostItem(float rawInput, bool buttonUp)
    {
        if (!AnchorMode)
        {
            GhostObject.transform.Rotate(Vector3.up * Time.deltaTime * rawInput * RotateSpeed);
        }
        else if(buttonUp)
        {
            GhostObject.transform.Rotate(new Vector3(0, 90, 0));
        }
    }

    protected void RaycastLogic(Ray ray, bool buttonUp, bool buttonDown)
    {
        //Raycast out and send the data to the GridController to be placed.
        RaycastHit hit;
        LayerMask layerMask = ~(1 << LayerMask.NameToLayer("Trigger"));
        if (Physics.Raycast(ray, out hit, PlaceDistance, layerMask))
        {
            if (RaycastHitTarget != null)
                RaycastHitTarget(hit);

            //if (DestructionMode)
            //{
            //    if (hit.collider.tag == "Placable")
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
                if (AnchorMode)
                    AnchorMode = !AnchorMode;

                HitPlacable(hit, buttonUp);
            } 
            else if(hit.transform.tag == "Anchor")
            {
                if (!AnchorMode)
                {
                    AnchorMode = !AnchorMode;

                    // Snap the ghost objects rotation to our anchors rotation
                    float angle = Vector3.SignedAngle(hit.transform.forward, GhostObject.transform.forward, Vector3.up);
                    angle = angle / 90f;
                    angle = Mathf.Round(angle) * 90f;
                    GhostObject.transform.rotation = Quaternion.LookRotation(Quaternion.AngleAxis(angle, Vector3.up) * hit.transform.forward, Vector3.up);
                }

                if(!DragMode || DraggedObject == null || DraggedObject.Anchors[0] == hit.transform.gameObject || hit.transform.parent.tag != "ConveyorBelt")
                    GhostObject.transform.position = hit.transform.position + hit.transform.forward * -0.99f;
                else
                {
                    GhostObject.transform.position = GhostObjectHiddenPosition;
                    DraggedObject.Anchors[1] = hit.transform.gameObject;
                }

                if (buttonDown)
                {
                    if (PlacableItems[SelectedPrefab].id == "ConveyorBelt" && hit.transform.parent.tag == "ConveyorBelt")
                    {
                        DragMode = true;
                        GameObject curve = Instantiate(PlacableItems[SelectedPrefab].Prefabs[1], Vector3.zero, Quaternion.identity);
                        DraggedObject = curve.GetComponent<DraggableObject>();
                        DraggedObject.Initialize(hit.transform.gameObject, GhostObject);
                    }
                }

                if (buttonUp)
                {
                    // Bezier curve logic
                    bool placeGhost = true;
                    if (DragMode)
                    {
                        if(DraggedObject != null)
                        {
                            if(DraggedObject.Anchors[0] == hit.collider.gameObject)
                            {
                                Destroy(DraggedObject.gameObject);
                                DragMode = false;
                            }
                            else if(hit.transform.parent.tag == "ConveyorBelt")
                            {
                                placeGhost = false;
                                DraggedObject.ObjectPlaced(hit.transform.gameObject);
                                DraggedObject = null;
                                DragMode = false;
                            }
                        }
                    }


                    if (ObjectCanBePlaced && placeGhost)
                    {
                        GameObject newObj = Instantiate(PlacableItems[SelectedPrefab].Prefabs[0], GhostObject.transform.position, GhostObject.transform.rotation);
                        if (DraggedObject != null && DragMode)
                        {
                            Debug.Log("HIT");
                            DraggedObject.ObjectPlaced(newObj.GetComponent<ConveyorBelt>());
                            DraggedObject = null;
                        }
                    }
                }
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

    public void HitPlacable(RaycastHit hit, bool placeObject)
    {
        GhostObject.transform.position = hit.point + Vector3.up;

        if (placeObject)
        {

            if (ObjectCanBePlaced)
            {
                GameObject newObj = Instantiate(PlacableItems[SelectedPrefab].Prefabs[0], GhostObject.transform.position, GhostObject.transform.rotation);
                if (DraggedObject != null && DragMode)
                {
                    DraggedObject.ObjectPlaced(newObj.GetComponent<ConveyorBelt>());
                    DraggedObject = null;
                }
            }
            DragMode = false;
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
        tempObject.transform.rotation = GhostObject == null ? Quaternion.identity : GhostObject.transform.rotation;

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
