using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestTrigger : MonoBehaviour {

    public Inventory inv;
    private void OnTriggerEnter(Collider other)
    {
        Item item = other.gameObject.GetComponent<Item>();

        if(item != null)
        {
            if(inv.Add(item.ID) == 0)
            {
                Destroy(other.gameObject);
            }
        }
    }
}
