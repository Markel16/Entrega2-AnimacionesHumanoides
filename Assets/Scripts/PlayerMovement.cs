using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]
public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 3f;
    public float rotationSpeed = 720f;
    public float jumpForce = 5f;
    public float gravity = -9.81f;

    private CharacterController controller;
    private Animator animator;
    private Vector3 velocity;
    private bool isJumping = false;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        // Por defecto NO usamos root motion para caminar/saltar
        animator.applyRootMotion = false;
    }

    void Update()
    {
        HandleMovement();
        HandleJump();
    }

    void HandleMovement()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 input = new Vector3(h, 0f, v);

        if (input.magnitude > 1f)
            input.Normalize();

        // Rotar hacia la dirección de movimiento si hay input
        if (input.magnitude > 0.1f)
        {
            Quaternion targetRot = Quaternion.LookRotation(input, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
        }

        // Movimiento en plano XZ
        Vector3 move = input * moveSpeed;
        controller.Move(move * Time.deltaTime);

        // Actualizar parámetro Speed 
        float speedParam = move.magnitude;
        animator.SetFloat("Speed", speedParam);
    }

    void HandleJump()
    {
        
        if (controller.isGrounded)
        {
            if (velocity.y < 0f)
                velocity.y = -1f;

            // Salto
            if (Input.GetKeyDown(KeyCode.Space))
            {
                velocity.y = jumpForce;
                isJumping = true;
                animator.SetBool("IsJumping", true);
            }
            else
            {
                
                if (isJumping && velocity.y <= 0f)
                {
                    isJumping = false;
                    animator.SetBool("IsJumping", false);
                }
            }
        }

        
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}
