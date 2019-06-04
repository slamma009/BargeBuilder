using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestTrigger : MonoBehaviour {

    public Inventory inv;
    private void OnTriggerStay(Collider other)
    {
        ItemInstance item = other.gameObject.GetComponent<ItemInstance>();

        if(item != null)
        {
            if(inv.Add(item.ID) == 0)
            {
                Destroy(other.gameObject);
            }
        }
    }
}
