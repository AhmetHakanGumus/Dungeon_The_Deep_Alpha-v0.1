// ============================================================
//  PlayerController.cs  —  Güncel Versiyon (Blend Tree + Knockback)
// ============================================================

using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Hareket Ayarları")]
    public float moveSpeed = 5f;

    [Header("Geri Tepme (Knockback) Ayarları")]
    [Tooltip("Büyü atıldığında geriye savrulma şiddeti")]
    public float knockbackForce = 8f;
    [Tooltip("Geriye savrulmanın ne kadar süreceği (saniye)")]
    public float knockbackDuration = 0.15f;

    // --- BİLEŞENLER ---
    Rigidbody2D rb;
    DirectionController directionController;
    ChargeSystem chargeSystem;
    Vector2 moveInput;
    Animator anim;

    // --- DAHİLİ DEĞİŞKENLER ---
    Vector2 lastMoveDirection = Vector2.right;
    float knockbackCounter; // Geri tepme süresini sayar
    Vector2 knockbackVelocity; // Geri tepme hız vektörü

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        directionController = GetComponent<DirectionController>();
        chargeSystem = GetComponent<ChargeSystem>();
        anim = GetComponent<Animator>();

        if (rb == null)
            Debug.LogError("[Player] Rigidbody2D eksik!");
    }

    void Update()
    {
        // Geri tepme süresi devam ediyorsa, süreyi azalt
        if (knockbackCounter > 0)
        {
            knockbackCounter -= Time.deltaTime;
        }

        // ── 1. Charge (Büyü Doldurma) Durumu ──────────
        bool lockedByCharge = chargeSystem != null && chargeSystem.IsCharging;

        if (anim != null)
            anim.SetBool("isCharging", lockedByCharge);

        if (lockedByCharge)
        {
            moveInput = Vector2.zero; // Girişi engelle

            if (anim != null)
            {
                anim.SetBool("isMoving", false);

                // Fareye baktığımız yönü al ve Animator'a gönder
                // Böylece Blend Tree sağa mı sola mı Charge yapacağını bilir
                if (directionController != null)
                {
                    anim.SetFloat("MoveX", directionController.FacingDirection.x);
                    anim.SetFloat("MoveY", directionController.FacingDirection.y);

                    // Şarj bittiğinde karakterin fareye baktığı yönde kalmasını sağla
                    lastMoveDirection = directionController.FacingDirection;
                }
            }
            return;
        }

        // ── 2. Normal Hareket Girişi ──────────────
        Vector2 input = Vector2.zero;

        if (Keyboard.current.wKey.isPressed) input.y = 1f;
        if (Keyboard.current.sKey.isPressed) input.y = -1f;
        if (Keyboard.current.aKey.isPressed) input.x = -1f;
        if (Keyboard.current.dKey.isPressed) input.x = 1f;

        moveInput = input.normalized;

        // ── 3. Animator'a (Blend Tree'ye) Veri Gönderme ──────────────
        if (moveInput.sqrMagnitude > 0)
        {
            lastMoveDirection = moveInput;

            if (anim != null)
            {
                anim.SetFloat("MoveX", moveInput.x);
                anim.SetFloat("MoveY", moveInput.y);
                anim.SetBool("isMoving", true);
            }
        }
        else
        {
            if (anim != null)
            {
                anim.SetFloat("MoveX", lastMoveDirection.x);
                anim.SetFloat("MoveY", lastMoveDirection.y);
                anim.SetBool("isMoving", false);
            }
        }

        if (directionController != null)
            directionController.SetDirection(moveInput);
    }

    void FixedUpdate()
    {
        // Eğer geri tepme yiyorsak normal hareketi ez, bizi geriye fırlat
        if (knockbackCounter > 0)
        {
            rb.linearVelocity = knockbackVelocity;
        }
        else
        {
            // Normal yürüme
            rb.linearVelocity = moveInput * moveSpeed;
        }
    }

    // ── Geri Tepmeyi Tetikleyen Metot (Dışarıdan Çağrılır) ─────────
    public void ApplyKnockback(Vector2 fireDirection)
    {
        knockbackCounter = knockbackDuration;
        // Büyüyü attığımız yönün TERSİNE kuvvet uygula
        knockbackVelocity = -fireDirection.normalized * knockbackForce;
    }
}