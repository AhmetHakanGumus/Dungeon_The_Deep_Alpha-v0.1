using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class SandikSistemi : MonoBehaviour
{
    [Header("Ayarlar")]
    public float basiliTutmaSuresi = 1.0f; 
    private float sayac = 0f;
    private bool oyuncuYakininda = false;
    private bool acildiMi = false;

    [Header("Gerekli Objeler")]
    public Animator animator;
    public GameObject sandikEnvanterPaneli;
    public GameObject oyuncuEnvanterPaneli;

    [Header("Rasgele İtem Sistemi")]
    public ItemData[] itemHavuzu; 
    public GameObject[] ahsapCerceveler; 

    void Start()
    {
        foreach (GameObject cerceve in ahsapCerceveler)
        {
            Image ikonYuvasi = cerceve.transform.GetChild(0).GetComponent<Image>();

            if (Random.value > 0.4f) 
            {
                ItemData rasgeleItem = itemHavuzu[Random.Range(0, itemHavuzu.Length)];
                ikonYuvasi.sprite = rasgeleItem.itemIkonu;
                ikonYuvasi.color = Color.white; 
            }
            else
            {
                ikonYuvasi.sprite = null;
                ikonYuvasi.color = new Color(1, 1, 1, 0); 
            }
        }
    }

    void Update()
    {
        if (!oyuncuYakininda || acildiMi) return;

        if (Keyboard.current.eKey.isPressed)
        {
            sayac += Time.deltaTime;
            if (sayac >= basiliTutmaSuresi) SandigiAc();
        }
        else sayac = 0f;
    }

    void SandigiAc()
    {
        acildiMi = true;
        if (animator != null) animator.SetBool("Open", true);
        if (sandikEnvanterPaneli != null) sandikEnvanterPaneli.SetActive(true);
        if (oyuncuEnvanterPaneli != null) oyuncuEnvanterPaneli.SetActive(true);
    }

    private void OnTriggerEnter2D(Collider2D other) { if (other.CompareTag("Player")) oyuncuYakininda = true; }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            oyuncuYakininda = false;
            sayac = 0f;
            if (sandikEnvanterPaneli != null) sandikEnvanterPaneli.SetActive(false);
        }
    }
}