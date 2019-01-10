using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class ExtrudeShape
{
    public ExtrudeShapeVert[] verts;
    public int[] lines;

    //public Vector2[] verts;
    //public Vector2[] normals;
    //public float[] us;
    //public int[] lines = new int[]
    //{
    //    0,1,
    //    2,3,
    //    3,4,
    //    4,5
    //};
}

public class ExtrudeShapeVert
{
    public Vector2 normal;
    public Vector2 point;
    public float u;
}

