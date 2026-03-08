using UnityEngine;

public enum ItemType
{
    Item1,
    Item2
}

public class StackableItem : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Collider col;

    [Header("Stacking Item Properties")]
    [SerializeField] private ItemType type;
    [SerializeField] private float height;
    [SerializeField] private Vector3 stackRotation;

    private bool isStacked;


    public ItemType Type => type;
    public float Height => height;
    public Vector3 StackRotation => stackRotation;
    public bool IsStacked => isStacked;


    public void SetStackedState()
    {
        rb.isKinematic = true;
        col.enabled = false;
        isStacked = true;
    }
}
