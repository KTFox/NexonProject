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

    [Header("Deliver Settings")]
    [SerializeField] private float deliverRate;
    [SerializeField] private float deliverDuration;

    [Header("Detection Settings")]
    [SerializeField] private Transform detectPoint;
    [SerializeField] private float detectRadius;
    [SerializeField] private LayerMask itemLayer;
    [SerializeField] private LayerMask interactableLayer;

    private Stack<StackableItem> stackedItems = new Stack<StackableItem>();
    private ItemType currentCarriedType;

    private float nextDeliverTime;


    private void Update()
    {
        DetectItems();
        DetectZones();
    }

    private void OnDrawGizmos()
    {
        if (detectPoint != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(detectPoint.position, detectRadius);
        }
    }

    private void DetectItems()
    {
        if (stackedItems.Count >= maxCapacity)
            return;

        Collider[] hits = Physics.OverlapSphere(detectPoint.position, detectRadius, itemLayer);
        foreach (Collider hit in hits)
        {
            if (stackedItems.Count >= maxCapacity)
                break;

            StackableItem stackableItem = hit.GetComponentInParent<StackableItem>();
            if (stackableItem != null)
            {
                if (stackableItem.IsStacked)
                    continue;

                if (stackedItems.Count == 0 || currentCarriedType == stackableItem.Type)
                {
                    Pickup(stackableItem);
                }
            }
        }
    }

    private void DetectZones()
    {
        Collider[] hits = Physics.OverlapSphere(detectPoint.position, detectRadius, interactableLayer);
        foreach (Collider hit in hits)
        {
            if (hit.TryGetComponent(out InputZone inputZone))
            {
                if (stackedItems.Count == 0 || !inputZone.CanAcceptItem(currentCarriedType) || Time.time < nextDeliverTime)
                    continue;

                PutItemToInputZone(inputZone);
                nextDeliverTime = Time.time + deliverRate;
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
        Quaternion targetLocalRotation = Quaternion.Euler(item.StackOnHandRotation);

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

    private void PutItemToInputZone(InputZone inputZone)
    {
        StackableItem itemToDrop = stackedItems.Pop();
        itemToDrop.transform.SetParent(null);
        inputZone.RegisterIncomingItem();

        Vector3 dropPosition = inputZone.GetNextDropPosition(itemToDrop.Height);
        Quaternion dropRotation = Quaternion.Euler(itemToDrop.StackOnZoneRotation);
        StartCoroutine(MoveItemToZone(itemToDrop, dropPosition, dropRotation, inputZone));
    }

    private IEnumerator MoveItemToZone(StackableItem item, Vector3 targetWorldPosition, Quaternion targetRotation, InputZone zone)
    {
        Vector3 startPosition = item.transform.position;
        Quaternion startRotation = item.transform.rotation;
        float elapsedTime = 0f;

        while (elapsedTime < deliverDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / deliverDuration);

            Vector3 currentPos = Vector3.Lerp(startPosition, targetWorldPosition, t);
            Quaternion currentRot = Quaternion.Lerp(startRotation, targetRotation, t);

            currentPos.y += Mathf.Sin(t * Mathf.PI) * arcHeight;

            item.transform.position = currentPos;
            item.transform.rotation = currentRot;

            yield return null;
        }

        item.transform.position = targetWorldPosition;
        item.transform.rotation = targetRotation;

        zone.AddItemToStack(item);
    }
}
