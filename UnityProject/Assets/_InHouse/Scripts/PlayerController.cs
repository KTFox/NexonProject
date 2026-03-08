using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Componenets")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Animator animator;

    [Header("Movement")]
    [SerializeField] private Joystick joyStick;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float rotationSpeed;

    private Camera mainCamera;

    private Vector3 moveDirection;


    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        UpdateMoveDirection();
        HandleMoving();
        HandleRotating();
        HandleAnimation();
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

    private void HandleAnimation()
    {
        animator.SetBool("isMoving", moveDirection != Vector3.zero);
    }
}
