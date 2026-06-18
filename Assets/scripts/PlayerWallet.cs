using UnityEngine;
using TMPro; // Eğer TextMeshPro kullanıyorsan bu şart. (Normal Text ise UnityEngine.UI yaz)

public class PlayerWallet : MonoBehaviour
{
    [Header("Yetenek Ayarları")]
    public float altinCarpani = 1f; // Başlangıçta x1 (Normal altın) kazanır
    [Header("Para Ayarları")]
    public int mevcutPara = 0;

    [Header("Arayüz")]
    public TextMeshProUGUI paraYazisi; // Normal Text kullanıyorsan: public Text paraYazisi;

    void Start()
    {
        ArayuzuGuncelle();
    }

    // Altın toplandığında bu fonksiyon çalışacak
    public void ParaEkle(int miktar)
    {
        // Gelen parayı altın çarpanı ile çarp ve tam sayıya yuvarla
        int kazanilanPara = Mathf.RoundToInt(miktar * altinCarpani); 
        
        mevcutPara += kazanilanPara;
        ArayuzuGuncelle();
        Debug.Log(kazanilanPara + " Altın toplandı! Toplam: " + mevcutPara);
    }

    // İleride sandıktan özellik alırken bu fonksiyonu kullanacağız
    public bool ParaHarca(int miktar)
    {
        if (mevcutPara >= miktar)
        {
            mevcutPara -= miktar;
            ArayuzuGuncelle();
            return true; // Para yetti ve harcandı
        }
        return false; // Para yetmedi
    }

    void ArayuzuGuncelle()
    {
        if (paraYazisi != null)
        {
            paraYazisi.text = mevcutPara.ToString() + " Altın";
        }
    }
}