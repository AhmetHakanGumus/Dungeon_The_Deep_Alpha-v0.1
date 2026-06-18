using UnityEngine;
using UnityEngine.InputSystem;

public class ChargeSystem : MonoBehaviour
{
    [Header("Büyü Ayarları")]
    public GameObject spellPrefab;
    public float chargeTime = 1.5f;
    public int spellDamage = 25;

    [Header("Mana Ayarları")]
    [Tooltip("Bu büyüyü atmanın temel mana bedeli")]
    public float temelManaBedeli = 25f;

    [Header("Fırlatma")]
    public Transform firePoint;

    [Header("Ses Efektleri")]
    public AudioClip chargeSesi;
    private AudioSource sesKaynagi; 

    [Header("Durum (sadece izleme için)")]
    [SerializeField] float chargeProgress = 0f;  
    [SerializeField] bool isCharging = false;

    public bool IsCharging => isCharging;
    public float ChargeProgress => chargeProgress;

    DirectionController directionController;

    [HideInInspector] public int kirmiziHasarBonusu = 0;
    [HideInInspector] public float kirmiziYaricapBonusu = 0f;
    [HideInInspector] public float kirmiziSarjHiziCarpani = 1f;

    [HideInInspector] public int morHasarBonusu = 0;
    [HideInInspector] public float morSarjHiziCarpani = 1f;

    public float GuncelManaHesapla()
    {
        if (spellPrefab == null) return temelManaBedeli;

        SpellProjectile sp = spellPrefab.GetComponent<SpellProjectile>();
        if (sp != null && sp.isAoE)
        {
            return temelManaBedeli + (kirmiziHasarBonusu * 0.5f) + (kirmiziYaricapBonusu * 5f);
        }
        else
        {
            return temelManaBedeli + (morHasarBonusu * 0.5f);
        }
    }

    void Start()
    {
        directionController = GetComponent<DirectionController>();
        sesKaynagi = gameObject.AddComponent<AudioSource>();
        sesKaynagi.playOnAwake = false; 
    }

    void Update()
    {
        // EKLENDİ: Zaman durmuşsa (ana menü açıksa) sol tıka basılsa bile büyü doldurmayı engelle!
        if (Time.timeScale == 0f) return; 

        HandleChargeInput();
    }

    void HandleChargeInput()
    {
        PlayerMana manaSys = GetComponent<PlayerMana>();
        
        if (manaSys != null && manaSys.currentMana < GuncelManaHesapla())
        {
            if (isCharging) CancelCharge(); 
            return; 
        }

        bool leftMouseHeld = Mouse.current.leftButton.isPressed;

        if (leftMouseHeld)
            StartOrContinueCharge();
        else if (isCharging)
            ReleaseCharge();
    }

    void StartOrContinueCharge()
    {
        if (!isCharging && chargeSesi != null && sesKaynagi != null)
        {
            sesKaynagi.clip = chargeSesi;
            sesKaynagi.loop = true; 
            sesKaynagi.Play();
        }

        isCharging = true;

        float aktifSarjCarpani = 1f;
        if (spellPrefab != null)
        {
            SpellProjectile sp = spellPrefab.GetComponent<SpellProjectile>();
            if (sp != null) aktifSarjCarpani = sp.isAoE ? kirmiziSarjHiziCarpani : morSarjHiziCarpani;
        }

        float realChargeTime = chargeTime * aktifSarjCarpani;
        chargeProgress += Time.deltaTime / realChargeTime;
        chargeProgress = Mathf.Clamp01(chargeProgress);

        AimAtMouse();

        if (chargeProgress >= 1f) FireSpell();
    }

    void ReleaseCharge()
    {
        if (chargeProgress >= 1f) FireSpell();
        else CancelCharge();
    }

    void FireSpell()
    {
        if (!isCharging) return;  

        PlayerMana manaSys = GetComponent<PlayerMana>();
        
        if (manaSys != null && !manaSys.HarcamaYap(GuncelManaHesapla()))
        {
            ResetCharge();
            return; 
        }

        if (sesKaynagi != null) sesKaynagi.Stop();

        if (spellPrefab == null)
        {
            ResetCharge();
            return;
        }

        Vector2 spawnPos = firePoint != null ? (Vector2)firePoint.position : (Vector2)transform.position;
        Vector2 fireDirection = GetFireDirection();

        GameObject spellObj = Instantiate(spellPrefab, spawnPos, Quaternion.identity);
        SpellProjectile spell = spellObj.GetComponent<SpellProjectile>();

        if (spell != null)
        {
            spellObj.tag = gameObject.CompareTag("Player") ? "PlayerSpell" : "EnemySpell";
            
            if (spell.isAoE)
                spell.Launch(fireDirection, spellDamage + kirmiziHasarBonusu, kirmiziYaricapBonusu);
            else
                spell.Launch(fireDirection, spellDamage + morHasarBonusu, 0f);
        }

        PlayerController playerCtrl = GetComponent<PlayerController>();
        if (playerCtrl != null) playerCtrl.ApplyKnockback(fireDirection);

        ResetCharge();
    }

    void CancelCharge()
    {
        if (sesKaynagi != null) sesKaynagi.Stop();
        ResetCharge();
    }

    void ResetCharge()
    {
        isCharging = false;
        chargeProgress = 0f;
    }

    void AimAtMouse()
    {
        if (directionController == null) return;
        Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        directionController.LookAt(mouseWorldPos);
    }

    Vector2 GetFireDirection()
    {
        if (directionController != null) return directionController.FacingDirection;
        return Vector2.right;
    }
}