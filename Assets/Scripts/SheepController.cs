using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class SheepController : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 4f;
    public float rotationSpeed = 10f;
    private bool isReadyToRun;
    public bool isAttacking;

    private Vector2 moveInput;
    private Rigidbody rb;
    private Animator animator;

    private Transform cam;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        isReadyToRun = true;
        isAttacking = true;
        // Freeze Y và rotation X/Z
        rb.constraints =    RigidbodyConstraints.FreezePositionY |
                            RigidbodyConstraints.FreezeRotationX |
                            RigidbodyConstraints.FreezeRotationY |
                            RigidbodyConstraints.FreezeRotationZ;

        cam = Camera.main.transform;
    }

    void FixedUpdate()
    {
        MoveWithCameraDirection();
    }

    void MoveWithCameraDirection()
    {
        if (moveInput.sqrMagnitude < 0.01f)
            return;

        // Hướng camera bỏ độ nghiêng
        Vector3 camForward = cam.forward;
        camForward.y = 0;
        camForward.Normalize();

        Vector3 camRight = cam.right;
        camRight.y = 0;
        camRight.Normalize();

        // Hướng di chuyển
        Vector3 moveDir = camForward * moveInput.y + camRight * moveInput.x;
        moveDir.Normalize();

        // Di chuyển bằng MovePosition, không thay đổi trục Y
        Vector3 newPos = rb.position + moveDir * speed * Time.fixedDeltaTime;
        rb.MovePosition(newPos);

        // Xoay theo hướng di chuyển
        Quaternion targetRot = Quaternion.LookRotation(moveDir);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.fixedDeltaTime);
    }

    public void OnMove(InputValue value)
    {
        animator.SetBool("isWalking", value.Get<Vector2>().magnitude > 0.1f);
        moveInput = value.Get<Vector2>();
    }
    public void OnSprint(InputValue value)
    {
        if(value.isPressed && isReadyToRun && moveInput.sqrMagnitude > 0.01f)
        {
            StartCoroutine(Run());
        }
    }
    private IEnumerator Run()
    {
        isReadyToRun = false;
        animator.SetBool("isRunning", true);
        speed = 8f;
        yield return new WaitForSeconds(3f);
        speed = 4f;
        animator.SetBool("isRunning", false);
        yield return new WaitForSeconds(2f);
        isReadyToRun = true;
    }
    public void OnAttack(InputValue value)
    {
        if(value.isPressed && isAttacking)
        {
            StartCoroutine(Attack());
        }
    }
    public IEnumerator Attack()
    {
        isAttacking = false;
        animator.SetBool("isAttacking", true);
        yield return new WaitForSeconds(0.5f);
        isAttacking = true;
        animator.SetBool("isAttacking", false);
    }
}
