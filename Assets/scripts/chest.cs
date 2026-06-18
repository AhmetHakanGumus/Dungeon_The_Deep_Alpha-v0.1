using UnityEngine;

public class Chest : MonoBehaviour
{
    [Header("Sandık Ayarları")]
    public int sandikBedeli = 50; // Sandığı açmak kaç altın?
    public bool acildiMi = false;

    private bool oyuncuYakindaMi = false;
    private PlayerWallet oyuncuCuzdani;

    void Update()
    {
        // Sadece oyuncu yanındaysa ve 'E' tuşuna basarsa:
        if (oyuncuYakindaMi && Input.GetKeyDown(KeyCode.E))
        {
            SandigiAcmayiDene();
        }
    }

   void SandigiAcmayiDene()
    {
        if (oyuncuCuzdani != null)
        {
            if (oyuncuCuzdani.ParaHarca(sandikBedeli))
            {
                Debug.Log("Sandık açıldı! Paralar harcandı.");
                
                UpgradeManager.Instance.OpenUpgradePanel();
                
                // YENİ: Sandığın bedelini %50 artır ve tam sayıya yuvarla!
                sandikBedeli = Mathf.RoundToInt(sandikBedeli * 1.5f);
            }
            else
            {
                Debug.Log("Yeterli altın yok! Gereken: " + sandikBedeli);
            }
        }
    }
    // Oyuncu sandığın yanına geldiğinde (Tetikleyici alanına girdiğinde)
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            oyuncuYakindaMi = true;
            oyuncuCuzdani = collision.GetComponent<PlayerWallet>();
            Debug.Log("Sandığın yanındasın. Açmak için 'E' tuşuna bas.");
        }
    }

    // Oyuncu sandığın yanından uzaklaştığında
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            oyuncuYakindaMi = false;
            oyuncuCuzdani = null; // Cüzdan bağlantısını kopar ki uzaktan açamasın
        }
    }
}