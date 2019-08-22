using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorAddItem : MonoBehaviour
{
    public int ItemId;
    int Amount = 1;

    public Inventory Target;

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            Target.Add(ItemId, 1);
        }
    }
}
