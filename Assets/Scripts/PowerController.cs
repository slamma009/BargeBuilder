using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerController : MonoBehaviour
{
    public List<PowerNodeGroup> NodeGroups = new List<PowerNodeGroup>();
    private List<Color> AllColors = new List<Color>(); // DEBUG, DELETE LATER

    private void Start()
    {
        AllColors.Add(Color.red);
        AllColors.Add(Color.blue);
        AllColors.Add(Color.green);
        AllColors.Add(Color.yellow);
        AllColors.Add(Color.cyan);
        AllColors.Add(Color.magenta);

    }

    // DELTE LATER
    private void SetWireColors(ElectricalPole pole, int i)
    {
        foreach (ElectricalPoleWireHolder holder in pole.ConnectedPoles)
        {
            holder.Wire.GetComponentInChildren<MeshRenderer>().material.color = AllColors[i];
        }
    }

    public void CheckNodeGroups()
    {
        // Clear all node groups to create new ones
        NodeGroups.Clear();
        // A list of all the nodes in the game
        ElectricalPole[] allNodes = GameObject.FindObjectsOfType<ElectricalPole>();
        // A lit of all nodes that have been checked in our outter foreachloop
        List<ElectricalPole> allNodesChecked = new List<ElectricalPole>();


        foreach (ElectricalPole node in allNodes)
        {
            if (node.Placed && !allNodesChecked.Contains(node))
            {
                // Create a list to be passed into the FindNodeGroup via reference
                List<ElectricalPole> checkedNodes = new List<ElectricalPole>();

                // Find all the nodes in the group
                FindNodeGroup(node, checkedNodes);

                // Add a new node group to our node groups list
                NodeGroups.Add(new PowerNodeGroup(checkedNodes.ToArray()));

                // Add the nodes from the new node group to our nodesChecked list
                allNodesChecked.AddRange(checkedNodes);
                

            }
        }

        // Temporary code to show the color of each node group
        for (var i = 0; i < NodeGroups.Count; ++i)
        {
            foreach(ElectricalPole pole in NodeGroups[i].Poles)
            {
                pole.GroupId = i;
                SetWireColors(pole, i);
            }
        }
        
    }

    /// <summary>
    /// Finds all nodes in the node tree and puts them in checkedNodes recursively
    /// </summary>
    /// <param name="node">Starting node</param>
    /// <param name="checkedNodes">Empty list passed by reference. Filled with all nodes found</param>
    void FindNodeGroup(ElectricalPole node, List<ElectricalPole> checkedNodes)
    {
        if (node.Placed && !checkedNodes.Contains(node))
        {
            checkedNodes.Add(node);
            foreach (ElectricalPoleWireHolder newNode in node.ConnectedPoles)
            {
                FindNodeGroup(newNode.Pole, checkedNodes);
            }
        } 
    }
}

[System.Serializable]
public class PowerNodeGroup
{
    public PowerNodeGroup(ElectricalPole[] poles)
    {
        Poles = poles;
    }

    public readonly ElectricalPole[] Poles;

    public float CurrentPowerLevels;
    public float MaxPowerLevels;

    [SerializeField]
    private List<Battery> _Batteries = new List<Battery>();

    public void AddBattery(Battery battery)
    {
        _Batteries.Add(battery);
    }



    public bool AddPowerToGrid(float watts)
    {
        float newPowerLevels = CurrentPowerLevels + watts;
        if (newPowerLevels > MaxPowerLevels)
        {
            return UtalizeBatteries(newPowerLevels - MaxPowerLevels);
        }
        CurrentPowerLevels = newPowerLevels;
        return true;
    }

    public bool UsePower(float watts)
    {
        float newPowerLevels = CurrentPowerLevels - watts;
        if (newPowerLevels < 0)
        {
            return UtalizeBatteries(newPowerLevels);
        }

        CurrentPowerLevels = newPowerLevels;
        return true;
    }


    private bool UtalizeBatteries(float watts)
    {
        float wattsToAdd = watts;
        int i;
        for (i = 0; i < _Batteries.Count; ++i)
        {
            wattsToAdd = _Batteries[i].CheckCapacity(wattsToAdd);
            if (wattsToAdd == 0)
                break;
        }

        if (i != _Batteries.Count)
        {
            wattsToAdd = watts;
            for (var j = 0; j <= i; ++j)
            {
                wattsToAdd = _Batteries[i].AddWatts(wattsToAdd);
            }
            return true;
        }

        return false;
    }

}
