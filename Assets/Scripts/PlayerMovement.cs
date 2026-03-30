using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Character : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpForce = 9f;
    
    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float checkRadius = 0.25f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Спрайты девочки")]
    [SerializeField] private Sprite girlRightSprite;
    [SerializeField] private Sprite girlLeftSprite;
    
    [Header("Спрайты мальчика")]
    [SerializeField] private Sprite boyRightSprite;
    [SerializeField] private Sprite boyLeftSprite;
    
    [Header("Текущий персонаж")]
    [SerializeField] private int currentCharacter = 0; // 0 = девочка, 1 = мальчик

    private bool isGrounded = false;
    private float moveInput = 0f;
    private bool jumpPressed = false;

    private Rigidbody2D rb;
    private SpriteRenderer sprite;
    private Sprite currentRightSprite;
    private Sprite currentLeftSprite;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponentInChildren<SpriteRenderer>();
        
        if (groundCheck == null)
            groundCheck = transform.Find("GroundCheck");

        // Устанавливаем начального персонажа
        ChangeCharacter(currentCharacter); // Исправлено: вместо SetCharacter
    }

    private void FixedUpdate()
    {
        CheckGround();
        
        // Движение
        rb.linearVelocity = new Vector2(moveInput * speed, rb.linearVelocity.y);
        
        // Прыжок
        if (jumpPressed && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            jumpPressed = false;
            isGrounded = false;
        }
    }

    private void Update()
    {
        // Смена спрайта в зависимости от направления
        if (moveInput > 0)
        {
            sprite.sprite = currentRightSprite;
        }
        else if (moveInput < 0)
        {
            sprite.sprite = currentLeftSprite;
        }
    }

    private void CheckGround()
    {
        if (groundCheck == null) return;
        
        Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheck.position, checkRadius, groundLayer);
        isGrounded = colliders.Length > 0;
    }

    // Метод для смены персонажа
    public void ChangeCharacter(int characterIndex)
    {
        if (characterIndex == 0)
        {
            currentCharacter = 0;
            currentRightSprite = girlRightSprite;
            currentLeftSprite = girlLeftSprite;
            Debug.Log("Персонаж изменен на девочку");
        }
        else if (characterIndex == 1)
        {
            currentCharacter = 1;
            currentRightSprite = boyRightSprite;
            currentLeftSprite = boyLeftSprite;
            Debug.Log("Персонаж изменен на мальчика");
        }
        
        // Обновляем текущий спрайт
        if (moveInput > 0)
            sprite.sprite = currentRightSprite;
        else if (moveInput < 0)
            sprite.sprite = currentLeftSprite;
        else
            sprite.sprite = currentRightSprite;
    }

    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>().x;
    }
    
    public void OnJump(InputValue value)
    {
        if (value.isPressed && isGrounded)
        {
            jumpPressed = true;
        }
    }
}