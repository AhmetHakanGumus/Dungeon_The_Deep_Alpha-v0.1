// ============================================================
//  ChargeUI.cs  —  Basit Charge Dolum Çubuðu (UI)
//
//  KURULUM:
//    1) Canvas oluþtur (Screen Space - Overlay)
//    2) Canvas altýna bir Image ekle (arka plan, koyu renk)
//    3) Onun altýna baþka bir Image ekle (dolum çubuðu, renkli)
//    4) Dolum çubuðunun Image Type'ýný "Filled" yap
//       Fill Method: Horizontal, Fill Origin: Left
//    5) Bu scripti Canvas'a veya herhangi bir objeye ekle
//    6) fillImage alanýna dolum çubuðunu sürükle
//    7) chargeSystem alanýna Player'ý sürükle
// ============================================================

using UnityEngine;
using UnityEngine.UI;

public class ChargeUI : MonoBehaviour
{
    [Header("Baðlantýlar")]
    public ChargeSystem chargeSystem;   // Player'daki ChargeSystem
    public Image fillImage;      // Dolum görseli (Image Type: Filled)
    public GameObject barContainer;   // Tüm bar — charge yokken gizlenecek

    [Header("Renkler")]
    public Color chargingColor = Color.yellow;  // Dolum sýrasýnda
    public Color completeColor = Color.cyan;    // Tamamlandýðýnda

    void Update()
    {
        if (chargeSystem == null || fillImage == null) return;

        float progress = chargeSystem.ChargeProgress;
        bool charging = chargeSystem.IsCharging;

        // Charge yoksa barý gizle
        if (barContainer != null)
            barContainer.SetActive(charging);

        if (!charging) return;

        // Dolum miktarýný güncelle (0..1)
        fillImage.fillAmount = progress;

        // Renk: tamamlandýysa mavi, doluyorsa sarý
        fillImage.color = progress >= 1f ? completeColor : chargingColor;
    }
}