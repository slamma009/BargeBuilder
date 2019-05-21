﻿using System.Collections.Generic;
using UnityEngine;

public class BezierBlender: BezierCurve3D
{
    public Mesh BezeirMesh;
    public ExtrudeShape shape;
    public float Segments = 5;
    public Transform[] Points;
    public GameObject PointRepresentation;
    public Material ObjectMaterial;

    private MeshFilter meshFilter;
    private Mesh mesh;
    private MeshRenderer meshRenderer;

    private ConveyorBeltBezeir BezierController;

    private int MeshLeftIndex;
    private int MeshRightIndex;

    public List<Vector3> Path { get; private set; }

    public void Awake()
    {
        shape = GenerateShape(BezeirMesh);
        
        meshFilter = GetComponent<MeshFilter>();
        mesh = meshFilter.mesh;
        meshRenderer = GetComponent<MeshRenderer>();

        BezierController = GetComponent<ConveyorBeltBezeir>();

        Path = new List<Vector3>();
    }

    public int[] GetLinesFromVerts(List<ExtrudeShapeVert> verts)
    {
        List<int> returnList = new List<int>();

        // Unity generates extra vertices for sharp edges on import. These verts are added to the end of the vert array.
        // So we'll create a dictionary to keep track of which points are hard points.
        Dictionary<int, int> hardVertPairs = new Dictionary<int, int>();

        // Save the soft End for later, this is where the loop actually ends.
        int softVertEnd = 0;

        // Loop through each vert backwards, we want to find all the hard points which are on the end of the array.
        for(var i=verts.Count - 1; i>=0; --i)
        {
            bool found = false;
            // Look through every vert from the beginning. If we hit our poitn and don't find a match then we aren't a hard point.
            for(var j=0; j<i; ++j)
            {
                // If the verts match on points, then they are the same edge, just different normals/UV
                if(verts[j].point == verts[i].point)
                {
                    // Save them to our hard points for later, and breakout
                    hardVertPairs.Add(j, i);
                    found = true;
                    break;
                }
            }

            // If there was no match then we are back to our normal loop.
            if(!found)
            {
                softVertEnd = i;
                break;
            }
        }

        bool lastUsedHard = false; // Used for tracking which point to use.

        // Loop through the vertex loop to create the lines
        for (var i = 0; i <= softVertEnd; ++i)
        {
            int x = i; // Line start point
            int y = i + 1; // Line end point.
            if(i == 0)
            {
                // if point 0 and point 1 are hard points there are 4 possible combinations
                // A   B
                // C   D
                // AB,AD     CB,CD
                //Determine which is the proper line to start with by matching normals
                bool zeroIsHard = hardVertPairs.ContainsKey(0);
                bool oneIsHard = hardVertPairs.ContainsKey(1);
                if (oneIsHard)
                {
                    if (verts[0].normal == verts[hardVertPairs[1]].normal)
                    {
                        y = hardVertPairs[1];
                        lastUsedHard = true;
                    }
                }
                if (zeroIsHard)
                {
                    if (verts[1].normal == verts[hardVertPairs[0]].normal)
                    {
                        x = hardVertPairs[0];
                        lastUsedHard = false;
                    }
                }
                if(oneIsHard && zeroIsHard)
                {
                    if(verts[hardVertPairs[0]].normal == verts[hardVertPairs[1]].normal)
                    {
                        x = hardVertPairs[1];
                        lastUsedHard = true;
                    }
                }
            }
            else if (i != softVertEnd)
            {
                // If the end point of the last line wasn't a hardpoint then we need to use our hard point if it exists.
                if (!lastUsedHard && hardVertPairs.ContainsKey(i))
                {
                    x = hardVertPairs[i];
                }

                // If the normals from our line start to our line end don't match then we can assume y is a hardpoint,  
                // and we need to use that hardpoint.
                if (verts[x].normal != verts[y].normal)
                {
                    y = hardVertPairs[y];
                    lastUsedHard = true;
                } else
                {
                    lastUsedHard = false;
                }
            }
            else
            {
                // Ending the loop. Make sure we use the proper starting point.
                if (!lastUsedHard && hardVertPairs.ContainsKey(i))
                    x = hardVertPairs[i];

                y = 0;
                // If 0 is a hardpoint, and the vertex at 0 doesn't match this normal, then we must use 0's hardpoint.
                if (hardVertPairs.ContainsKey(0) && verts[0].normal != verts[x].normal)
                {
                    y = hardVertPairs[0];
                }
            }

            // Add the line to the array
            returnList.Add(x);
            returnList.Add(y);
        }
        return returnList.ToArray();
    }

    public ExtrudeShape GenerateShape(Mesh mesh)
    {
        ExtrudeShape newShape = new ExtrudeShape();
        List<ExtrudeShapeVert> verts = new List<ExtrudeShapeVert>();
        MeshLeftIndex = -1;
        float minX = 100;
        MeshRightIndex = -1;
        float maxX = 0;
        for(var i=0; i<mesh.vertexCount; ++i)
        {
            // Blender flips the z and y axis's. checking if the vertex y == 0 removes all depth from the object.
            if (mesh.vertices[i].y == 0)
            {
                if(mesh.vertices[i].x < minX)
                {
                    minX = mesh.vertices[i].x;
                    MeshLeftIndex = i;
                }
                if (mesh.vertices[i].x > maxX)
                {
                    maxX = mesh.vertices[i].x;
                    MeshRightIndex = i;
                }
                verts.Add(new ExtrudeShapeVert()
                {
                    normal = new Vector2(mesh.normals[i].x, mesh.normals[i].z),
                    point = new Vector2(mesh.vertices[i].x, mesh.vertices[i].z),
                    u = mesh.uv[i].x
                });

            }
        }

        newShape.verts = verts.ToArray();
        newShape.lines = GetLinesFromVerts(verts);
        return newShape;

    }

    public bool CreateCurve()
    {
        // Generate Curve

        Vector3[] pointPositions = new Vector3[4];
        for (int i = 0; i < Points.Length; ++i)
        {
            pointPositions[i] = Points[i].position;
        }
        OrientedPoint[] path = new OrientedPoint[(int)Segments];
        Path.Clear();
        for (int i = 0; i < Segments; ++i)
        {
            float t = i / (Segments - 1);
            Vector3 curvePoint = GetPoint(pointPositions, t);

            Quaternion rotation = GetPointOrientation3d(pointPositions, t, Vector3.up);

            path[i] = new OrientedPoint()
            {
                Position = curvePoint,
                Rotation = rotation
            };
            Path.Add(curvePoint);

        }
        bool intersecting = false;
        meshFilter.mesh = Extrude(mesh, shape, path, pointPositions, out intersecting, MeshLeftIndex, MeshRightIndex);

        GetComponent<MeshRenderer>().sharedMaterial = null;
        GetComponent<MeshRenderer>().sharedMaterial = ObjectMaterial;
        GetComponent<MeshRenderer>().UpdateGIMaterials();


        MeshCollider collider = GetComponent<MeshCollider>();
        if (collider.enabled)
        {
            collider.sharedMesh = null;
            collider.sharedMesh = mesh;
        }

        return intersecting;
    }


}

