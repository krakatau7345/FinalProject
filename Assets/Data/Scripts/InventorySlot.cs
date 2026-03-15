using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class InventorySlot
{
    public Item item;
    public int amount;

    public bool IsEmpty()
    {
        return item == null;
    }

    public void AddItem(Item newItem, int count)
    {
        item = newItem;
        amount = count;
    }

    public void ClearSlot()
    {
        item = null;
        amount = 0;
    }
}
