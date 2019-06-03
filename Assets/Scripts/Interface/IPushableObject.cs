using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public interface IPushableObject
{
    bool CanTakeItem(Item item);

    void PushItem(Item item);
}

