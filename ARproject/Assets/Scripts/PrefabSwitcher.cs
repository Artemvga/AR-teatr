using UnityEngine;
using TMPro;

public class PrefabSwitcherUI : MonoBehaviour
{
    public Transform spawnPoint;       // Точка спавна
    public GameObject[] prefabs;       // Массив префабов
    public string[] names;             // Названия
    public TMP_Text nameLabel;         // TextMeshPro

    private int currentIndex = 0;
    private GameObject currentObject;

    void Start()
    {
        SpawnCurrent();
    }

    public void Next()
    {
        currentIndex++;
        if (currentIndex >= prefabs.Length)
            currentIndex = 0;

        SpawnCurrent();
    }

    public void Previous()
    {
        currentIndex--;
        if (currentIndex < 0)
            currentIndex = prefabs.Length - 1;

        SpawnCurrent();
    }

    void SpawnCurrent()
    {
        // Удаляем предыдущий объект
        if (currentObject != null)
            Destroy(currentObject);

        // Создаем новый объект и делаем дочерним spawnPoint
        currentObject = Instantiate(
            prefabs[currentIndex],
            spawnPoint.position,
            spawnPoint.rotation,
            spawnPoint            // ← вот это делает объект дочерним
        );

        // Устанавливаем название
        if (names != null && names.Length > currentIndex)
            nameLabel.text = names[currentIndex];
        else
            nameLabel.text = "Название не задано";
    }
}
