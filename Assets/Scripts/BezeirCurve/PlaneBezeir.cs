using System.Collections.Generic;
using UnityEngine;

public class PlaneBezeir: BezierCurve3D
{
    public ExtrudeShape shape;
    public float Segments = 5;
    public Transform[] Points;
    public GameObject PointRepresentation;

    public bool ExtrudeMesh = false;

    private List<GameObject> SpawnedPoints = new List<GameObject>();

    private MeshFilter meshFilter;
    private Mesh mesh;
    private MeshRenderer meshRenderer;
    public void Start()
    {
        shape = new ExtrudeShape();
        shape.verts = new ExtrudeShapeVert[]
        {
            new ExtrudeShapeVert()
            {
                normal = new Vector2(0, 1),
                point = new Vector2(0,0),
                u = 0
            },
            new ExtrudeShapeVert()
            {
                normal = new Vector2(0,1),
                point = new Vector2(1,0),
                u = 1
            }
        };
        shape.lines = new int[] { 0, 1 };
        
        meshFilter = GetComponent<MeshFilter>();
        mesh = meshFilter.mesh;
        meshRenderer = GetComponent<MeshRenderer>();
    }

    public void FixedUpdate()
    {
        if (!ExtrudeMesh)
        {
            if (meshRenderer.enabled)
                meshRenderer.enabled = false;

            if (SpawnedPoints.Count < Segments)
            {
                for (int i = SpawnedPoints.Count; i < Segments; ++i)
                {
                    SpawnedPoints.Add(Instantiate(PointRepresentation, Vector3.zero, Quaternion.identity));
                }
            }
            else if (SpawnedPoints.Count >= Segments)
            {
                for (var i = SpawnedPoints.Count - 1; i >= Segments; --i)
                {
                    Destroy(SpawnedPoints[i]);
                    SpawnedPoints.RemoveAt(i);
                }
            }
        } else
        {
            if (!meshRenderer.enabled)
                meshRenderer.enabled = true;

            if (SpawnedPoints.Count > 0)
            {
                foreach (GameObject point in SpawnedPoints)
                {
                    Destroy(point);
                }
                SpawnedPoints.Clear();
            };
        }

        // Generate Curve

        Vector3[] pointPositions = new Vector3[4];
        for (int i= 0; i< Points.Length; ++i)
        {
            pointPositions[i] = Points[i].position;
        }
        OrientedPoint[] path = new OrientedPoint[(int)Segments];
        for(int i =0; i<Segments; ++i)
        {
            if (!ExtrudeMesh)
            {
                SpawnedPoints[i].transform.position = GetPoint(pointPositions, i / (Segments - 1));
            }
            else
            {
                path[i] = new OrientedPoint()
                {
                    Position = GetPoint(pointPositions, i / (Segments - 1)),
                    Rotation = GetPointOrientation3d(pointPositions, i / (Segments - 1), Vector3.up)
                };
            }
        }
        if (ExtrudeMesh)
        meshFilter.mesh = Extrude(mesh, shape, path);


    }


}

