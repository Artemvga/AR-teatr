using UnityEngine;

/// <summary>
/// Повесьте этот скрипт на любой 3D-объект, чтобы при нажатии показывалась информационная панель.
/// Обязательно: у объекта должен быть Collider, а на AR Camera — Physics Raycaster.
/// </summary>
public class InfoTrigger : MonoBehaviour
{
    [Header("Информация для отображения")]
    public string title = "Заголовок";
    [Multiline] public string description = "Описание объекта...";
    public Sprite image;

    private ObjectInfoPopup popupManager;

    void Start()
    {
        // Ищем менеджер UI-панели на сцене
        popupManager = FindObjectOfType<ObjectInfoPopup>();

        if (popupManager == null)
        {
            Debug.LogWarning("ObjectInfoPopup не найден на сцене. Убедитесь, что он добавлен (например, на объекте UIManager).");
        }
    }

    // Вызывается при нажатии мышкой или тапе (если настроена камера правильно)
    void OnMouseDown()
    {
        popupManager?.ShowPanel(title, description, image);
    }
}