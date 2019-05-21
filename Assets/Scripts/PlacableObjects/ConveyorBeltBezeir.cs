using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ConveyorBeltBezeir : DraggableObject, IPushableObject
{
    public List<ConveyorItemInfo> ItemsOnBelt = new List<ConveyorItemInfo>();
    private BezierBlender BezierLogic;
    public IPushableObject AttachechedObject;

    private float ItemMoveDistance = 0;
    private int ItemSegments = 0;
    private float SpeedModifier = 1;

    private bool IsValid = true;
    private void Start()
    {
        BezierLogic = GetComponent<BezierBlender>();
    }
    private Vector3[] ConveyorBeltCheckpoints;
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
        AttachechedObject = anchor.GetComponentInParent<IPushableObject>();
        //Debug.Log(AttachechedObject);
        AnchorObject obj = anchor.transform.GetTopParent().GetComponent<PlacableObject>().Anchors.SingleOrDefault(x => x.Anchor.name == anchor.name);
        obj.ConnectAnchor = this.gameObject;
        AnchorObject firstAnchor = FirstAnchor.transform.GetTopParent().GetComponent<PlacableObject>().Anchors.SingleOrDefault(x => x.Anchor.name == FirstAnchor.name);
        firstAnchor.ConnectAnchor = this.gameObject;
        SnapToSecondAnchor();

        base.ObjectPlaced(anchor);

        Collider.enabled = true;
        BezierLogic.CreateCurve();
        
        ItemSegments = Mathf.RoundToInt(BezierLogic.CurveLength);
        ConveyorBeltCheckpoints = new Vector3[ItemSegments];
        SpeedModifier = ItemMoveDistance = BezierLogic.CurveLength / ItemSegments;

        float itemMoved = 0;
        int j = 0;
        for(var i=0; i< BezierLogic.Path.Count; ++i)
        {
            if (i < BezierLogic.LookupTable.Length)
            {
                if (BezierLogic.LookupTable[i] > itemMoved + ItemMoveDistance)
                {
                    itemMoved += ItemMoveDistance;
                    float extraDistance = itemMoved - BezierLogic.LookupTable[i - 1];
                    float percentage = extraDistance / (BezierLogic.LookupTable[i] - BezierLogic.LookupTable[i - 1]);
                    ConveyorBeltCheckpoints[j] = Vector3.Lerp(BezierLogic.Path[i], BezierLogic.Path[i + 1], percentage);
                    j++;
                }
            }
            else
            {
                Debug.Log(j +  " : " + ItemSegments);
                if (j != ItemSegments)
                ConveyorBeltCheckpoints[j] = BezierLogic.Path[i];
                break;
            }
        }
        

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
                IsValid = !BezierLogic.CreateCurve();
            }
        }
    }
    private void FixedUpdate()
    {
        for (var i = ItemsOnBelt.Count - 1; i >= 0; --i)
        {
            
            ItemsOnBelt[i].Item.transform.position = Vector3.MoveTowards(
            ItemsOnBelt[i].Item.transform.position,
            ItemsOnBelt[i].Target,
            Time.deltaTime * SpeedModifier);

            if (Vector3.Distance(ItemsOnBelt[i].Item.transform.position, ItemsOnBelt[i].Target) < 0.001f)
            {
                ItemsOnBelt[i].Item.transform.position = ItemsOnBelt[i].Target; float ourDistance = BezierLogic.LookupTable[ItemsOnBelt[i].State]; //BezierLogic.CalcBezeirDistance(ItemsOnBelt[i].State, ItemsOnBelt[i].Item.transform.position);
               
                if (ItemsOnBelt[i].State < ConveyorBeltCheckpoints.Length - 1)
                {
                    if (ItemsOnBelt.Where(x => x.State == ItemsOnBelt[i].State + 1).Count() == 0)
                    {
                        //Debug.Log(Time.time + ": " + ItemsOnBelt[i].State);
                        ItemsOnBelt[i].State += 1;
                        ItemsOnBelt[i].Target =
                            new Vector3(ConveyorBeltCheckpoints[ItemsOnBelt[i].State].x,
                            ItemsOnBelt[i].Item.transform.position.y,
                            ConveyorBeltCheckpoints[ItemsOnBelt[i].State].z);
                    }
                }
                else
                {
                    //Debug.Log(AttachechedObject);
                    if (AttachechedObject != null && !AttachechedObject.ObjectIsFull())
                    {
                        AttachechedObject.PushObject(ItemsOnBelt[i].Item);
                        ItemsOnBelt.RemoveAt(i);
                    }
                }
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

    public override bool ObjectIsValid()
    {
        return IsValid;
    }


    public bool ObjectIsFull(List<IPushableObject> CheckedObjects = null)
    {
        return !(ItemsOnBelt.Count == 0 ||
            (ItemsOnBelt[ItemsOnBelt.Count - 1].State != 0));
    }

    public void PushObject(GameObject item)
    {
        item.transform.parent = this.transform;
        ItemsOnBelt.Add(new ConveyorItemInfo(item, 
            new Vector3(ConveyorBeltCheckpoints[0].x, 
            item.transform.position.y,
            ConveyorBeltCheckpoints[0].z)));
    }
}
