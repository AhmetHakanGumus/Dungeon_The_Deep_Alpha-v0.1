using UnityEngine;
using UnityEngine.SceneManagement; // Sahneleri yeniden yüklemek için şart!

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance;

    [Header("Menü Panelleri")]
    public GameObject mainMenuPanel;
    public GameObject pauseMenuPanel;
    public GameObject gameOverPanel;

    private bool isPaused = false;
    private bool isDead = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    void Start()
    {
        // Oyun başladığında direkt Ana Menü açılsın ve zaman dursun
        mainMenuPanel.SetActive(true);
        pauseMenuPanel.SetActive(false);
        gameOverPanel.SetActive(false);
        Time.timeScale = 0f; 
    }

    void Update()
    {
        // Eğer ölmedik ve Ana Menüde değilsek ESC tuşuyla oyunu durdur/devam ettir
        if (!isDead && !mainMenuPanel.activeSelf && Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    // --- BUTON FONKSİYONLARI ---

    public void StartGame()
    {
        mainMenuPanel.SetActive(false);
        Time.timeScale = 1f; // Oyunu başlat
    }

    public void PauseGame()
    {
        pauseMenuPanel.SetActive(true);
        Time.timeScale = 0f; // Oyunu dondur
        isPaused = true;
    }

    public void ResumeGame()
    {
        pauseMenuPanel.SetActive(false);
        Time.timeScale = 1f; // Oyunu devam ettir
        isPaused = false;
    }

    public void GameOver()
    {
        isDead = true;
        gameOverPanel.SetActive(true);
        Time.timeScale = 0f; // Ölünce oyunu dondur
    }

    public void RestartGame()
    {
        // Mevcut sahneyi en baştan yükler (Her şeyi sıfırlar)
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitGame()
    {
        Debug.Log("Oyundan Çıkılıyor...");
        Application.Quit(); // Not: Unity Editöründe çalışmaz, sadece oyun Build alınınca çalışır.
    }
}