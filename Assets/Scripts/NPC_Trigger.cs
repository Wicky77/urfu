using UnityEngine;
using UnityEngine.InputSystem;

public class NPC_Trigger : MonoBehaviour
{
    public GameObject chatPanel;
    public GameObject playerCharacter; // Перетащи сюда игрока

    public GameObject interactPrompt;
    
    private bool isPlayerNear = false;
    private Component playerMovementScript; // Будет хранить найденный скрипт движения
    private System.Reflection.FieldInfo jumpField; // Будет хранить переменную jump

    void Start()
    {
        if (chatPanel != null)
            chatPanel.SetActive(false);

        if (interactPrompt != null)
            interactPrompt.SetActive(false);
        
        // Ищем скрипт движения у игрока
        if (playerCharacter != null)
        {
            // Пытаемся найти любой скрипт с переменной canJump или disableJump
            var scripts = playerCharacter.GetComponents<MonoBehaviour>();
            foreach (var script in scripts)
            {
                // Ищем поле canJump
                var field = script.GetType().GetField("canJump");
                if (field != null && field.FieldType == typeof(bool))
                {
                    playerMovementScript = script;
                    jumpField = field;
                    break;
                }
                
                // Ищем поле disableJump
                field = script.GetType().GetField("disableJump");
                if (field != null && field.FieldType == typeof(bool))
                {
                    playerMovementScript = script;
                    jumpField = field;
                    break;
                }
            }
        }
    }

    void Update()
    {
        // Открытие чата по F
        if (isPlayerNear && Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            if (chatPanel != null && !chatPanel.activeSelf)
            {
                chatPanel.SetActive(true);
                DisablePlayerJump(true); // Отключаем прыжок
            }
        }
        
        // Закрытие по ESC
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (chatPanel != null && chatPanel.activeSelf)
            {
                chatPanel.SetActive(false);
                DisablePlayerJump(false); // Включаем прыжок
                if (interactPrompt != null && isPlayerNear)
                    interactPrompt.SetActive(true);
            }
        }
    }

    private void DisablePlayerJump(bool disable)
    {
        if (playerMovementScript != null && jumpField != null)
        {
            jumpField.SetValue(playerMovementScript, !disable);
        }
        else
        {
            if (playerCharacter != null)
            {
                var movementScripts = playerCharacter.GetComponents<MonoBehaviour>();
                foreach (var script in movementScripts)
                {
                    // Отключаем только те скрипты, которые похожи на контроллер движения
                    if (script.GetType().Name.Contains("Player") || 
                        script.GetType().Name.Contains("Movement") || 
                        script.GetType().Name.Contains("Controller"))
                    {
                        script.enabled = !disable;
                        break;
                    }
                }
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = true;
            if (playerCharacter == null)
                playerCharacter = other.gameObject;
            if (interactPrompt != null && !chatPanel.activeSelf)
                interactPrompt.SetActive(true);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (interactPrompt != null)
                interactPrompt.SetActive(false);
                
            isPlayerNear = false;
            if (chatPanel != null && chatPanel.activeSelf)
            {
                chatPanel.SetActive(false);
                DisablePlayerJump(false);
            }
        }
    }
}