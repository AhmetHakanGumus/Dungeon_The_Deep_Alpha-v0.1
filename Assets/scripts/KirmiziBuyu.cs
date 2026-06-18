using System.Collections.Generic;
using UnityEngine;

public class KirmiziBuyu : MonoBehaviour
{
    [Header("Alan Hasarı Ayarları")]
    public float patlamaYaricapi = 2.5f; 
    public int hasarMiktari = 50;
    
    [Header("Görsel ve Ses Efektleri")]
    public GameObject patlamaEfekti; 
    public GameObject alanGostergePrefabi; 
    public AudioClip patlamaSesi; // YENİ: Patlama sesi için yuva

    private bool patladiMi = false; 

    private void OnTriggerEnter2D(Collider2D hitInfo)
    {
        if (hitInfo.CompareTag("Player") || patladiMi) return;
        
        patladiMi = true; 
        PatlaVeHasarVer();
    }

    void PatlaVeHasarVer()
    {
        Collider2D[] alandakiObjeler = Physics2D.OverlapCircleAll(transform.position, patlamaYaricapi);
        List<EnemyHealth> hasarAlanlar = new List<EnemyHealth>();

        foreach (Collider2D obje in alandakiObjeler)
        {
            if (obje.CompareTag("Enemy"))
            {
                EnemyHealth dusmanCan = obje.GetComponent<EnemyHealth>();
                
                if (dusmanCan != null && !hasarAlanlar.Contains(dusmanCan))
                {
                    dusmanCan.TakeDamage(hasarMiktari);
                    hasarAlanlar.Add(dusmanCan); 
                }
            }
        }

        // YENİ: Patlama sesini objeden bağımsız, serbest olarak oynat (Asla kesilmez!)
        if (patlamaSesi != null)
        {
            GameObject geciciSes = new GameObject("PatlamaSesi_MesafeAyarlı");
            geciciSes.transform.parent = Camera.main.transform;
            geciciSes.transform.localPosition = Vector3.zero;

            AudioSource kaynak = geciciSes.AddComponent<AudioSource>();
            kaynak.clip = patlamaSesi;
            kaynak.spatialBlend = 0f; // Ses hala 2D (Kulaklarda sağa/sola kaymaz)

            // --- İŞTE MESAFE SİHRİ BURADA ---
            // Kamera (biz) ile patlamanın olduğu yer arasındaki mesafeyi ölçüyoruz
            float mesafe = Vector2.Distance(Camera.main.transform.position, transform.position);
            
            // Sesin duyulabileceği maksimum uzaklık (İstersen bu sayıyı kendine göre değiştirebilirsin)
            float maxDuyulmaMesafesi = 15f; 
            
            // Mesafeye göre sesin gücünü hesapla (Uzaklaştıkça sıfıra yaklaşır)
            float hesaplananSesGucu = 1f - (mesafe / maxDuyulmaMesafesi);
            
            // Sesi uygula (Clamp01 kodu, sesin 0'ın altına inmesini veya 1'in üstüne çıkmasını engeller)
            kaynak.volume = Mathf.Clamp01(hesaplananSesGucu); 
            // --------------------------------

            kaynak.Play();
            Destroy(geciciSes, patlamaSesi.length);
        }
        if (patlamaEfekti != null) Instantiate(patlamaEfekti, transform.position, Quaternion.identity);
        
        if (alanGostergePrefabi != null)
        {
            GameObject gosterge = Instantiate(alanGostergePrefabi, transform.position, Quaternion.identity);
            float boyutFactoru = patlamaYaricapi * 2f;
            gosterge.transform.localScale = new Vector3(boyutFactoru, boyutFactoru, 1f);
        }

        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, patlamaYaricapi);
    }
}