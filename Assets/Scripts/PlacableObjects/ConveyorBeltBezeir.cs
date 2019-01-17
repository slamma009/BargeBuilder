using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorBeltBezeir : DraggableObject
{

    MeshRenderer Renderer;
    MeshCollider Collider;

    private void Awake()
    {
        Renderer = GetComponent<MeshRenderer>();
        Collider = GetComponent<MeshCollider>();
        Collider.enabled = false;
    }

    public override void ObjectPlaced(GameObject anchor)
    {
        Collider.enabled = true;
        base.ObjectPlaced(anchor);
    }



    public Vector3 FindIntersection(Vector3 pointA, Vector3 pointB, Vector3 dirA, Vector3 dirB)
    {
        Vector3 pointA2 = pointA + dirA * 100;
        Vector3 pointB2 = pointB + dirB * 100;


        float A1 = pointA2.z - pointA.z;
        float B1 = pointA.x - pointA2.x;
        float C1 = A1 * pointA.x + B1 * pointA.z;

        float A2 = pointB2.z - pointB.z;
        float B2 = pointB.x - pointB2.x;
        float C2 = A2 * pointB.x + B2 * pointB.z;


        float det = A1 * B2 - A2 * B1;

        float x = (B2 * C1 - B1 * C2) / det;
        float z = (A1 * C2 - A2 * C1) / det;

        return new Vector3(x, this.transform.position.y , z);
    }
}
