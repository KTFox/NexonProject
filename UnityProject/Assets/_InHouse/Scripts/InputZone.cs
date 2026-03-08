using System.Collections.Generic;
using UnityEngine;

public class InputZone : MonoBehaviour
{
    [SerializeField] private ItemType acceptedItemType;
    [SerializeField] private int maxCapacity;
    [SerializeField] private Transform stackedItemParent;

    private Stack<StackableItem> stackableItems = new Stack<StackableItem>();
    private int incomingCount;


    public bool CanAcceptItem(ItemType itemType)
    {
        if (stackableItems.Count + incomingCount >= maxCapacity)
            return false;

        if (itemType != acceptedItemType)
            return false;

        return true;
    }

    public Vector3 GetNextDropPosition(float itemHeight)
    {
        float targetY = (stackableItems.Count + incomingCount) * itemHeight;
        return stackedItemParent.position + new Vector3(0, targetY, 0);
    }

    public void RegisterIncomingItem()
    {
        incomingCount++;
    }

    public void AddItemToStack(StackableItem item)
    {
        incomingCount--;
        stackableItems.Push(item);
        item.transform.SetParent(stackedItemParent);
    }
}
