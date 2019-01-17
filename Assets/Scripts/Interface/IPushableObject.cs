using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public interface IPushableObject
{
    bool ObjectIsFull(List<IPushableObject> CheckedObjects = null);
}

