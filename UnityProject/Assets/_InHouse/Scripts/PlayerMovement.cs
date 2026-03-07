using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Joystick joyStick;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float rotationSpeed;

    private Rigidbody rb;
    private Camera mainCamera;

    private Vector3 moveDirection;


    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        UpdateMoveDirection();
        HandleMoving();
        HandleRotating();
    }

    private void UpdateMoveDirection()
    {
        if (joyStick.Direction.magnitude <= 0.01f)
        {
            moveDirection = Vector3.zero;
            return;
        }

        Vector3 rawInputDirection = new Vector3(joyStick.Direction.x, 0, joyStick.Direction.y);

        Vector3 camForward = mainCamera.transform.forward;
        Vector3 camRight = mainCamera.transform.right;

        camForward.y = 0;
        camRight.y = 0;

        camForward.Normalize();
        camRight.Normalize();

        moveDirection = (camForward * rawInputDirection.z + camRight * rawInputDirection.x).normalized;
    }

    private void HandleMoving()
    {
        Vector3 targetVelocity = new Vector3(moveDirection.x, 0, moveDirection.z) * moveSpeed;
        rb.linearVelocity = targetVelocity;
    }

    private void HandleRotating()
    {
        if (moveDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            rb.rotation = Quaternion.Slerp(rb.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }
}
