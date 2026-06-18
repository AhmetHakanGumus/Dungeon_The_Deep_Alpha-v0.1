using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [Header("Can Ayarları")]
    public float maxHealth = 100f; // YENİ: Regen küsüratlı dolabilsin diye float yaptık
    public float currentHealth;    // YENİ: Float yapıldı
    
    [Header("Yetenek Ayarları (Upgrade)")]
    public float saniyeBasinaCanYenilenme = 0f; // Saniyede dolacak can miktarı
    public int zirh = 0; // Gelen hasardan düşülecek miktar

    [Header("Arayüz (UI)")]
    public Slider canBari; 

    void Start()
    {
        currentHealth = maxHealth;
        
        if(canBari != null)
        {
            canBari.maxValue = maxHealth;
            canBari.value = currentHealth;
        }
    }

    void Update()
    {
        // YENİ: Eğer canımız eksikse ve regen yeteneğimiz varsa, zamanla doldur
        if (currentHealth < maxHealth && saniyeBasinaCanYenilenme > 0)
        {
            currentHealth += saniyeBasinaCanYenilenme * Time.deltaTime;
            
            // Can maksimumu geçmesin
            if (currentHealth > maxHealth) 
            {
                currentHealth = maxHealth;
            }
            
            // Barı yumuşak bir şekilde doldur
            if(canBari != null) 
            {
                canBari.value = currentHealth;
            }
        }
    }

    // Hasar alma fonksiyonumuz
    public void TakeDamage(int hasarMiktari)
    {
        // YENİ: Zırhı hesaba katarak en az 1 hasar almasını sağla
        int gercekHasar = Mathf.Max(1, hasarMiktari - zirh);
        
        currentHealth -= gercekHasar;
        
        if(canBari != null) 
        {
            canBari.value = currentHealth;
        }

        Debug.Log($"[{gercekHasar}] hasar yedin! Zırhın [{hasarMiktari - gercekHasar}] hasarı engelledi. Kalan Can: {currentHealth}");

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            if(canBari != null) canBari.value = 0;
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Oyuncu ÖLDÜ!");
        // YENİ: MenuManager'daki GameOver fonksiyonunu çağır
        MenuManager.Instance.GameOver();
        
    }
}