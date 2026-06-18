using UnityEngine;
using UnityEngine.UI;

public class PlayerMana : MonoBehaviour
{
    [Header("Mana Ayarları")]
    public float maxMana = 100f;
    public float currentMana;
    
    [Tooltip("Saniyede ne kadar mana otomatik dolsun?")]
    public float saniyeBasinaYenilenme = 5f; 

    [Header("Arayüz (UI)")]
    public Slider manaBari;

    void Start()
    {
        currentMana = maxMana;
        
        if (manaBari != null)
        {
            manaBari.maxValue = maxMana;
            manaBari.value = currentMana;
        }
    }

    void Update()
    {
        // Mana ful değilse zamanla otomatik doldur
        if (currentMana < maxMana)
        {
            currentMana += saniyeBasinaYenilenme * Time.deltaTime;
            currentMana = Mathf.Clamp(currentMana, 0, maxMana); // Sınırı aşmasın
            
            if (manaBari != null) manaBari.value = currentMana;
        }
    }

    // Büyü kodları bunu çağırıp mana harcayacak
    public bool HarcamaYap(float miktar)
    {
        if (currentMana >= miktar)
        {
            currentMana -= miktar;
            if (manaBari != null) manaBari.value = currentMana;
            return true; // Mana yetti ve harcandı!
        }
        
        return false; // Mana yetmedi!
    }
}