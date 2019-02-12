using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Battery : ElectricalPole
{
    public float StorageCapacity = 50;

    public float StoredPower = 0;

    public MeshRenderer[] PowerBars;
    protected override void GroupChanged()
    {
        Power.NodeGroups[GroupId].AddBattery(this);
    }
    
    /// <summary>
    /// Returns left over watts when trying to add or subtract watts from the system. Does not apply the changes to the stored power
    /// </summary>
    /// <param name="watts"></param>
    /// <returns>Unused watts</returns>
    public float CheckCapacity(float watts)
    {
        return AddWatts(watts, false);
    }

    /// <summary>
    /// Returns left over watts when trying to add or subtract watts from the system.
    /// </summary>
    /// <param name="watts"></param>
    /// <param name="apply">Apply the changes to the stored power?</param>
    /// <returns>Unused watts</returns>
    public float AddWatts(float watts, bool apply = true)
    {
        float newCapacity = StoredPower + watts;
        if (newCapacity > StorageCapacity)
        {
            if(apply)
                StoredPower = StorageCapacity;
            return newCapacity - StorageCapacity;
        }

        if (newCapacity < 0)
        {
            if (apply)
                StoredPower = 0;
            return newCapacity;
        }

        if (apply)
            StoredPower = newCapacity;
        return 0;
    }

    private void FixedUpdate()
    {
        if (!Placed)
            return;
        float powerLevel = StoredPower / StorageCapacity;
        powerLevel *= 10;

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
