using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BezierCurve3D : MonoBehaviour
{
    protected Vector3 GetPoint ( Vector3[] pts, float t)
    {
        // Calculation of bezier curve point. 
        // a = (1-t)p0 + tp1
        // b = (1-t)p1 + tp2
        // c = (1-t)p2 + tp3
        // d = (1-t)a + tb
        // e = (1-t)b + tc
        // pt = (1-t)d + te
        float omt = 1f - t;
        float omt2 = omt * omt;
        float t2 = t * t;
        return 
            pts[0] * (omt2 * omt) +
            pts[1] * (3f * omt2 * t) +
            pts[2] * (3f * omt * t2) +
            pts[3] * (t2 * t);
    }

    Vector3 GetPointTangent(Vector3[] pts, float t)
    {
        //Calculate the tangent for a pt on the curve
        // e-d
        // ((1-t)b + tc) - ((1-t)a + tb)
        float omt = 1f - t;
        float omt2 = omt * omt;
        float t2 = t * t;
        Vector3 tangent =
            pts[0] * (-omt2) +
            pts[1] * (3 * omt2 - 2 * omt) +
            pts[2] * (-3 * t2 + 2 * t) +
            pts[3] * (t2);

        return tangent.normalized;
    }

    Vector3 GetPointsNormal ( Vector3[] pts, float t, Vector3 up)
    {
        Vector3 tng = GetPointTangent(pts, t); // z axis
        Vector3 binormal = Vector3.Cross(up, tng).normalized; // x axis
        return Vector3.Cross(tng, binormal); // y axis
    }

    protected Quaternion GetPointOrientation3d(Vector3[] pts, float t, Vector3 up)
    {
        Vector3 tng = GetPointTangent(pts, t);
        Vector3 nrm = GetPointsNormal(pts, t, up);
        return Quaternion.LookRotation(tng, nrm);
    }

    public Mesh Extrude(Mesh mesh, ExtrudeShape shape, OrientedPoint[] path)
    {
        int vertsInShape = shape.verts.Length;
        int segments = path.Length - 1;
        int edgeLoops = path.Length;
        int vertCount = vertsInShape * edgeLoops;
        int triCount = shape.lines.Length * segments;
        int triIndexCount = triCount * 3;

        int[] triangleIndices = new int[ triIndexCount ];
        Vector3[] vertices  = new Vector3[ vertCount ];
        Vector3[] normals    = new Vector3[ vertCount ];
        Vector2[] uvs        = new Vector2[ vertCount ];

        /* Generation Code */

        // Loop through each point on the path
        for(int i=0; i<path.Length; ++i)
        {
            // Offset the index of the verticies by the verticies in the shape
            int offset = i * vertsInShape;

            // Loop through each vertix in the shape. 
            for(int j=0; j<vertsInShape; ++j)
            {
                int id = offset + j;
                vertices[id] = path[i].LocalToWorld(shape.verts[j].point); // might break to verts being 2d
                normals[id] = path[i].LocalToWorldDirection(shape.verts[j].normal);
                uvs[id] = new Vector2(shape.verts[j].u, i / ((float)edgeLoops));

            }
        }

        int ti = 0;
        // Loop through each segment to make triangles
        for(int i=0; i<segments; ++i)
        {
            int offset = i * vertsInShape;
            for (int l = 0; l < shape.lines.Length; l += 2)
            {
                // Add 2 triangles per line.
                int a = offset + shape.lines[l] + vertsInShape;
                int b = offset + shape.lines[l];
                int c = offset + shape.lines[l + 1];
                int d = offset + shape.lines[l + 1] + vertsInShape;
                triangleIndices[ti] = a; ti++;
                triangleIndices[ti] = b; ti++;
                triangleIndices[ti] = c; ti++;
                triangleIndices[ti] = c; ti++;
                triangleIndices[ti] = d; ti++;
                triangleIndices[ti] = a; ti++;
            }
        }

        /* --------------- */
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangleIndices;
        mesh.normals = normals;
        mesh.uv = uvs;

        return mesh;
    }



    
    
}
