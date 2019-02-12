using System;
using UnityEngine;

public class SimpleLight : PowerConsumer
{
    public Light Lamp = new Light();
    protected override void PowerOn()
    {
        Lamp.enabled = true;
        base.PowerOn();
    }

    protected override void PowerOff()
    {
        Lamp.enabled = false;
        base.PowerOn();
    }
}

