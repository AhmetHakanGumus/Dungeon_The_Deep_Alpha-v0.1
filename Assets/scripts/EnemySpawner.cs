using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Doğma (Spawn) Ayarları")]
    public GameObject iskeletPrefab;
    
    // YENİ EKLENENLER: Alev Ruhu ve İhtimali
    [Tooltip("Alev Ruhu (Ranged Enemy) Prefabı")]
    public GameObject fireSpiritPrefab; 
    [Tooltip("Alev Ruhunun çıkma ihtimali (Yüzde olarak, örn: 15)")]
    public float fireSpiritChance = 15f; 

    [Tooltip("Oyun başındaki doğma aralığı (Saniye)")]
    public float saniyeBasinaSpawn = 2f; 
    [Tooltip("Oyun en fazla ne kadar zorlaşabilir? (Örn: 0.5 saniyede bir doğsun)")]
    public float minSpawnSuresi = 0.5f;  
    [Tooltip("Her saniye geçtikçe doğma süresinden bu kadar kısılır")]
    public float zorlukArtisCarpani = 0.01f; 

    [Header("Mesafe Ayarları (Donut Şekli)")]
    public Transform player;
    public float minUzaklik = 10f; 
    public float maxUzaklik = 15f; 

    [Header("Harita Sınırları (Harita Dışı Doğmamaları İçin)")]
    public float haritaMinX = -20f;
    public float haritaMaxX = 20f;
    public float haritaMinY = -15f;
    public float haritaMaxY = 15f;

    [Header("Duvar İçi Doğmayı Engelleme (İsteğe Bağlı)")]
    [Tooltip("Eğer duvarların varsa Katmanını (Layer) buradan seç ki içine doğmasınlar")]
    public LayerMask engelKatmani; 
    public float iskeletKontrolYaricapi = 0.5f;

    private float zamanlayici = 0f;

    void Update()
    {
        if (player == null) return;

        // 1. ZORLAŞMA SİSTEMİ: Zaman geçtikçe spawn süresini kısalt
        if (saniyeBasinaSpawn > minSpawnSuresi)
        {
            saniyeBasinaSpawn -= zorlukArtisCarpani * Time.deltaTime;
        }

        zamanlayici += Time.deltaTime;

        if (zamanlayici >= saniyeBasinaSpawn)
        {
            SpawnEnemy(); // FONKSİYON İSMİ GÜNCELLENDİ
            zamanlayici = 0f;
        }
    }

    void SpawnEnemy()
    {
        Vector2 spawnNoktasi = Vector2.zero;
        bool gecerliNoktaBulundu = false;
        int denemeSayisi = 0;

        // 2. GÜVENLİ NOKTA BULMA SİSTEMİ: Uygun yer bulana kadar (maks 15 kez) dene
        while (!gecerliNoktaBulundu && denemeSayisi < 15)
        {
            // Rastgele bir yön ve mesafe seç
            Vector2 rastgeleYon = Random.insideUnitCircle.normalized;
            float rastgeleMesafe = Random.Range(minUzaklik, maxUzaklik);
            spawnNoktasi = (Vector2)player.position + (rastgeleYon * rastgeleMesafe);

            // A) Bu nokta haritanın (belirlediğimiz yeşil kutunun) içinde mi?
            bool haritaIcinde = spawnNoktasi.x >= haritaMinX && spawnNoktasi.x <= haritaMaxX &&
                                spawnNoktasi.y >= haritaMinY && spawnNoktasi.y <= haritaMaxY;

            // B) Bu noktada bir duvar/engel var mı?
            bool duvaraCarpiyorMu = Physics2D.OverlapCircle(spawnNoktasi, iskeletKontrolYaricapi, engelKatmani);

            // Eğer hem harita içindeyse hem de duvara çarpmıyorsa, mükemmel nokta!
            if (haritaIcinde && !duvaraCarpiyorMu)
            {
                gecerliNoktaBulundu = true;
            }
            
            denemeSayisi++;
        }

        // Eğer 15 denemeye rağmen bulamadıysa (örneğin oyuncu köşeye çok sıkıştıysa)
        // Noktayı zorla harita sınırlarının içine it (Clamp) ki dışarı düşmesin
        if (!gecerliNoktaBulundu)
        {
            spawnNoktasi.x = Mathf.Clamp(spawnNoktasi.x, haritaMinX, haritaMaxX);
            spawnNoktasi.y = Mathf.Clamp(spawnNoktasi.y, haritaMinY, haritaMaxY);
        }

        // --- YENİ: HANGİ DÜŞMAN DOĞACAK? ---
        float zar = Random.Range(0f, 100f);
        GameObject dogacakDusman;

        // Eğer zar, belirlediğimiz şans oranından küçük/eşitse Alev Ruhu seç
        if (zar <= fireSpiritChance)
        {
            dogacakDusman = fireSpiritPrefab;
        }
        else // Değilse standart İskelet seç
        {
            dogacakDusman = iskeletPrefab;
        }

        // Seçilen düşmanı belirlediğimiz mükemmel noktada yarat
        Instantiate(dogacakDusman, spawnNoktasi, Quaternion.identity);
    }

    // Sahne ekranında sınırları renklendirerek görmemizi sağlar
    private void OnDrawGizmosSelected()
    {
        // Donut (Mesafe) Çizgileri
        if (player != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(player.position, minUzaklik); 
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(player.position, maxUzaklik); 
        }

        // Harita Sınırlarını (Yeşil Kutu) Çiz
        Gizmos.color = Color.green;
        Vector3 center = new Vector3((haritaMinX + haritaMaxX) / 2, (haritaMinY + haritaMaxY) / 2, 0);
        Vector3 size = new Vector3(haritaMaxX - haritaMinX, haritaMaxY - haritaMinY, 0);
        Gizmos.DrawWireCube(center, size);
    }
}