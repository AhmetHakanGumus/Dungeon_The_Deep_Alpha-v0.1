// ============================================================
//  FSMBase.cs  —  Tüm FSM'ler için Soyut Temel Sınıf
//
//  AMAÇ:
//    Her düşman tipinin kendi FSM'si olacak (melee, ranged...).
//    Bu sınıf onların hepsinin ortak iskeletidir.
//    "Bir durum değiştiğinde ne yapmalıyım?" sorusunu standartlaştırır.
//
//  KULLANIM:
//    Bu sınıfı direkt kullanmazsın. Sadece miras alırsın:
//    public class EnemyFSM : FSMBase<EnemyState> { ... }
// ============================================================

using UnityEngine;

// T = Durum enum'u (örn: EnemyState, RangedEnemyState...)
// Bu sayede her FSM kendi durum listesini tanımlayabilir.
public abstract class FSMBase<T> : MonoBehaviour where T : System.Enum
{
    // Şu anki durum — sadece bu sınıf içinden değiştirilebilir
    protected T currentState;

    // ── Durum Geçişi ─────────────────────────────────────────
    // Durumu değiştirmenin TEK yolu budur.
    // Otomatik olarak OnExitState ve OnEnterState'i çağırır.
    protected void ChangeState(T newState)
    {
        // Zaten aynı durumdaysa hiçbir şey yapma
        if (currentState.Equals(newState)) return;

        // Mevcut durumdan çıkış
        OnExitState(currentState);

        T previousState = currentState;
        currentState = newState;

        // Yeni duruma giriş
        OnEnterState(newState, previousState);

        // Geliştirme sırasında durum geçişlerini izle
        Debug.Log($"[{gameObject.name}] {previousState} → {newState}");
    }

    // ── Alt Sınıfların Dolduracağı Metodlar ──────────────────

    // Bir duruma GİRİLDİĞİNDE çağrılır
    // newState  = girilen durum
    // fromState = çıkılan durum (nereden geldiğini bilmek için)
    protected virtual void OnEnterState(T newState, T fromState) { }

    // Bir durumdan ÇIKILDIĞINDA çağrılır
    protected virtual void OnExitState(T state) { }

    // Her karede çalışacak mantık — alt sınıf doldurur
    protected abstract void UpdateState();

    // Unity'nin Update'i buradan çağırılır
    // Alt sınıflar kendi Update'lerini yazmak zorunda kalmaz
    protected virtual void Update()
    {
        UpdateState();
    }
}