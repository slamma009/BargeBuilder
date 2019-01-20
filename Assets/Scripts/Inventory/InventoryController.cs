using System.Linq;
using UnityEngine;

public class InventoryController : MonoBehaviour
{
    [SerializeField]
    private InventoryItem[] InventoryItems;

    public InventoryItem GetItem(int id)
    {
        return InventoryItems.SingleOrDefault(x => x.ID == id);
    }
}

