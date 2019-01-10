
using UnityEngine;

public struct OrientedPoint
{
    public Vector3 Position;
    public Quaternion Rotation;

    public OrientedPoint(Vector3 position, Quaternion rotation)
    {
        this.Position = position;
        this.Rotation = rotation;
    }

    public Vector3 LocalToWorld(Vector3 point)
    {
        return Position + Rotation * point;
    }

    public Vector3 WorldToLocal( Vector3 point )
    {
        return Quaternion.Inverse(Rotation) * (point - Position);
    }

    public Vector3 LocalToWorldDirection ( Vector3 dir )
    {
        return Rotation * dir;
    }

}