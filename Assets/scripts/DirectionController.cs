// ============================================================
//  DirectionController.cs  —  Yön Sistemi (Güncellenmiş)
// ============================================================

using UnityEngine;

public class DirectionController : MonoBehaviour
{
    // ── Inspector ────────────────────────────────────────────
    [Header("Bağlantılar")]
    [Tooltip("Flip uygulanacak SpriteRenderer. Boş bırakılırsa bu objedeki bulunur.")]
    public SpriteRenderer spriteRenderer;

    [Header("Ayarlar")]
    [Tooltip("Sprite varsayılan olarak sağa mı bakıyor?")]
    public bool defaultFacingRight = true;

    // YENİ EKLENEN KISIM BURASI:
    [Tooltip("Yön değiştiğinde Sprite ters çevrilsin mi? (Düşmanlarda AÇIK, Oyuncuda KAPALI olmalı)")]
    public bool enableSpriteFlip = true;

    // ── Dışarıdan Okunabilir Özellikler ──────────────────────
    public Vector2 FacingDirection { get; private set; } = Vector2.right;
    public bool IsFacingRight { get; private set; } = true;
    public bool IsMoving { get; private set; } = false;

    // ─────────────────────────────────────────────────────────
    void Awake()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        IsFacingRight = defaultFacingRight;
        FacingDirection = defaultFacingRight ? Vector2.right : Vector2.left;
    }

    public void SetDirection(Vector2 movement)
    {
        IsMoving = movement.sqrMagnitude > 0.01f;

        if (!IsMoving) return;

        FacingDirection = movement.normalized;

        if (movement.x > 0.01f)
            SetFacing(true);   // Sağa
        else if (movement.x < -0.01f)
            SetFacing(false);  // Sola
    }

    public void LookAt(Vector2 targetPosition)
    {
        Vector2 dir = ((Vector2)targetPosition - (Vector2)transform.position);

        if (dir.sqrMagnitude < 0.01f) return;

        FacingDirection = dir.normalized;

        if (dir.x > 0.01f)
            SetFacing(true);
        else if (dir.x < -0.01f)
            SetFacing(false);
    }

    void SetFacing(bool facingRight)
    {
        if (IsFacingRight == facingRight) return;

        IsFacingRight = facingRight;

        // YENİ EKLENEN KORUMA BURASI: Eğer flip kapalıysa resmi ters çevirme
        if (spriteRenderer == null || !enableSpriteFlip) return;

        spriteRenderer.flipX = defaultFacingRight ? !facingRight : facingRight;
    }

    public float GetFacingAngle()
    {
        return Mathf.Atan2(FacingDirection.y, FacingDirection.x) * Mathf.Rad2Deg;
    }
}