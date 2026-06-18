using UnityEngine;
using UnityEngine.InputSystem;

public class MaviBuyuSistemi : MonoBehaviour
{
    [Header("Büyü Durumu")]
    public bool aktifMi = true;
    public GameObject maviBuyuPrefab;
    public Transform atisNoktasi;

    [Header("Mana Ayarları")]
    public float manaBedeli = 5f; // Başlangıç mana bedeli

    [Header("Büyü Fazı")]
    [Range(1, 3)]
    public int maviBuyuFazi = 1;

    [Header("Otomatik Atış Ayarları")]
    [Tooltip("İki atış arasındaki bekleme süresi (Saniye cinsinden)")]
    public float atesEtmeSikligi = 0.15f; 
    private float sonAtisZamani = 0f;

    [Header("Gauss Rastgelelik Ayarları")]
    public float maksimumAciSapmasi = 90f; 
    public Vector2 frekansAraligi = new Vector2(3f, 8f); 
    public Vector2 genlikAraligi = new Vector2(0.5f, 2.5f);

    void Update()
    {
        if (!aktifMi) return;

        if (Mouse.current.leftButton.isPressed)
        {
            if (Time.time >= sonAtisZamani + atesEtmeSikligi)
            {
                BuyuyuAtesle();
            }
        }
    }

    // YENİ: Hangi fazdaysak manayı ona göre 2 veya 4 katına çıkaran fonksiyon
    public float GetGuncelManaMaliyeti()
    {
        if (maviBuyuFazi == 1) return manaBedeli;
        if (maviBuyuFazi == 2) return manaBedeli * 2f; // 2 katı
        if (maviBuyuFazi == 3) return manaBedeli * 4f; // 4 katı
        
        return manaBedeli;
    }

    void BuyuyuAtesle()
    {
        PlayerMana manaSys = GetComponent<PlayerMana>();
        
        // YENİ EKLENDİ: Artık sabit manaBedeli'ni değil, dinamik olan GetGuncelManaMaliyeti'ni harcıyoruz
        if (manaSys != null && !manaSys.HarcamaYap(GetGuncelManaMaliyeti()))
        {
            return; 
        }

        sonAtisZamani = Time.time; 

        int mermiSayisi = 1;
        if (maviBuyuFazi == 2) mermiSayisi = 2;
        else if (maviBuyuFazi == 3) mermiSayisi = 4;

        Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        Vector2 yon = (mouseWorldPos - (Vector2)atisNoktasi.position).normalized;
        float temelAci = Mathf.Atan2(yon.y, yon.x) * Mathf.Rad2Deg;

        for (int i = 0; i < mermiSayisi; i++)
        {
            float sapma = GaussRastgele(-maksimumAciSapmasi, maksimumAciSapmasi);
            float rastgeleAci = temelAci + sapma;
            Quaternion mermiRotasyonu = Quaternion.Euler(0, 0, rastgeleAci);

            GameObject atilanBuyu = Instantiate(maviBuyuPrefab, atisNoktasi.position, mermiRotasyonu);

            MaviBuyuHareket hareketKodu = atilanBuyu.GetComponent<MaviBuyuHareket>();
            if (hareketKodu != null)
            {
                hareketKodu.dalgaFrekansi = GaussRastgele(frekansAraligi.x, frekansAraligi.y);
                hareketKodu.dalgaGenligi = GaussRastgele(genlikAraligi.x, genlikAraligi.y);
            }
        }
    }

    float GaussRastgele(float min, float max)
    {
        float u1 = 1f - Random.value; 
        float u2 = 1f - Random.value;
        float randStdNormal = Mathf.Sqrt(-2f * Mathf.Log(u1)) * Mathf.Sin(2f * Mathf.PI * u2);
        float ortalama = (min + max) / 2f;
        float standartSapma = (max - min) / 6f;
        float sonuc = ortalama + standartSapma * randStdNormal;
        return Mathf.Clamp(sonuc, min, max);
    }
}