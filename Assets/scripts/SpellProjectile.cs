using UnityEngine;

public class SpellProjectile : MonoBehaviour
{
    [Header("Uçuş Ayarları")]
    public float speed = 10f;
    public float lifetime = 3f;

    [Header("Büyü Tipi (Alan vs Tekil)")]
    [Tooltip("Eğer bu Kırmızı Büyü ise (Alan vuracaksa) tik atın!")]
    public bool isAoE = false;
    public float baseRadius = 1.5f; 
    private float actualRadius;     

    [Header("Hasar")]
    public int damage = 25;

    [Header("Engel Katmanları")]
    public LayerMask obstacleLayer;

    Vector2 flyDirection;
    Rigidbody2D rb;
    bool hasHit = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        actualRadius = baseRadius; // Güvenlik önlemi: Launch çalışmadan önce çarparsa çap 0 olmasın
        Destroy(gameObject, lifetime);
    }

    public void Launch(Vector2 direction, int projectileDamage, float radiusBonus = 0f)
    {
        flyDirection = direction.normalized;
        damage = projectileDamage;
        actualRadius = baseRadius + radiusBonus; 

        if (rb != null)
            rb.linearVelocity = flyDirection * speed;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    void OnTriggerEnter2D(Collider2D other)
    {   if (hasHit) return;

        // Büyülerin birbiriyle VE PARALARLA çarpışmasını engelle (Etiketin "Coin" olduğunu varsayıyorum)
        if (other.GetComponent<SpellProjectile>() != null || other.CompareTag("EnemySpell") || other.CompareTag("Coin")) return;
        if (hasHit) return;

        // Büyülerin birbiriyle çarpışmasını engelle
        if (other.GetComponent<SpellProjectile>() != null || other.CompareTag("EnemySpell")) return;

        // Büyüyü kimin attığını anla
        bool isEnemySpell = gameObject.CompareTag("EnemySpell");

        // Kendi sahibini vurmasını engelle (Dost Ateşi)
        if (isEnemySpell && other.CompareTag("Enemy")) return;
        if (!isEnemySpell && other.CompareTag("Player")) return; 

        // Çarptığımız şey geçerli bir hedef mi veya engel mi?
        bool hitTarget = (isEnemySpell && other.CompareTag("Player")) || (!isEnemySpell && other.CompareTag("Enemy"));
        bool hitObstacle = obstacleLayer != 0 && ((1 << other.gameObject.layer) & obstacleLayer) != 0;

        // --- ALAN HASARI (KIRMIZI BÜYÜ) ---
        if (isAoE)
        {
            if (hitTarget || hitObstacle)
            {
                PatlaVeAlanHasariVer();
                HitAndDestroy();
            }
            else if (!other.CompareTag("Player") && !other.CompareTag("Enemy"))
            {
                // Hedef olmayan rastgele bir şeye (örn: yerdeki sandık) çarparsa patlamadan yok ol
                HitAndDestroy();
            }
        }
        // --- TEKİL HASAR (MOR BÜYÜ) ---
        else 
        {
            if (hitTarget)
            {
                if (!isEnemySpell) // Oyuncu Düşmanı Vurdu
                {
                    EnemyHealth enemyHealth = other.GetComponent<EnemyHealth>();
                    if (enemyHealth != null) enemyHealth.TakeDamage(damage);
                }
                else // Düşman Oyuncuyu Vurdu
                {
                    PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
                    if (playerHealth != null) playerHealth.TakeDamage(damage);
                }
                
                HitAndDestroy();
            }
            else if (hitObstacle)
            {
                HitAndDestroy();
            }
            else if (!other.CompareTag("Player") && !other.CompareTag("Enemy"))
            {
                HitAndDestroy();
            }
        }
    }

    void PatlaVeAlanHasariVer()
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, actualRadius);
        bool isEnemySpell = gameObject.CompareTag("EnemySpell");

        foreach (var hit in hitColliders)
        {
            // Eğer büyüyü oyuncu attıysa, alandaki düşmanlar hasar alır
            if (!isEnemySpell && hit.CompareTag("Enemy"))
            {
                EnemyHealth enemy = hit.GetComponent<EnemyHealth>();
                if (enemy != null) enemy.TakeDamage(damage);
            }
            // Eğer büyüyü düşman attıysa, alandaki oyuncu hasar alır
            else if (isEnemySpell && hit.CompareTag("Player"))
            {
                PlayerHealth player = hit.GetComponent<PlayerHealth>();
                if (player != null) player.TakeDamage(damage);
            }
        }
        Debug.Log($"Büyü patladı! Çapı: {actualRadius}");
    }

    void OnDrawGizmosSelected()
    {
        if (isAoE)
        {
            Gizmos.color = Color.red;
            // Editor'de gerçek çapı görmek için "actualRadius" sıfırdan büyükse onu, değilse baseRadius kullan
            Gizmos.DrawWireSphere(transform.position, actualRadius > 0 ? actualRadius : baseRadius);
        }
    }

    void HitAndDestroy()
    {
        hasHit = true;
        ParticleSystem trailParticles = GetComponentInChildren<ParticleSystem>();
        if (trailParticles != null)
        {
            trailParticles.transform.SetParent(null);
            trailParticles.Stop();
            Destroy(trailParticles.gameObject, 1f); 
        }

        AudioSource audioSrc = GetComponentInChildren<AudioSource>();
        if (audioSrc != null)
        {
            audioSrc.transform.SetParent(null); 
            float delay = audioSrc.clip != null ? audioSrc.clip.length : 2f;
            Destroy(audioSrc.gameObject, delay);
        }

        Destroy(gameObject);
    }
}