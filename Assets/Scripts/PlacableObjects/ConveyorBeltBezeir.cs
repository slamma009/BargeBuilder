using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ConveyorBeltBezeir : DraggableObject
{
    private BezierBlender BezierLogic;
    private void Start()
    {
        BezierLogic = GetComponent<BezierBlender>();
    }

    public override void Initialize(GameObject anchor, GameObject ghostObject)
    {
        base.Initialize(anchor, ghostObject);
        gameObject.AddComponent<Rigidbody>().isKinematic = true;

        Renderer.enabled = true;
        Collider.enabled = false;

        BezeirPoints[0].position = anchor.transform.position + Anchors[0].transform.forward * 0.01f;
        BezeirPoints[1].position = anchor.transform.position + anchor.transform.forward * -2;

        gameObject.SetActive(false);
    }

    public override void ObjectPlaced(GameObject anchor)
    {
        Anchors[1] = anchor;
        BezierLogic.SecondTriggerBox.nextObject = anchor.transform.GetTopParent().GetComponent<IPushableObject>();
        AnchorObject obj = anchor.transform.GetTopParent().GetComponent<PlacableObject>().Anchors.SingleOrDefault(x => x.Anchor.name == anchor.name);
        obj.ConnectAnchor = this.gameObject;
        AnchorObject firstAnchor = FirstAnchor.transform.GetTopParent().GetComponent<PlacableObject>().Anchors.SingleOrDefault(x => x.Anchor.name == FirstAnchor.name);
        firstAnchor.ConnectAnchor = this.gameObject;
        SnapToSecondAnchor();

        base.ObjectPlaced(anchor);

        Collider.enabled = true;
        BezierLogic.CreateCurve();

    }


    public void Update()
    {
        if (!Placed)
        {
            if (GhostObject != null)
            {
                if (Anchors[1] == null)
                {
                    BezeirPoints[3].position = GhostObject.transform.position + GhostObject.transform.forward * -1;
                    BezeirPoints[2].position = GhostObject.transform.position + GhostObject.transform.forward * -5;
                }
                else
                {
                    SnapToSecondAnchor();
                }
                BezierLogic.CreateCurve();
            }
        }


    }

    public void SnapToSecondAnchor()
    {
        BezeirPoints[3].position = Anchors[1].transform.position + Anchors[1].transform.forward * 0.01f;
        BezeirPoints[2].position = Anchors[1].transform.position + Anchors[1].transform.forward * -5;
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
