using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SlotObject : MonoBehaviour
{
    public Image ItemImage;
    public Text ItemAmount;
    public Image OutlineImage;

    public delegate void IntArgument(int i);
    public IntArgument SlotEvent;

    [HideInInspector]
    public int Index;
    public void SlotSelected()
    {
        if (SlotEvent != null)
            SlotEvent(Index);
    }
}
