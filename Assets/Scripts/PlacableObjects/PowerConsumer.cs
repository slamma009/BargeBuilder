using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerConsumer : ElectricalPole
{
    public float TickLength = 1;
    public int WattConsumtion = 1;

    private float SavedTime; // Saved time for burning objects
    
    private void Update()
    {
        if (!Placed)
            return;

        if (Time.timeSinceLevelLoad - SavedTime >= TickLength)
        {
            if (Power.NodeGroups[GroupId].UsePower(WattConsumtion))
            {
                SavedTime = Time.timeSinceLevelLoad;
                Execute();
            }
        }
    }

    protected virtual void Execute()
    {

    }
}
