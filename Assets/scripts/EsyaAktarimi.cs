using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EsyaAktarimi : MonoBehaviour, IPointerClickHandler
{
    public bool sandikSlotuMu = true;
    public Transform oyuncuEnvanterSlotAlani; 

    private Image itemIkonu;

    void Awake()
    {
        itemIkonu = transform.GetChild(0).GetComponent<Image>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("1- Tıklama algılandı.");
        
        if (itemIkonu.sprite == null) 
        { 
            Debug.Log("2- İPTAL: Tıklanan slotta item yok (Sprite null)."); 
            return; 
        }
        if (!sandikSlotuMu) 
        { 
            Debug.Log("3- İPTAL: Bu obje sandık slotu olarak işaretlenmemiş."); 
            return; 
        }
        if (oyuncuEnvanterSlotAlani == null)
        {
            Debug.Log("4- HATA: Oyuncu Envanter Slot Alanı koda atanmamış.");
            return;
        }

        Debug.Log("5- Tıklama geçerli, oyuncu envanteri taranıyor...");
        
        foreach (Transform oyuncuSlotu in oyuncuEnvanterSlotAlani)
        {
            Image oyuncuIkonu = oyuncuSlotu.GetChild(0).GetComponent<Image>();

            if (oyuncuIkonu.sprite == null)
            {
                oyuncuIkonu.sprite = itemIkonu.sprite;
                oyuncuIkonu.color = Color.white;

                itemIkonu.sprite = null;
                itemIkonu.color = new Color(1, 1, 1, 0);
                
                Debug.Log("6- BAŞARILI: İtem oyuncuya aktarıldı.");
                return; 
            }
        }
        
        Debug.Log("7- İPTAL: Oyuncu envanterinde boş yer yok.");
    }
}