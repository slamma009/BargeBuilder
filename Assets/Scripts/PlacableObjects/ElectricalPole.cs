using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ElectricalPole : PlacableObject
{
    public float MaxWireDistance;
    public GameObject WirePrefab;

    public Transform WireAnchorPoint;
    
    public List<ElectricalPoleWireHolder> ConnectedPoles = new List<ElectricalPoleWireHolder>();

    public int GroupId
    {
        get
        {
            return _GroupId;
        }
        set
        {
            _GroupId = value;
            GroupChanged();
        }
    }

    private int _GroupId;

    protected PowerController Power;


    protected virtual void GroupChanged()
    {

    }

    private void Update()
    {

        if (IsGhost)
        {
            ConnectedPoles.ForEach(x => x.CheckedThisFrame = false);

            GetPolesInRadius();

            for (var i = ConnectedPoles.Count - 1; i >= 0; --i)
            {
                if (!ConnectedPoles[i].CheckedThisFrame)
                {
                    Destroy(ConnectedPoles[i].Wire);
                    ConnectedPoles.RemoveAt(i);

                }
            }
        }
    }

    /// <summary>
    /// Finds all poles within a given radius and attaches a wire to them
    /// </summary>
    /// <param name="setOtherPole">If true will assign itself to the poles it found</param>
    private void GetPolesInRadius(bool setOtherPole = false)
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, MaxWireDistance, LayerMask.GetMask("PlacementCollider"));

        foreach (Collider c in hitColliders)
        {
            Transform TopParent = c.transform.GetTopParent();
            if (TopParent != transform)
            {
                ElectricalPole electricalPole = TopParent.GetComponent<ElectricalPole>();
                if (electricalPole != null && !electricalPole.IsGhost && electricalPole.Placed)
                {
                    ElectricalPoleWireHolder pole = ConnectedPoles.Where(x => x.Pole.transform == TopParent).SingleOrDefault();
                    if (pole == null)
                    {
                        GameObject wire = Instantiate(WirePrefab);
                        ConnectedPoles.Add(new ElectricalPoleWireHolder(electricalPole, wire));
                        ConnectedPoles[ConnectedPoles.Count - 1].SetWirePosition(electricalPole.WireAnchorPoint.position);
                        if(setOtherPole)
                            electricalPole.ConnectedPoles.Add(new ElectricalPoleWireHolder(this, wire));
                    }
                    else
                    {
                        pole.CheckedThisFrame = true;
                        pole.SetWirePosition(WireAnchorPoint.position);
                    }
                }
            }
        }
    }

    public override void ObjectPlaced()
    {
        Power = GameObject.FindObjectOfType<PowerController>();
        GetPolesInRadius(true);
        base.ObjectPlaced();
        Power.CheckNodeGroups();
    }

    private void OnDestroy()
    {
        foreach (ElectricalPoleWireHolder holder in ConnectedPoles)
        {
            for (var i = holder.Pole.ConnectedPoles.Count - 1; i >= 0; --i)
            {
                if (holder.Pole.ConnectedPoles[i].Pole == this)
                {
                    holder.Pole.ConnectedPoles.RemoveAt(i);
                    break;
                }
            }

            Destroy(holder.Wire);
        }
        if(Placed && Power != null)
            Power.CheckNodeGroups();
    }
}

[System.Serializable]
public class ElectricalPoleWireHolder
{
    public ElectricalPole Pole;
    public bool CheckedThisFrame = false;
    public GameObject Wire;

    public ElectricalPoleWireHolder(ElectricalPole pole, GameObject wire)
    {
        CheckedThisFrame = true;
        Pole = pole;
        Wire = wire;
    }

    public void SetWirePosition(Vector3 otherPoint)
    {
        Wire.transform.position = (otherPoint + Pole.WireAnchorPoint.position) * 0.5f;
        Wire.transform.LookAt(Wire.transform.position + (otherPoint - Pole.WireAnchorPoint.position).normalized);
        Wire.transform.localScale = new Vector3(1, 1, Vector3.Distance(otherPoint, Pole.WireAnchorPoint.position) * 0.5f);
    }
}