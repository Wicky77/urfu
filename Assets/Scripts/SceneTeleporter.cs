using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections;

public class SceneTeleporter : MonoBehaviour
{
    [Header("Настройки телепорта")]
    [SerializeField] private Transform teleportDestination;
    [SerializeField] private string destinationTag = "TeleportPoint";
    
    [Header("Настройки затемнения")]
    [SerializeField] private float fadeDuration = 0.5f;
    [SerializeField] private Color fadeColor = Color.black;
    
    [Header("UI")]
    [SerializeField] private Image fadePanel;
    
    [Header("Взаимодействие")]
    [SerializeField] private float interactRange = 2f;
    
    [Header("Подсказка")]
    [SerializeField] private GameObject promptText;
    
    // ✅ ДОБАВЬТЕ ЭТО — ссылка на камеру
    [Header("Камера")]
    [SerializeField] private CameraFollow cameraFollow;
    
    private Transform player;
    private bool isPlayerNear = false;
    private bool isTeleporting = false;
    private Canvas teleportCanvas;
    
    void Start()
    {
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
        
        // ✅ НАЙТИ КАМЕРУ, ЕСЛИ НЕ НАЗНАЧЕНА
        if (cameraFollow == null)
        {
            cameraFollow = FindObjectOfType<CameraFollow>();
        }
        
        if (fadePanel == null)
            CreateFadePanel();
        
        if (promptText != null)
            promptText.SetActive(false);
        
        if (fadePanel != null)
        {
            Color c = fadePanel.color;
            c.a = 0;
            fadePanel.color = c;
            fadePanel.gameObject.SetActive(true);
        }
    }
    
    void CreateFadePanel()
    {
        teleportCanvas = FindObjectOfType<Canvas>();
        if (teleportCanvas == null)
        {
            GameObject canvasObj = new GameObject("TeleportCanvas");
            teleportCanvas = canvasObj.AddComponent<Canvas>();
            teleportCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            teleportCanvas.sortingOrder = 999;
            
            if (FindObjectOfType<UnityEngine.EventSystems.EventSystem>() == null)
            {
                GameObject eventSystem = new GameObject("EventSystem");
                eventSystem.AddComponent<UnityEngine.EventSystems.EventSystem>();
                eventSystem.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
            }
        }
        
        GameObject panelObj = new GameObject("FadePanel");
        panelObj.transform.SetParent(teleportCanvas.transform, false);
        
        RectTransform rect = panelObj.AddComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.sizeDelta = Vector2.zero;
        
        fadePanel = panelObj.AddComponent<Image>();
        fadePanel.color = new Color(fadeColor.r, fadeColor.g, fadeColor.b, 0);
        fadePanel.raycastTarget = true;
    }
    
    void Update()
    {
        if (isTeleporting || player == null) return;
        
        float distance = Vector2.Distance(transform.position, player.position);
        bool wasNear = isPlayerNear;
        isPlayerNear = distance <= interactRange;
        
        if (promptText != null && wasNear != isPlayerNear)
        {
            promptText.SetActive(isPlayerNear);
        }
        
        if (isPlayerNear && Keyboard.current != null && Keyboard.current.fKey.wasPressedThisFrame)
        {
            Teleport();
        }
    }
    
    void Teleport()
    {
        if (teleportDestination == null)
        {
            GameObject dest = GameObject.FindGameObjectWithTag(destinationTag);
            if (dest != null)
                teleportDestination = dest.transform;
            else
            {
                Debug.LogError("Нет точки телепортации!");
                return;
            }
        }
        
        StartCoroutine(TeleportCoroutine());
    }
    
    IEnumerator TeleportCoroutine()
    {
        isTeleporting = true;
        
        // Затемнение
        yield return StartCoroutine(Fade(0, 1, fadeDuration));
        
        // ✅ ТЕЛЕПОРТАЦИЯ ПЕРСОНАЖА
        if (player != null)
        {
            player.position = teleportDestination.position;
            
            Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
            }
        }
        
        // ✅ КЛЮЧЕВОЙ МОМЕНТ: МГНОВЕННОЕ ПЕРЕМЕЩЕНИЕ КАМЕРЫ
        if (cameraFollow != null)
        {
            cameraFollow.SnapToPlayer();
        }
        else
        {
            // Запасной вариант: перемещаем камеру напрямую
            Camera mainCamera = Camera.main;
            if (mainCamera != null && player != null)
            {
                // Примерная позиция — зависит от вашей логики
                mainCamera.transform.position = new Vector3(
                    player.position.x,
                    player.position.y,
                    mainCamera.transform.position.z
                );
            }
        }
        
        // Небольшая задержка для стабильности
        yield return new WaitForSeconds(0.05f);
        
        // Постепенное появление
        yield return StartCoroutine(Fade(1, 0, fadeDuration));
        
        isTeleporting = false;
    }
    
    IEnumerator Fade(float startAlpha, float endAlpha, float duration)
    {
        if (fadePanel == null) yield break;
        
        float elapsed = 0;
        Color color = fadePanel.color;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            color.a = Mathf.Lerp(startAlpha, endAlpha, t);
            fadePanel.color = color;
            yield return null;
        }
        
        color.a = endAlpha;
        fadePanel.color = color;
    }
    
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, interactRange);
    }
}