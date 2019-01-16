using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TransformExtensions  {

    public static Transform GetTopParent(this Transform t)
    {
        Transform parent = t;
        while (parent.parent != null)
        {
            parent = parent.parent;
        }

        return parent;
    }
    
}
