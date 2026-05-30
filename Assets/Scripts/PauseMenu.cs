using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    [Header("UI паузы")]
    public GameObject pausePanel;
    
    [Header("Кнопки")]
    public Button resumeButton;
    public Button quitButton;
    public Button girlButton;
    public Button boyButton;
    
    [Header("Ссылки")]
    public Character character;
    
    [Header("Чат NPC (перетащи сюда ChatPanel)")]
    public GameObject chatPanel; // ← ДОБАВЬ ЭТО
    
    private bool isPaused = false;
    
    void Start()
    {
        if (pausePanel != null)
            pausePanel.SetActive(false);
        
        if (resumeButton != null)
            resumeButton.onClick.AddListener(ResumeGame);
        
        if (quitButton != null)
            quitButton.onClick.AddListener(QuitGame);
        
        if (girlButton != null && character != null)
            girlButton.onClick.AddListener(() => ChangeCharacter(0));
        
        if (boyButton != null && character != null)
            boyButton.onClick.AddListener(() => ChangeCharacter(1));
    }
    
    // Метод для Input System
    public void OnPause(InputValue value)
    {
        if (!value.isPressed) return;
        
        // ✅ НОВАЯ ПРОВЕРКА: если чат открыт — НЕ открываем паузу
        if (chatPanel != null && chatPanel.activeSelf)
        {
            Debug.Log("Чат открыт, пауза не включается");
            return;
        }
        
        if (isPaused)
            ResumeGame();
        else
            PauseGame();
    }
    
    void PauseGame()
    {
        if (pausePanel != null)
        {
            pausePanel.SetActive(true);
            Time.timeScale = 0f;
            isPaused = true;
            Debug.Log("Игра на паузе");
        }
    }
    
    void ResumeGame()
    {
        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
            Time.timeScale = 1f;
            isPaused = false;
            Debug.Log("Игра продолжена");
        }
    }
    
    void ChangeCharacter(int index)
    {
        if (character != null)
        {
            character.ChangeCharacter(index);
            ResumeGame();
        }
    }
    
    void QuitGame()
    {
        Debug.Log("Выход из игры");
        Time.timeScale = 1f;
        
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
}