using UnityEngine;

public class MaviBuyuHareket : MonoBehaviour
{
    [Header("Hareket Ayarları")]
    public float hiz = 8f;              // İleri gitme hızı
    public float dalgaFrekansi = 5f;    // Ne kadar hızlı kıvrılacağı
    public float dalgaGenligi = 1f;     // Yukarı aşağı ne kadar genişleyeceği
    public float yasamSuresi = 3f;      // 3 saniye sonra yok olsun

    private Vector2 baslangicPozisyonu;
    private Vector2 ileriYonu;
    private Vector2 dikYon;
    private float yasadigiSure;

    void Start()
    {
        // Doğduğu anı ve yönleri kaydediyoruz
        baslangicPozisyonu = transform.position;
        ileriYonu = transform.right; // Büyünün baktığı yön
        dikYon = new Vector2(-ileriYonu.y, ileriYonu.x); // İleri yöne dik açı
        
        // RAM dolmasın diye 3 saniye sonra kendini imha et komutu
        Destroy(gameObject, yasamSuresi);
    }

    void Update()
    {
        yasadigiSure += Time.deltaTime;

        // İleriye doğru hareket
        Vector2 duzGidis = ileriYonu * (hiz * yasadigiSure);

        // Sinüs dalgası ile süzülme (kıvrılma) hareketi
        float dalgaMiktari = Mathf.Sin(yasadigiSure * dalgaFrekansi * Mathf.PI) * dalgaGenligi;
        Vector2 suzulmeGidisi = dikYon * dalgaMiktari;

        // Pozisyonu güncelle
        transform.position = baslangicPozisyonu + duzGidis + suzulmeGidisi;
    }
}