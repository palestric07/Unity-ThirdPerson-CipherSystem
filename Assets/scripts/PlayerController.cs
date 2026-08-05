using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMotor : MonoBehaviour
{
    [SerializeField] private CipherStats stats;
    [SerializeField] private Transform cameraTransform;

    [SerializeField] private LayerMask vaultLayer;
    [SerializeField] private float vaultRayDistance = 1.2f;
    [SerializeField] private float vaultHeight = 1.2f;

    private CharacterController controller;
    private Vector3 velocity;
    
    public bool IsGrounded { get; private set; }
    public bool IsCrouching { get; private set; }
    public bool IsVaulting { get; private set; }
    public float CurrentSpeedPercent { get; private set; }

    private void Start()
    {
        controller = GetComponent<CharacterController>();

        if (cameraTransform == null && Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }
    }

    private void Update()
    {
        if (IsVaulting) return;

        HandleGrounded();
        HandleMovement();
        HandleJump();
        HandleCrouch();
        HandleVault();
    }

    private void HandleGrounded()
    {
        IsGrounded = controller.isGrounded;
        if (IsGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
    }

    private void HandleMovement()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;
            transform.rotation = Quaternion.Euler(0f, targetAngle, 0f);

            Vector3 moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

            float currentSpeed = stats.WalkSpeed;
            CurrentSpeedPercent = 0.5f;

            if (Input.GetKey(KeyCode.LeftShift) && !IsCrouching)
            {
                currentSpeed = stats.SprintSpeed;
                CurrentSpeedPercent = 1.0f;
            }
            else if (IsCrouching)
            {
                currentSpeed = stats.CrouchSpeed;
                CurrentSpeedPercent = 0.5f;
            }

            controller.Move(moveDirection.normalized * currentSpeed * Time.deltaTime);
        }
        else
        {
            CurrentSpeedPercent = 0f;
        }

        velocity.y += stats.Gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    private void HandleJump()
    {
        if (Input.GetButtonDown("Jump") && IsGrounded && !IsCrouching)
        {
            velocity.y = Mathf.Sqrt(stats.JumpHeight * -2f * stats.Gravity);
        }
    }

    private void HandleCrouch()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            IsCrouching = !IsCrouching;
        }
    }

    private void HandleVault()
    {
        if (Input.GetKeyDown(KeyCode.E) && IsGrounded && !IsCrouching)
        {
            Vector3 lowerOrigin = transform.position + Vector3.up * 0.2f;
            Vector3 upperOrigin = transform.position + Vector3.up * vaultHeight;

            if (Physics.Raycast(lowerOrigin, transform.forward, vaultRayDistance, vaultLayer))
            {
                if (!Physics.Raycast(upperOrigin, transform.forward, vaultRayDistance, vaultLayer))
                {
                    StartCoroutine(PerformVault());
                }
            }
        }
    }

    private IEnumerator PerformVault()
    {
        IsVaulting = true;
        Vector3 vaultTargetPosition = transform.position + transform.forward * 1.5f + Vector3.up * 1f;
        float elapsedTime = 0f;
        float duration = 0.4f;

        Vector3 startPos = transform.position;

        while (elapsedTime < duration)
        {
            transform.position = Vector3.Lerp(startPos, vaultTargetPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = vaultTargetPosition;
        IsVaulting = false;
    }
}