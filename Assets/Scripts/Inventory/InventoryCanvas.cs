using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryCanvas : MonoBehaviour
{
    public GameObject Slot;
    public Transform Panel;
    [SerializeField]
    private Inventory _AttachedInv;

    [HideInInspector]
    public List<SlotObject> currentSlots = new List<SlotObject>();

    public delegate void InventorySlotArg(int Index, InventoryCanvas canvas);
    public InventorySlotArg SlotSelectedEvent;


    public Inventory AttachedInventory {
        get
        {
            return _AttachedInv;
        }
        set
        {
            _AttachedInv = value;
            ChangeSlots(_AttachedInv == null ? 0 : _AttachedInv.InventorySize);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        if(_AttachedInv != null)
        {
            ChangeSlots(_AttachedInv.InventorySize);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_AttachedInv != null)
        {
            for(var i=0; i < _AttachedInv.InventorySize; ++i)
            {
                bool isEmpty = _AttachedInv.InventorySlots[i].Item == null || _AttachedInv.InventorySlots[i].Amount == 0;
                currentSlots[i].ItemAmount.text = isEmpty ? " " : _AttachedInv.InventorySlots[i].Amount.ToString();
                currentSlots[i].ItemImage.sprite = isEmpty ? null : _AttachedInv.InventorySlots[i].Item.Image;
                currentSlots[i].ItemImage.color = isEmpty ? new Color(1,1,1,0) : Color.white;
            }
        }
    }

    private void ChangeSlots(int count)
    {
        foreach(SlotObject obj in currentSlots)
        {
            Destroy(obj.gameObject);
        }
        currentSlots.Clear();
        for (var i = 0; i < count; ++i)
        {
            RectTransform rTransform = Instantiate(Slot, transform.position, transform.rotation).GetComponent<RectTransform>();
            rTransform.parent = Panel;
            rTransform.localScale = Vector3.one;
            SlotObject slot = rTransform.gameObject.GetComponent<SlotObject>();
            slot.SlotEvent = SlotSelected;
            slot.Index = i;
            currentSlots.Add(slot);
        }
    }
    
    public void SlotSelected(int index)
    {
        SlotSelectedEvent(index, this);
    }
}
