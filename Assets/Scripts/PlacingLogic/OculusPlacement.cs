using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class OculusPlacement : AnchorSystem
{
    
    public HandGrabbing RightHand;
    public HandGrabbing LeftHand;
    
    private LineRenderer GhostLine;
    
    private void Start()
    {

        GhostLine = transform.GetComponent<LineRenderer>();

        RaycastHitTarget = GhostLineToHit;
        RaycastMissTarget = GhostLineRay;
        PlayingModeChanged = HideGhostLine;

        Setup();
    }

    private void GhostLineToHit(RaycastHit hit)
    {
        // Draw the ghost line from the hand to the hit point
        GhostLine.SetPosition(0, RightHand.transform.position);
        GhostLine.SetPosition(1, hit.point);
    }

    private void GhostLineRay()
    {
        // Draw the ghost line from the hand to it's max distance
        GhostLine.SetPosition(0, RightHand.transform.position);
        Vector3 rayDir = RightHand.transform.forward;
        rayDir = Quaternion.AngleAxis(35, RightHand.transform.right) * rayDir;
        GhostLine.SetPosition(1, RightHand.transform.position + rayDir * PlaceDistance);
    }

    private void HideGhostLine()
    {
        GhostLine.SetPosition(0, GhostObjectHiddenPosition);
        GhostLine.SetPosition(1, GhostObjectHiddenPosition);
    }
    bool RotatingObject = false;
    // Update is called once per frame
    void Update()
    {
        // Make sure both hands are onoccupied
        //if (!RightHand.Grabbed && !LeftHand.Grabbed)
        //{
            if (PlacingModeEnabled)
            {
                // Rotate the ghost avatar above the hand
                GhostAvatar.transform.Rotate(Vector3.up * Time.deltaTime * 30);

                if (!DestructionMode)
                {

                float axisInput = SteamVR_Input._default.inActions.ThumbStick.GetAxis(SteamVR_Input_Sources.RightHand).x;
                bool rotate = false;
                if (RotatingObject && Mathf.Abs(axisInput) < 0.9f)
                {
                    RotatingObject = false;
                }
                else if(!RotatingObject && Mathf.Abs(axisInput) > 0.9f)
                {
                    rotate = true;
                    RotatingObject = true;
                }

                    RotateGhostItem(axisInput, rotate);
                }

                Vector3 rayDir = RightHand.transform.forward;
                rayDir = Quaternion.AngleAxis(35, RightHand.transform.right) * rayDir;
                Ray ray = new Ray(RightHand.transform.position, rayDir);
                RaycastLogic(ray, SteamVR_Input._default.inActions.GrabPinch.GetStateUp(SteamVR_Input_Sources.RightHand), SteamVR_Input._default.inActions.GrabPinch.GetStateDown(SteamVR_Input_Sources.RightHand));


                if (SteamVR_Input._default.inActions.XButton.GetStateDown(SteamVR_Input_Sources.RightHand))
                {
                    DestructionMode = !DestructionMode;

                    GhostObject.transform.position = GhostObjectHiddenPosition;
                    //GhostDeleteMarker.transform.position = GhostObjectHiddenPosition;


                }

                // Check to exit placing mode
                if (SteamVR_Input._default.inActions.AButton.GetStateDown(SteamVR_Input_Sources.RightHand))
                {
                    EnablePlacingMode(false);
                }


                if (SteamVR_Input._default.inActions.BButton.GetStateDown(SteamVR_Input_Sources.RightHand))
                {
                    ChangePrefabs();

                }
            }
            else
            {
                // Check to enter placing mode
                if (SteamVR_Input._default.inActions.AButton.GetStateDown(SteamVR_Input_Sources.RightHand))
                {
                    PlacingModeEnabled = true;
                    EnablePlacingMode(true);
                }
            }
        //}
        //else if (PlacingModeEnabled)
        //{
        //    PlacingModeEnabled = false;
        //    EnablePlacingMode(false);
        //}
    }

}