using UnityEngine;
using UnityEngine.UI;

public class RadialSpellMenu : MonoBehaviour
{
    public GameObject[] spellPrefabs;
    public Image[] spellIcons;
    
    [Header("Büyü Sistemleri")]
    public ChargeSystem playerChargeSystem;
    public MaviBuyuSistemi maviBuyuSistemi; 
    
    public GameObject menuCanvas;

    Vector2 screenCenter;

    void Start()
    {
        if (menuCanvas != null) menuCanvas.SetActive(false);
        
        if (maviBuyuSistemi != null) maviBuyuSistemi.aktifMi = false;
        if (playerChargeSystem != null) playerChargeSystem.enabled = false;
    }

    void Update()
    {
        // EKLENDİ: Ana menü açıkken (zaman 0 iken) sağ tık menüsünün açılmasını tamamen engeller!
        if (Time.timeScale == 0f) return;

        if (Input.GetMouseButtonDown(1))
        {
            menuCanvas.SetActive(true);
            Time.timeScale = 0.2f;
        }

        if (Input.GetMouseButton(1) && spellPrefabs.Length > 0)
        {
            // EKLENDİ (ÖLÇEKLENDİRME ÇÖZÜMÜ): Ekranın merkezini her karede yeniden hesapla!
            // Böylece oyun çalışırken ekranı tam ekran yapsan bile farenin açısı asla kaymaz.
            screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);

            Vector2 mousePos = Input.mousePosition;
            Vector2 direction = mousePos - screenCenter;

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            if (angle < 0) angle += 360f;

            float sliceAngle = 360f / spellPrefabs.Length;
            int selectedIndex = Mathf.RoundToInt(angle / sliceAngle) % spellPrefabs.Length;

            for (int i = 0; i < spellIcons.Length; i++)
            {
                if (i == selectedIndex)
                    spellIcons[i].color = Color.white;
                else
                    spellIcons[i].color = new Color(1f, 1f, 1f, 0.3f);
            }

            // ŞALTER KONTROL SİSTEMİ
            if (selectedIndex == 0) // Mavi Büyü
            {
                if (maviBuyuSistemi != null) maviBuyuSistemi.aktifMi = true;      
                if (playerChargeSystem != null) playerChargeSystem.enabled = false; 
            }
            else // Hare veya Mor Büyü
            {
                if (maviBuyuSistemi != null) maviBuyuSistemi.aktifMi = false;     
                if (playerChargeSystem != null) 
                {
                    playerChargeSystem.enabled = true;                            
                    playerChargeSystem.spellPrefab = spellPrefabs[selectedIndex]; 
                }
            }
        }

        if (Input.GetMouseButtonUp(1))
        {
            menuCanvas.SetActive(false);
            Time.timeScale = 1f;
        }
    }
}