using UnityEngine;

public class KendiSesiniCal : MonoBehaviour
{
    [Header("Atış Sesi")]
    public AudioClip atisSesi;

    void Start()
    {
        // Büyü sahnede doğduğu saniye, sesi direkt kameranın içinde serbest olarak patlat!
        if (atisSesi != null)
        {
            AudioSource.PlayClipAtPoint(atisSesi, Camera.main.transform.position, 1f);
        }
    }
}