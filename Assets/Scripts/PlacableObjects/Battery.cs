using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Battery : ElectricalPole
{
    public int StorageCapacity = 50;

    public MeshRenderer[] PowerBars;
    protected override void GroupChanged()
    {
        Power.NodeGroups[GroupId].MaxPowerLevels += StorageCapacity;
    }

    private void FixedUpdate()
    {
        if (!Placed)
            return;
        float powerLevel = (float)Power.NodeGroups[GroupId].CurrentPowerLevels / (float)Power.NodeGroups[GroupId].MaxPowerLevels;
        powerLevel *= 10;
        Debug.Log(powerLevel);

        for(var i=0; i<PowerBars.Length; ++i)
        {
            if(i <= powerLevel)
            {
                PowerBars[i].material.color = Color.green;
            } else
            {
                PowerBars[i].material.color = Color.white;
            }
        }
    }
}
