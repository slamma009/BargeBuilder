using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerController : MonoBehaviour
{
    public List<ElectricalPole[]> NodeGroups = new List<ElectricalPole[]>();
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

    public void CheckNodeGroups()
    {

        NodeGroups.Clear();
        ElectricalPole[] allNodes = GameObject.FindObjectsOfType<ElectricalPole>();

        List<ElectricalPole> allNodesChecked = new List<ElectricalPole>();
        foreach (ElectricalPole node in allNodes)
        {
            if (node.Placed && !allNodesChecked.Contains(node))
            {
                List<ElectricalPole> checkedNodes = new List<ElectricalPole>();

                FindNodeGroup(node, checkedNodes);
                NodeGroups.Add(checkedNodes.ToArray());
                allNodesChecked.AddRange(checkedNodes);
                

            }
        }

        for (var i = 0; i < NodeGroups.Count; ++i)
        {
            foreach(ElectricalPole pole in NodeGroups[i])
            {
                foreach(ElectricalPoleWireHolder holder in pole.ConnectedPoles)
                {
                    holder.Wire.GetComponentInChildren<MeshRenderer>().material.color = AllColors[i];
                }
            }
        }
        
    }

    void FindNodeGroup(ElectricalPole node, List<ElectricalPole> checkedNodes)
    {
        if (node.Placed && !checkedNodes.Contains(node))
        {
            checkedNodes.Add(node);
            foreach (ElectricalPoleWireHolder newNode in node.ConnectedPoles)
            {
                FindNodeGroup(newNode.Pole, checkedNodes);
            }
        } else if (!node.Placed)
        {
            Debug.Log(node.name);
        }

    }
}
