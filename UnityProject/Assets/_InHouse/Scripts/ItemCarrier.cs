using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemCarrier : MonoBehaviour
{
    [Header("Carrier Properties")]
    [SerializeField] private Transform stackedItemsParent;
    [SerializeField] private int maxCapacity;
    [SerializeField] private float stackDuration;
    [SerializeField] private float arcHeight;

    [Header("Detection Settings")]
    [SerializeField] private Transform detectPoint;
    [SerializeField] private float detectRadius;
    [SerializeField] private LayerMask itemLayer;

    private Stack<StackableItem> stackedItems = new Stack<StackableItem>();
    private ItemType currentCarriedType;


    private void Update()
    {
        DetectAndPickupItems();
    }

    private void OnDrawGizmos()
    {
        if (detectPoint != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(detectPoint.position, detectRadius);
        }
    }

    private void DetectAndPickupItems()
    {
        if (stackedItems.Count >= maxCapacity)
            return;

        Collider[] hits = Physics.OverlapSphere(detectPoint.position, detectRadius, itemLayer);
        foreach (Collider hit in hits)
        {
            if (stackedItems.Count >= maxCapacity)
                break;

            StackableItem item = hit.GetComponentInParent<StackableItem>();
            if (item != null)
            {
                if (item.IsStacked)
                    continue;

                if (stackedItems.Count == 0 || currentCarriedType == item.Type)
                {
                    Pickup(item);
                }
            }
        }
    }

    private void Pickup(StackableItem item)
    {
        if (stackedItems.Count == 0)
        {
            currentCarriedType = item.Type;
        }

        item.SetStackedState();
        item.transform.SetParent(stackedItemsParent);

        Vector3 targetLocalPosition = new Vector3(0, stackedItems.Count * item.Height, 0);
        Quaternion targetLocalRotation = Quaternion.Euler(item.StackRotation);

        stackedItems.Push(item);

        StartCoroutine(MoveAndRotateItem(item, targetLocalPosition, targetLocalRotation));
    }

    private IEnumerator MoveAndRotateItem(StackableItem item, Vector3 targetLocalPosition, Quaternion targetLocalRotation)
    {
        Vector3 startPosition = item.transform.localPosition;
        Quaternion startRotation = item.transform.localRotation;
        float elapsedTime = 0f;

        while (elapsedTime < stackDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / stackDuration);

            Vector3 currentPos = Vector3.Lerp(startPosition, targetLocalPosition, t);
            currentPos.y += Mathf.Sin(t * Mathf.PI) * arcHeight;

            item.transform.localPosition = currentPos;
            item.transform.localRotation = Quaternion.Lerp(startRotation, targetLocalRotation, t);

            yield return null;
        }

        item.transform.localPosition = targetLocalPosition;
        item.transform.localRotation = targetLocalRotation;
    }
}
