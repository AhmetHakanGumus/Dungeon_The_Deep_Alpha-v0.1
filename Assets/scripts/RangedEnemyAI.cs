using UnityEngine;

public class RangedEnemyAI : MonoBehaviour
{
    [Header("Hareket Ayarları")]
    public float speed = 2.5f;
    public float attackRange = 7f;   
    public float retreatRange = 3f;  

    [Header("Saldırı Ayarları")]
    public GameObject enemySpellPrefab; 
    public Transform firePoint;         
    public float fireRate = 2f;         
    private float nextFireTime;

    private Transform player;
    private Rigidbody2D rb;
    private Vector2 movement;
    
    // Animator referansını ekledik
    private Animator anim;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody2D>();
        
        // Animator'ı tanımlıyoruz
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        Vector2 direction = (player.position - transform.position).normalized;

        // --- YENİ EKLENEN: Animator'a yönü bildiriyoruz (-1 Sol, 1 Sağ) ---
        anim.SetFloat("dirX", direction.x);

        // --- HAREKET MANTIĞI ---
        if (distanceToPlayer > attackRange)
        {
            movement = direction; // Yaklaş
        }
        else if (distanceToPlayer <= attackRange && distanceToPlayer > retreatRange)
        {
            movement = Vector2.zero; // Dur ve nişan al
        }
        else if (distanceToPlayer <= retreatRange)
        {
            movement = -direction; // Kaç
        }

        // --- ATEŞ ETME MANTIĞI ---
        if (distanceToPlayer <= attackRange && Time.time >= nextFireTime)
        {
            // Büyü fırlatılmadan önce Attack animasyonunu tetikle!
            anim.SetTrigger("Attack");
            
            Shoot(direction);
            nextFireTime = Time.time + fireRate;
        }
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + movement * speed * Time.fixedDeltaTime);
    }

    void Shoot(Vector2 shootDirection)
    {
        // Büyüyü FirePoint konumunda yarat
        GameObject spell = Instantiate(enemySpellPrefab, firePoint.position, Quaternion.identity);
        
        // YENİ EKLENEN: Büyünün üzerindeki koda ulaşıp "Fırlat" emrini veriyoruz!
        SpellProjectile proj = spell.GetComponent<SpellProjectile>();
        if (proj != null)
        {
            int enemyDamage = 15; // Düşmanın vereceği hasarı buradan ayarlayabilirsin
            proj.Launch(shootDirection, enemyDamage);
        }
    }
}