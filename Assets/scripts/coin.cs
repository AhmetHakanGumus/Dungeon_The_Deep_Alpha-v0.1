using UnityEngine;

public class Coin : MonoBehaviour
{
    public int deger = 10; // Bu altın kaç para verecek?
    public AudioClip toplamaSesi; // İsteğe bağlı: Toplanma sesi

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Eğer içinden geçen şeyin etiketi (Tag) "Player" ise:
        if (collision.CompareTag("Player"))
        {
            // Oyuncunun cüzdanını bul
            PlayerWallet cuzdan = collision.GetComponent<PlayerWallet>();
            
            if (cuzdan != null)
            {
                cuzdan.ParaEkle(deger); // Parayı cüzdana ekle
                
                // Eğer ses varsa, tam o noktada çal
                if (toplamaSesi != null)
                {
                    AudioSource.PlayClipAtPoint(toplamaSesi, Camera.main.transform.position, 1f);
                }

                // Altını sahneden sil
                Destroy(gameObject);
            }
        }
    }
}