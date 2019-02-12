using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerConsumer : ElectricalPole
{
    public float WattConsumtion = 0.2f;

    private bool _HasPower;

    public override void ObjectPlaced()
    {
        TickController.TickEvent += TickUpdate;
        base.ObjectPlaced();
    }

    private void TickUpdate(object sender, TickArgs args)
    {
        if (!Placed)
            return;
        
        if (Power.NodeGroups[GroupId].UsePower(WattConsumtion))
        {
            if (!_HasPower)
            {
                _HasPower = true;
                PowerOn();
            }
            PowerUsed();
        } else
        {
            if (_HasPower)
            {
                _HasPower = false;
                PowerOff();
            }
        }
    }

    protected virtual void PowerOn()
    {

    }

    protected virtual void PowerOff()
    {

    }
    protected virtual void PowerUsed()
    {

    }

    private void OnDestroy()
    {
        if(Placed)
            TickController.TickEvent -= TickUpdate;
    }
}
