// ============================================================
//  FieldOfView.cs  —  Görüş Konisi (Gizlilik Mekaniği)
//
//  AMAÇ:
//    Düşman 360° değil, sadece önündeki belirli bir açıda görebilir.
//    Oyuncu bu açının dışında kalırsa tespit edilmez → gizlilik!
//
//  ÇALIŞMA PRENSİBİ:
//    1) Oyuncu mesafe içinde mi?  (detectionRange)
//    2) Oyuncu görüş açısı içinde mi?  (fieldOfViewAngle)
//    3) Aralarında engel var mı?  (Physics2D.Raycast ile duvar kontrolü)
//    Üçü de evet → oyuncu görüldü!
//
//  KULLANIM:
//    Enemy objesine ekle. DirectionController da olmalı.
//    EnemyFSM.cs içinden: fieldOfView.CanSeePlayer()
// ============================================================

using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    // ── Inspector ────────────────────────────────────────────
    [Header("Görüş Ayarları")]
    [Range(10f, 360f)]
    [Tooltip("Görüş konisinin toplam açısı (derece). 90 = dar koni, 180 = yarı daire")]
    public float fieldOfViewAngle = 90f;

    [Tooltip("Görüş mesafesi")]
    public float detectionRange = 5f;

    [Tooltip("Oyuncu bu kadar yakınsa açı fark etmez, her zaman görülür (0 = kapat)")]
    public float alwaysDetectRange = 0.8f;

    [Header("Engel Kontrolü")]
    [Tooltip("Duvar ve engellerin bulunduğu Layer. Inspector'dan 'Obstacles' layer'ı ata.")]
    public LayerMask obstacleLayer;

    [Header("Hata Ayıklama")]
    [Tooltip("Editor'da görüş konisini göster")]
    public bool showDebugGizmos = true;

    // ── Bağlantılar ──────────────────────────────────────────
    DirectionController directionController;

    // ── Sonuç ─────────────────────────────────────────────────
    // Dışarıdan okunur: şu an oyuncu görülüyor mu?
    public bool PlayerVisible { get; private set; } = false;

    // Son görülen oyuncunun pozisyonu (kovalama için)
    public Vector2 LastKnownPlayerPosition { get; private set; }

    // ─────────────────────────────────────────────────────────
    void Awake()
    {
        directionController = GetComponent<DirectionController>();

        if (directionController == null)
            Debug.LogError($"[{gameObject.name}] FieldOfView: DirectionController bulunamadı! Lütfen ekle.");
    }

    void Update()
    {
        // Her karede oyuncu görünür mü kontrol et
        CheckForPlayer();
    }

    // ── Ana Kontrol Metodu ────────────────────────────────────
    void CheckForPlayer()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj == null)
        {
            PlayerVisible = false;
            return;
        }

        Vector2 playerPos = playerObj.transform.position;
        Vector2 myPos = transform.position;
        float distance = Vector2.Distance(myPos, playerPos);

        // 1) Mesafe kontrolü
        if (distance > detectionRange)
        {
            PlayerVisible = false;
            return;
        }

        // 2) Çok yakındaysa açıya bakmadan her zaman gör
        if (distance <= alwaysDetectRange)
        {
            PlayerVisible = true;
            LastKnownPlayerPosition = playerPos;
            return;
        }

        // 3) Açı kontrolü: oyuncu görüş konisi içinde mi?
        if (!IsInFieldOfView(playerPos, myPos))
        {
            PlayerVisible = false;
            return;
        }

        // 4) Engel kontrolü: aralarında duvar var mı?
        if (IsBlockedByObstacle(myPos, playerPos))
        {
            PlayerVisible = false;
            return;
        }

        // Tüm kontrollerden geçti → oyuncu görüldü!
        PlayerVisible = true;
        LastKnownPlayerPosition = playerPos;
    }

    // ── Görüş Açısı Kontrolü ─────────────────────────────────
    bool IsInFieldOfView(Vector2 playerPos, Vector2 myPos)
    {
        // Oyuncuya olan yön
        Vector2 dirToPlayer = (playerPos - myPos).normalized;

        // Baktığım yön ile oyuncuya olan yön arasındaki açı
        float angle = Vector2.Angle(directionController.FacingDirection, dirToPlayer);

        // Açı, görüş konisinin yarısından küçükse içeride
        return angle <= fieldOfViewAngle * 0.5f;
    }

    // ── Engel Kontrolü ───────────────────────────────────────
    bool IsBlockedByObstacle(Vector2 myPos, Vector2 playerPos)
    {
        // obstacleLayer 0 ise engel kontrolünü atla
        if (obstacleLayer == 0) return false;

        Vector2 direction = (playerPos - myPos).normalized;
        float distance = Vector2.Distance(myPos, playerPos);

        // Benden oyuncuya doğru ışın gönder
        RaycastHit2D hit = Physics2D.Raycast(myPos, direction, distance, obstacleLayer);

        // Bir şeye çarptıysa engel var demektir
        return hit.collider != null;
    }

    // ── Dışarıdan Çağrılabilir ───────────────────────────────
    // EnemyFSM içinden kullanım: if (fieldOfView.CanSeePlayer()) ...
    public bool CanSeePlayer()
    {
        return PlayerVisible;
    }

    // ── Gizmos: Editor'da Görüş Konisini Çiz ─────────────────
    void OnDrawGizmosSelected()
    {
        if (!showDebugGizmos) return;

        Vector2 myPos = transform.position;

        // Baktığım yönü belirle
        Vector2 facing = Vector2.right; // Default
        if (directionController != null)
            facing = directionController.FacingDirection;

        // Görüş yarıçapı (sarı daire)
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(myPos, detectionRange);

        // Her zaman algılama mesafesi (kırmızı daire)
        if (alwaysDetectRange > 0)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(myPos, alwaysDetectRange);
        }

        // Görüş konisi (iki çizgi)
        Gizmos.color = new Color(1f, 0.9f, 0f, 0.8f);
        float halfAngle = fieldOfViewAngle * 0.5f;

        // Sol sınır
        Vector2 leftBound = RotateVector(facing, -halfAngle) * detectionRange;
        // Sağ sınır
        Vector2 rightBound = RotateVector(facing, halfAngle) * detectionRange;

        Gizmos.DrawLine(myPos, myPos + leftBound);
        Gizmos.DrawLine(myPos, myPos + rightBound);

        // Merkez çizgi (baktığım yön)
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(myPos, myPos + facing * detectionRange);
    }

    // Vektörü belirli açı kadar döndür (yardımcı metot)
    Vector2 RotateVector(Vector2 v, float angleDeg)
    {
        float rad = angleDeg * Mathf.Deg2Rad;
        float cos = Mathf.Cos(rad);
        float sin = Mathf.Sin(rad);
        return new Vector2(v.x * cos - v.y * sin, v.x * sin + v.y * cos);
    }
}