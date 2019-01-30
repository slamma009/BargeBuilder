using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class LaserPointer : GrabbableObject
{
    public Transform RayOrigin;
    public LineRenderer Line;
    public InventoryCanvas SetCanvas;
    public override void ItemGrabbed(GameObject grabber, bool isRighthand)
    {
        transform.position = grabber.transform.position;// Quaternion.LookRotation(-grabber.transform.right, grabber.transform.up); ;
        //Quaternion.AngleAxis(90, Vector3.up) * forward;
        base.ItemGrabbed(grabber, isRighthand);
        transform.localEulerAngles = new Vector3(-52.513f, 20.037f, -113.025f);
        Line.enabled = true;

    }

    public override void ItemDropped()
    {
        Line.enabled = false;
        base.ItemDropped();
    }
    public void FixedUpdate()
    {
        if (IsGrabbed)
        {
            Line.SetPosition(0, RayOrigin.position);
            Ray ray = new Ray(RayOrigin.position, RayOrigin.forward);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 1000))
            {
                Line.SetPosition(1, hit.point);
                if (SteamVR_Input._default.inActions.GrabPinch.GetStateUp(SteamVR_Input_Sources.RightHand))
                {
                    Inventory inv = hit.transform.GetTopParent().GetComponent<Inventory>();
                    SetCanvas.AttachedInventory = inv;
                }
            }
            else
            {
                Line.SetPosition(1, RayOrigin.position + ray.direction * 1000);
            }
        }
    }
}
