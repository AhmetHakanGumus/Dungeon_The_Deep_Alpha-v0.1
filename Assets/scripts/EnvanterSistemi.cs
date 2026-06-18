using UnityEngine;
using UnityEngine.InputSystem;

public class EnvanterSistemi : MonoBehaviour
{
    [Header("Arayüz Objeleri")]
    [Tooltip("Açılıp kapanacak olan ana envanter çerçevesini buraya sürükle")]
    public GameObject envanterPaneli; 

    private bool envanterAcikMi = false;

    void Start()
    {
        // Oyun başladığında envanter paneli gizli olsun
        if (envanterPaneli != null)
        {
            envanterPaneli.SetActive(false);
            envanterAcikMi = false;
        }
    }

    void Update()
    {
        // TAB tuşuna basıldığını algıla
        if (Keyboard.current.tabKey.wasPressedThisFrame)
        {
            EnvanteriAcKapat();
        }
    }

    void EnvanteriAcKapat()
    {
        // Durumu tersine çevir (Açıksa kapat, kapalıysa aç)
        envanterAcikMi = !envanterAcikMi;
        
        // Panelin görünürlüğünü yeni duruma göre ayarla
        if (envanterPaneli != null)
        {
            envanterPaneli.SetActive(envanterAcikMi);
        }

        // OPSİYONEL: Envanter açıkken oyunun (zamanın) durmasını istersen alttaki satırın başındaki "//" işaretlerini sil
        // Time.timeScale = envanterAcikMi ? 0f : 1f; 
    }
}