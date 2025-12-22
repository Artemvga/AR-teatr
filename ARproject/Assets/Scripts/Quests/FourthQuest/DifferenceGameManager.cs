// DifferenceGameManager.cs
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class DifferenceGameManager : MonoBehaviour
{
    [Header("Отличия")]
    public DifferenceItem[] differenceItems; // Задаёшь в инспекторе

    [Header("Win Panel")]
    public RectTransform winPanel; // Ссылка на панель (должна быть внутри Canvas!)
    public Button exitButton;

    private int _foundCount = 0;
    private int _totalDifferences = 2;

    void Start()
    {
        if (winPanel == null)
        {
            Debug.LogError("WinPanel не назначен!");
            return;
        }

        // Скроем панель и поставим её ВЫШЕ экрана
        winPanel.anchoredPosition = new Vector2(0, 2400f); // за пределами экрана (сверху)
        winPanel.gameObject.SetActive(true);

        DifferenceItem.OnDifferenceClicked += OnItemFound;
        exitButton.onClick.AddListener(LoadMenuScene);
    }

    void OnItemFound()
    {
        _foundCount++;
        Debug.Log($"Найдено: {_foundCount} / {_totalDifferences}");

        if (_foundCount >= _totalDifferences)
        {
            ShowWinPanel();
        }
    }

    void ShowWinPanel()
    {
        // Анимация: панель плавно "падает" сверху в центр
        winPanel.DOAnchorPos(new Vector2(0, 0), 0.8f)
                .SetEase(Ease.OutBack);
    }

    void LoadMenuScene()
    {
        SceneController controller = FindObjectOfType<SceneController>();
        if (controller != null)
        {
            controller.SceneLoad(1); // или нужный индекс сцены меню
        }
        else
        {
            Debug.LogError("SceneController не найден!");
        }
    }

    void OnDestroy()
    {
        DifferenceItem.OnDifferenceClicked -= OnItemFound;
        exitButton.onClick.RemoveListener(LoadMenuScene);
    }
}