using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public interface IPushableObject
{
    bool ObjectIsFull(List<IPushableObject> CheckedObjects = null);

    void PushObject(GameObject item);
}

