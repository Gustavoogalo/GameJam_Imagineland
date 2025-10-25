using System;
using Player;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class Player_Moviment : MonoBehaviour
{
    private CharacterController controller;
    private float turnSmoothVelocity;
    private Vector3 playerVelocity; // Para velocidade vertical (gravidade e pulo)
    private Vector3 horizontalVelocity; // Para velocidade horizontal persistente no ar
    public float speed = 5f;
    [SerializeField] private float turnSmoothTime = 0.1f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float jumpHeight = 2f; // Altura do pulo, ajuste conforme necessário

    private Vector3 _input; // Entrada horizontal atual

    private InputSystem_Actions inputSystem;
    
    private float guardedSpeed;

    [Header("Damage Settings")] [SerializeField]
    private TriggerCheck damageCollider;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        inputSystem = new InputSystem_Actions();
        inputSystem.Enable(); // Habilita o sistema de entrada
    }

    void Start()
    {
        guardedSpeed = speed;
    }

    private void OnEnable()
    {
        inputSystem.Player.Jump.performed += OnJump;

        damageCollider.EnteredTrigger += OnDamageEnemy;
        damageCollider.ExitedTrigger += OnExitedEnemy;
    }

    private void OnDisable()
    {
        inputSystem.Player.Jump.performed -= OnJump;

        damageCollider.EnteredTrigger -= OnDamageEnemy;
        damageCollider.ExitedTrigger -= OnExitedEnemy;

        inputSystem.Disable(); // Desabilita o sistema de entrada
    }

    // Update is called once per frame
    void Update()
    {
        // Lê a entrada de movimento diretamente
        Vector2 moveInput = inputSystem.Player.Move.ReadValue<Vector2>();
        _input = new Vector3(moveInput.x, 0, moveInput.y);

        HandleGravity();
        OnMove();
        //DamageOnAir();
    }

    public void OnMove()
    {
        Vector3 direction = _input.normalized;

        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity,
                turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            horizontalVelocity = moveDir * speed; // Atualiza a velocidade horizontal
        }
        else if (controller.isGrounded)
        {
            horizontalVelocity = Vector3.zero; // Zera a velocidade horizontal quando parado no chão
        }

       
        if (!controller.isGrounded)
        {
            speed = guardedSpeed / 2;
        }
        else
        {
            speed = guardedSpeed;
        }
        // Se não grounded, mantém a horizontalVelocity da última direção

        // Sempre aplica movimento: horizontal + vertical
        controller.Move((horizontalVelocity + playerVelocity) * Time.deltaTime);
    }

    public void OnJump(InputAction.CallbackContext callbackContext)
    {
        if (controller.isGrounded)
        {
            // Calcula a velocidade inicial para alcançar a altura do pulo
            playerVelocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }

    private void HandleGravity()
    {
        if (controller.isGrounded && playerVelocity.y < 0)
        {
            playerVelocity.y = -2f;
        }

        playerVelocity.y += gravity * Time.deltaTime;
    }

    private void DamageOnAir()
    {
        if (!controller.isGrounded)
        {
            damageCollider.GetComponent<Collider>().enabled = true;
        }
        else
        {
            damageCollider.GetComponent<Collider>().enabled = false;
        }
    }

    private void OnDamageEnemy(Collider other)
    {
        int enemyLayer = LayerMask.NameToLayer("Enemy");

        if (other.gameObject.layer == enemyLayer)
        {
            var health = other.GetComponent<HealthSystem>();
            var damageInfo = new HSS_DamageInfo()
            {
                Amount = 1
            };
            if (health != null)
            {
                health.TakeDamage(damageInfo);
            }
        }
    }

    private void OnExitedEnemy(Collider other)
    {
        // int enemyLayer = LayerMask.NameToLayer("Enemy");
        //
        // if (other.gameObject.layer == enemyLayer)
        // {
        //     var health = other.GetComponent<HealthSystem>();
        //     var damageInfo = new HSS_DamageInfo()
        //     {
        //         Amount = 1
        //     };
        //     health.TakeDamage(damageInfo);
        // }
    }
}