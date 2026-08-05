using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    [SerializeField] private PlayerMotor motor;
    [SerializeField] private Animator animator;

    private void Start()
    {
        if (motor == null)
        {
            motor = GetComponent<PlayerMotor>();
        }

        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }
    }

    private void Update()
    {
        if (animator == null || motor == null) return;

        animator.SetFloat("Speed", motor.CurrentSpeedPercent);
        animator.SetBool("IsCrouching", motor.IsCrouching);

        if (Input.GetButtonDown("Jump") && motor.IsGrounded && !motor.IsCrouching)
        {
            animator.SetTrigger("Jump");
        }
    }
}