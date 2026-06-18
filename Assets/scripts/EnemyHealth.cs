using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    [Header("Can Ayarları")]
    public int maxHealth = 50;
    public int currentHealth;

    [Header("Görsel Ayarlar")]
    public Animator anim; 
    public Slider canBari; 

    [Header("Ödül Ayarları")]
    public GameObject coinPrefab; // YENİ EKLENDİ: İskeletin içinden çıkacak altın

    void Start()
    {
        currentHealth = maxHealth;
        
        if(canBari != null)
        {
            canBari.maxValue = maxHealth;
            canBari.value = currentHealth;
            canBari.gameObject.SetActive(false); 
        }
    }

    public void TakeDamage(int amount)
    {
        if (currentHealth <= 0) return;

        currentHealth -= amount;
        
        if(canBari != null)
        {
            canBari.gameObject.SetActive(true); 
            canBari.value = currentHealth;
        }

        Debug.Log($"[{name}] Hasar aldı: {amount} | Kalan can: {currentHealth}");

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
    }

    void Die()
    {
        if (anim != null) anim.SetTrigger("Die");
        
        if (GetComponent<Collider2D>() != null) GetComponent<Collider2D>().enabled = false; 

        if(canBari != null) canBari.gameObject.SetActive(false);

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null) 
        {
            rb.linearVelocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Static; 
        }

        if (GetComponent<EnemyFSM>() != null) 
        {
            GetComponent<EnemyFSM>().enabled = false;
        }

        // YENİ EKLENDİ: Düşman tam ölme animasyonuna girerken altını yere bırakır!
        if (coinPrefab != null)
        {
            Instantiate(coinPrefab, transform.position, Quaternion.identity);
        }

        Destroy(gameObject, 2f); 
    }
}