using UnityEngine;

public class ZamanliYokOlma : MonoBehaviour
{
    void Start()
    {
        // Obje sahneye geldiği an saniye sayar ve 0.2 saniye sonra kendini siler.
        Destroy(gameObject, 0.2f); 
    }
}