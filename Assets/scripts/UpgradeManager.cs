using UnityEngine;
using System.Collections.Generic;
using TMPro; 

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance;

    [System.Serializable]
    public struct UpgradeOption
    {
        public string id;          
        public string yetenekAdi;  
        [TextArea]
        public string aciklama;    
    }

    [Header("Yetenek Havuzu")]
    public List<UpgradeOption> tumYeteneklerHavuzu = new List<UpgradeOption>();

    [Header("Arayüz Elemanları")]
    public GameObject upgradePanel;
    public List<TextMeshProUGUI> butonYazilari = new List<TextMeshProUGUI>();

    [Header("Oyuncu Referansları")]
    public PlayerMana playerMana;
    public ChargeSystem playerCharge;
    public PlayerHealth playerHealth;       
    public PlayerController playerController; 
    public PlayerWallet playerWallet;       

    private List<UpgradeOption> mevcutSecenekler = new List<UpgradeOption>();

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    public void OpenUpgradePanel()
    {
        if (tumYeteneklerHavuzu.Count < 3) return;

        upgradePanel.SetActive(true);
        Time.timeScale = 0f; 

        RastgeleSecenekleriBelirle();
    }

    void RastgeleSecenekleriBelirle()
    {
        mevcutSecenekler.Clear();
        List<UpgradeOption> geciciHavuz = new List<UpgradeOption>();

        // YENİ: Oyuncunun üzerindeki Mavi Büyü sistemine ulaşıyoruz
        MaviBuyuSistemi maviSistem = null;
        if (playerMana != null) maviSistem = playerMana.GetComponent<MaviBuyuSistemi>();

        // YENİ AKILLI HAVUZ: Faz 1'deyken Faz 3 kartını, 3'teyken de diğerlerini engelle
        foreach (var yetenek in tumYeteneklerHavuzu)
        {
            if (yetenek.id == "mavi_faz2")
            {
                // Sadece karakter Faz 1'deyse bu kart havuza eklensin
                if (maviSistem != null && maviSistem.maviBuyuFazi == 1) geciciHavuz.Add(yetenek);
            }
            else if (yetenek.id == "mavi_faz3")
            {
                // Sadece karakter Faz 2'deyse bu kart havuza eklensin
                if (maviSistem != null && maviSistem.maviBuyuFazi == 2) geciciHavuz.Add(yetenek);
            }
            else
            {
                // Diğer tüm yetenekleri normal şekilde havuza ekle
                geciciHavuz.Add(yetenek);
            }
        }

        // Eğer 3'ten az seçenek kaldıysa (kartlar silindikçe olabilir) hata vermesin diye güvenlik
        int cekilecekSayi = Mathf.Min(3, geciciHavuz.Count);

        for (int i = 0; i < cekilecekSayi; i++)
        {
            int rastgeleIndeks = Random.Range(0, geciciHavuz.Count);
            mevcutSecenekler.Add(geciciHavuz[rastgeleIndeks]);
            geciciHavuz.RemoveAt(rastgeleIndeks); 
        }

        ArayuzuGuncelle();
    }

    void ArayuzuGuncelle()
    {
        // Butonları önce temizleyelim
        for (int i = 0; i < butonYazilari.Count; i++) butonYazilari[i].transform.parent.gameObject.SetActive(false);

        for (int i = 0; i < mevcutSecenekler.Count; i++)
        {
            if (i < butonYazilari.Count)
            {
                butonYazilari[i].transform.parent.gameObject.SetActive(true);
                butonYazilari[i].text = $"<b>{mevcutSecenekler[i].yetenekAdi}</b>\n<size=80%>{mevcutSecenekler[i].aciklama}</size>";
            }
        }
    }

    public void OnUpgradeButtonClicked(int butonIndeksi)
    {
        if (butonIndeksi >= mevcutSecenekler.Count) return;

        UpgradeOption secilenYetenek = mevcutSecenekler[butonIndeksi];
        YetenegiUygula(secilenYetenek.id);
        ClosePanel();
    }

    // İŞTE BAHSETTİĞİMİZ YER! Case mantıkları buraya geldi.
    void YetenegiUygula(string yetenekID)
    {
        switch (yetenekID)
        {
            // --- YENİ MAVİ BÜYÜ KARTLARI ---
            case "mavi_faz2":
                if (playerMana != null) {
                    MaviBuyuSistemi maviSistem = playerMana.GetComponent<MaviBuyuSistemi>();
                    if (maviSistem != null) maviSistem.maviBuyuFazi = 2;
                }
                break;
            case "mavi_faz3":
                if (playerMana != null) {
                    MaviBuyuSistemi maviSistem = playerMana.GetComponent<MaviBuyuSistemi>();
                    if (maviSistem != null) maviSistem.maviBuyuFazi = 3;
                }
                break;

            // --- YENİ 7 YETENEK ---
            case "max_can":
                if (playerHealth != null) {
                    playerHealth.maxHealth += 50f;
                    playerHealth.currentHealth += 50f; 
                }
                break;
            case "can_regen":
                if (playerHealth != null) playerHealth.saniyeBasinaCanYenilenme += 2f;
                break;
            case "zirh":
                if (playerHealth != null) playerHealth.zirh += 2;
                break;
            case "hareket_hizi":
                if (playerController != null) playerController.moveSpeed += 1f;
                break;
            case "altin_carpani":
                if (playerWallet != null) playerWallet.altinCarpani += 0.5f;
                break;
            case "mor_guc":
                if (playerCharge != null) {
                    playerCharge.morHasarBonusu += 15;
                    playerCharge.morSarjHiziCarpani *= 0.85f; 
                }
                break;
            case "kirmizi_alan":
                if (playerCharge != null) {
                    playerCharge.kirmiziHasarBonusu += 10;
                    playerCharge.kirmiziYaricapBonusu += 0.5f; 
                }
                break;

            // --- ESKİ 4 YETENEK ---
            case "max_mana":
                if (playerMana != null) {
                    playerMana.maxMana += 25f;
                    playerMana.currentMana = playerMana.maxMana; 
                }
                break;
            case "mana_regen":
                if (playerMana != null) playerMana.saniyeBasinaYenilenme += 2f;
                break;
            case "red_spell_damage":
                if (playerCharge != null) playerCharge.kirmiziHasarBonusu += 10;
                break;
            case "charge_speed":
                if (playerCharge != null) playerCharge.kirmiziSarjHiziCarpani *= 0.8f; 
                break;
        }
    }

    public void ClosePanel()
    {
        upgradePanel.SetActive(false);
        Time.timeScale = 1f; 
    }
}