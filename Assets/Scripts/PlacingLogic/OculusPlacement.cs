using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OculusPlacement : PlacingController
{
    
    public HandGrabbing RightHand;
    public HandGrabbing LeftHand;
    
    private LineRenderer GhostLine;
    private OculusInputControl InputControl;

    //private void Start()
    //{
    //    InputControl = GameObject.Find("InputObject").GetComponent<OculusInputControl>();

    //    GhostLine = transform.GetComponent<LineRenderer>();

    //    RaycastHitTarget = GhostLineToHit;
    //    RaycastMissTarget = GhostLineRay;
    //    PlayingModeChanged = HideGhostLine;

    //    Setup();
    //}

    //private void GhostLineToHit(RaycastHit hit)
    //{
    //    // Draw the ghost line from the hand to the hit point
    //    GhostLine.SetPosition(0, RightHand.transform.position);
    //    GhostLine.SetPosition(1, hit.point);
    //}

    //private void GhostLineRay()
    //{
    //    // Draw the ghost line from the hand to it's max distance
    //    GhostLine.SetPosition(0, RightHand.transform.position);
    //    GhostLine.SetPosition(1, RightHand.transform.position + RightHand.transform.forward * PlaceDistance);
    //}

    //private void HideGhostLine()
    //{
    //    GhostLine.SetPosition(0, GhostObjectHiddenPosition);
    //    GhostLine.SetPosition(1, GhostObjectHiddenPosition);
    //}

    //// Update is called once per frame
    //void Update()
    //{
    //    // Make sure both hands are onoccupied
    //    if (!RightHand.Grabbed && !LeftHand.Grabbed)
    //    {
    //        if (PlacingModeEnabled)
    //        {
    //            // Rotate the ghost avatar above the hand
    //            GhostAvatar.transform.Rotate(Vector3.up * Time.deltaTime * 30);

    //            if (!DestructionMode)
    //            {
    //                if (InputControl.GetButton("RightJoystickRight"))
    //                {
    //                    RotateGhostItem(true);
    //                }
    //                if (InputControl.GetButton("RightJoystickLeft"))
    //                {
    //                    RotateGhostItem(false);
    //                }
    //            }

    //            RaycastLogic(new Ray(RightHand.transform.position, RightHand.transform.forward), InputControl.GetButton("Oculus_CrossPlatform_SecondaryIndexTrigger"), false, false);


    //            if (InputControl.GetButton("Oculus_CrossPlatform_ButtonX"))
    //            {
    //                DestructionMode = !DestructionMode;

    //                GhostObject.transform.position = GhostObjectHiddenPosition;
    //                GhostDeleteMarker.transform.position = GhostObjectHiddenPosition;


    //            }

    //            // Check to exit placing mode
    //            if (InputControl.GetButton("Oculus_CrossPlatform_ButtonA"))
    //            {
    //                EnablePlacingMode(false);
    //            }
                

    //            if(InputControl.GetButton("Oculus_CrossPlatform_ButtonB"))
    //            {
    //                ChangePrefabs();
                    
    //            }
    //        }
    //        else
    //        {
    //            // Check to enter placing mode
    //            if (InputControl.GetButton("Oculus_CrossPlatform_ButtonA"))
    //            {
    //                PlacingModeEnabled = true;
    //                EnablePlacingMode(true);
    //            }
    //        }
    //    }
    //    else if (PlacingModeEnabled)
    //    {
    //        PlacingModeEnabled = false;
    //        EnablePlacingMode(false);
    //    }
    //}

}