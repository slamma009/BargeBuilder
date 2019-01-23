using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Joystick : GrabbableObject {

    //OVRPlayerController PlayerController;
    public HoverEngineStatic HoverBarge;

    [SerializeField]
    JoystickType Type;

    public override void ItemGrabbed(GameObject grabber, bool isRighthand)
    {
        base.ItemGrabbed(grabber, isRighthand);
        //while (grabber.tag != "Player")
        //{
        //    grabber = grabber.transform.parent.gameObject;
        //}

        //PlayerController = grabber.GetComponent<OVRPlayerController>();
        //PlayerController.EnableLinearMovement = false;
        //PlayerController.EnableRotation = false;

        if (Type == JoystickType.Engine)
        {
            HoverBarge.EngineIsRightHand = isRighthand;
            HoverBarge.EnableEngine = true;
        }
        else
        {
            HoverBarge.HeightIsRightHand = isRighthand;
            HoverBarge.HeightEnabled = true;
        }

    }
    public override void ItemDropped()
    {
        base.ItemDropped();
        //PlayerController.EnableLinearMovement = true;
        //PlayerController.EnableRotation = true;
        if (Type == JoystickType.Engine)
        {
            HoverBarge.EnableEngine = false;
        }
        else
        {
            HoverBarge.HeightEnabled = false;
        }
    }
}


public enum JoystickType
{
    Engine,
    Height
}
