// Bootstrap.cs
// Инициализирует глобальные настройки при запуске игры.
// Удаляется после инициализации (DontDestroyOnLoad + Destroy).
// Работает на любой сцене, но логичнее всего — на Menu.

using UnityEngine;

[AddComponentMenu("Debug/Bootstrap")]
public class Bootstrap : MonoBehaviour
{
    void Awake()
    {
        // Гарантируем, что инициализация произойдёт только один раз
        if (Application.isPlaying)
        {
            DontDestroyOnLoad(gameObject); // Чтобы не удалился при смене сцены
            Initialize();
            Destroy(gameObject); // Удаляем после инициализации
        }
    }

    void Initialize()
    {
        // 1. Инициализация URL и PIN
        SettingsManager.Initialize();

        // 2. Логируем текущие настройки (только в Editor / Debug-сборке)
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        Debug.Log($"[Bootstrap] Cloud URL: {CloudAPI.BaseUrl}");
        Debug.Log($"[Bootstrap] Database PIN: {SettingsManager.DatabasePin}");
        Debug.Log($"[Bootstrap] Active Dataset: {DatasetManager.ActiveDatasetName}");
#endif

        // 3. Дополнительная отладка: проверим, что Newtonsoft.Json доступен
        try
        {
            var testJson = Newtonsoft.Json.JsonConvert.SerializeObject(new { test = true });
            Debug.Log("[Bootstrap] Newtonsoft.Json работает корректно.");
        }
        catch (System.Exception ex)
        {
            Debug.LogError("[Bootstrap] Ошибка Newtonsoft.Json: " + ex.Message);
        }
    }
}