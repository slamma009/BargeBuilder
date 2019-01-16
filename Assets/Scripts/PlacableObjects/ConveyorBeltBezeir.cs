using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorBeltBezeir : PlacableObject
{

    public GameObject[] BezeirPoints;

    Vector3 firstGridPosition;

    Vector3 firstPointDirection;

    MeshRenderer Renderer;
    MeshCollider Collider;

    private void Awake()
    {
        Renderer = GetComponent<MeshRenderer>();
        Collider = GetComponent<MeshCollider>();
        Collider.enabled = false;
    }
    

    public void SetFirstPosition(GridPositionObject gridPositionObject, Vector3 direction)
    {
        firstGridPosition = gridPositionObject.GridPosition;
        firstPointDirection = direction;
        BezeirPoints[0].transform.position = gridPositionObject.Position + direction;
        //BezeirPoints[1].transform.position = gridPositionObject.Position + direction * 3;
    }

    public Vector3 SetLastPosition(GridPositionObject gridPositionObject)
    {
        Vector3 dir = firstPointDirection;
        bool forward = firstPointDirection == Vector3.forward;
        bool backward = firstPointDirection == Vector3.back;
        bool right = firstPointDirection == Vector3.right;
        bool left = firstPointDirection == Vector3.left;
        if (forward || backward)
        {
            if (gridPositionObject.GridPosition.x > firstGridPosition.x)
                dir = Vector3.right;
            else if (gridPositionObject.GridPosition.x < firstGridPosition.x)
                dir = Vector3.left;
        }
        else if (right || left)
        {
            if (gridPositionObject.GridPosition.z > firstGridPosition.z)
                dir = Vector3.forward;
            else if (gridPositionObject.GridPosition.z < firstGridPosition.z)
                dir = Vector3.back;
        }
        BezeirPoints[3].transform.position = gridPositionObject.Position - dir;
        //BezeirPoints[2].transform.position = gridPositionObject.Position - dir * 3;


        return dir;
    }

    public bool UpdatePosition(GridPositionObject secondGridPosition)
    {
        bool forward = firstPointDirection == Vector3.forward;
        bool backward = firstPointDirection == Vector3.back;
        bool right = firstPointDirection == Vector3.right;
        bool left = firstPointDirection == Vector3.left;

        if ((forward && secondGridPosition.GridPosition.z <= firstGridPosition.z) ||
            (backward && secondGridPosition.GridPosition.z >= firstGridPosition.z) ||
            (right && secondGridPosition.GridPosition.x <= firstGridPosition.x) ||
            (left && secondGridPosition.GridPosition.x >= firstGridPosition.x) 
            )
        {
            Renderer.enabled = false;
            return false;
        }
        else if (!Renderer.enabled)
            Renderer.enabled = true;

        Vector3 dir = firstPointDirection;
        float firstDistance = 0;
        float secondDistance = 0;
        
        if(forward || backward) {
            if (secondGridPosition.GridPosition.x > firstGridPosition.x)
            {
                dir = Vector3.right;
                secondDistance = secondGridPosition.GridPosition.x - firstGridPosition.x;
            }
            else if (secondGridPosition.GridPosition.x < firstGridPosition.x)
            { 
                dir = Vector3.left;
                secondDistance = firstGridPosition.x - secondGridPosition.GridPosition.x;
            }

            firstDistance = (forward ? secondGridPosition.GridPosition.z - firstGridPosition.z : firstGridPosition.z - secondGridPosition.GridPosition.z ) * 1.5f;
        }
        else if (right || left)
        {
            if (secondGridPosition.GridPosition.z > firstGridPosition.z)
            {
                dir = Vector3.forward;
                secondDistance = secondGridPosition.GridPosition.z - firstGridPosition.z;
            }
            else if (secondGridPosition.GridPosition.z < firstGridPosition.z)
            {
                dir = Vector3.back;
                secondDistance = firstGridPosition.z - secondGridPosition.GridPosition.z;
            }
            firstDistance = (right ? secondGridPosition.GridPosition.x - firstGridPosition.x : firstGridPosition.x - secondGridPosition.GridPosition.x) * 1.5f;
        }
        SetLastPosition(secondGridPosition);
        

        //Vector3 position = FindIntersection(firstGridPosition * 2, secondGridPosition.Position, firstPointDirection, dir * -1);
        BezeirPoints[1].transform.position = firstGridPosition * 2 + firstPointDirection * firstDistance + Vector3.up;// position + new Vector3(0, firstGridPosition.y + 1, 0);
        BezeirPoints[2].transform.position = secondGridPosition.Position -  dir * secondDistance * 2; // position + new Vector3(0, secondGridPosition.Position.y, 0);

        return true;
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
