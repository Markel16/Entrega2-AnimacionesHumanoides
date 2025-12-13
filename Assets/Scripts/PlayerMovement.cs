using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movimiento")]
    public float moveSpeed = 3f;
    public float rotationSpeed = 720f;

    [Header("Salto")]
    public float jumpForce = 5f;
    public float gravity = -9.81f;

    [Header("Agarre")]
    public Transform hangPoint;          
    public KeyCode hangKey = KeyCode.E;  

    private CharacterController controller;
    private Animator animator;

    private Vector3 velocity;
    private bool isJumping = false;
    private bool isHolding = false;

    private RagdollManager ragdoll;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        ragdoll = GetComponent<RagdollManager>();

        animator.applyRootMotion = false;
    }

    void Update()
    {
        
        if (!controller.enabled)
            return;

        
        if (Input.GetKeyDown(hangKey))
        {
            if (!isHolding) StartHanging();
            else StopHanging();
        }

        
        if (isHolding)
        {
            HandleHanging();
        }
        else
        {
            HandleMovementAndJump();
        }

        
        animator.SetBool("IsHolding", isHolding);
        animator.SetLayerWeight(1, isHolding ? 1f : 0f);
    }



    
    void HandleMovementAndJump()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 input = new Vector3(h, 0f, v);

        if (input.magnitude > 1f)
            input.Normalize();

       
        if (input.magnitude > 0.1f)
        {
            Quaternion targetRot = Quaternion.LookRotation(input, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                targetRot,
                rotationSpeed * Time.deltaTime
            );
        }

        Vector3 horizontalMove = input * moveSpeed;

        
        if (controller.isGrounded)
        {
            if (velocity.y < 0f)
            {
                velocity.y = -2f;

                if (isJumping)
                {
                    isJumping = false;
                    animator.SetBool("IsJumping", false);
                }
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                velocity.y = jumpForce;
                isJumping = true;
                animator.SetBool("IsJumping", true);
                animator.SetTrigger("IsJumpingT");
                print("Jump");
            }
        }

       
        velocity.y += gravity * Time.deltaTime;

        
        Vector3 finalMove = horizontalMove;
        finalMove.y = velocity.y;
        controller.Move(finalMove * Time.deltaTime);

        
        float speedParam = horizontalMove.magnitude;
        animator.SetFloat("Speed", speedParam);
    }

    
    void StartHanging()
    {
        if (hangPoint == null) return;

        isHolding = true;

        
        controller.enabled = false;
        transform.position = hangPoint.position;
        controller.enabled = true;

        
        velocity = Vector3.zero;

        
        animator.SetBool("IsHolding", true);
        
        animator.SetFloat("Speed", 0f);
    }

    void StopHanging()
    {
        isHolding = false;

        animator.SetBool("IsHolding", false);
        
        velocity.y = 0f;

        if (ragdoll != null)
        {
            ragdoll.EnableRagdoll();
        }
    }

   
    void HandleHanging()
    {
        
        float h = Input.GetAxis("Horizontal");  

        
        Vector3 sideMove = new Vector3(h, 0f, 0f);  
        controller.Move(sideMove * moveSpeed * Time.deltaTime);

       
        float speedParam = Mathf.Abs(h);
        animator.SetFloat("Speed", speedParam);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            StopHanging();
        }
    }
}
