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
    public Transform hangPoint;          // punto de la barra donde se cuelga
    public KeyCode hangKey = KeyCode.E;  // tecla para agarrarse / soltarse

    private CharacterController controller;
    private Animator animator;

    private Vector3 velocity;
    private bool isJumping = false;
    private bool isHolding = false;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        animator.applyRootMotion = false;
    }

    void Update()
    {
        // Si está agarrado, usamos lógica distinta
        if (isHolding)
        {
            HandleHanging();
        }
        else
        {
            HandleMovementAndJump();
        }

        // Tecla para alternar agarrarse/soltarse
        if (Input.GetKeyDown(hangKey))
        {
            if (!isHolding)
                StartHanging();
            else
                StopHanging();
        }

        // Actualizar parámetro del Animator
        animator.SetBool("IsHolding", isHolding);
    }

    // ---------------- MOVIMIENTO NORMAL + SALTO ----------------
    void HandleMovementAndJump()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 input = new Vector3(h, 0f, v);

        if (input.magnitude > 1f)
            input.Normalize();

        // Rotar hacia la dirección de movimiento
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

        // Suelo / salto
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
            }
        }

        // Gravedad
        velocity.y += gravity * Time.deltaTime;

        // Movimiento final
        Vector3 finalMove = horizontalMove;
        finalMove.y = velocity.y;
        controller.Move(finalMove * Time.deltaTime);

        // Parámetro Speed (para Idle/Walk y también para la capa de Agarrarse)
        float speedParam = horizontalMove.magnitude;
        animator.SetFloat("Speed", speedParam);
    }

    // ---------------- LÓGICA DE AGARRARSE ----------------
    void StartHanging()
    {
        if (hangPoint == null) return;

        isHolding = true;

        // Colocar al personaje justo en el punto de agarre
        controller.enabled = false;
        transform.position = hangPoint.position;
        controller.enabled = true;

        // Resetear la velocidad vertical para que no se caiga
        velocity = Vector3.zero;

        // Activar el bool de Animator (segunda capa se encenderá)
        animator.SetBool("IsHolding", true);
        // Dejar Speed a 0 al empezar colgado
        animator.SetFloat("Speed", 0f);
    }

    void StopHanging()
    {
        isHolding = false;

        animator.SetBool("IsHolding", false);
        // Al soltarse, permitimos que la gravedad vuelva a actuar
        velocity.y = 0f;
    }

    // Mientras está agarrado a la barra
    void HandleHanging()
    {
        // Movimiento lateral sobre la barra (para LeftShimmy)
        float h = Input.GetAxis("Horizontal");   // A / D o flechas

        // Opcional: mover realmente el personaje a lo largo de la barra
        Vector3 sideMove = new Vector3(h, 0f, 0f);  // local X
        controller.Move(sideMove * moveSpeed * Time.deltaTime);

        // Usamos Speed para que la layer de Agarrarse cambie a LeftShimmy
        float speedParam = Mathf.Abs(h);
        animator.SetFloat("Speed", speedParam);

        // Si pulsas Space mientras estás colgado, te sueltas (y caes)
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StopHanging();
        }
    }
}
