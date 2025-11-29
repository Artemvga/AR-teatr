// ObjectInfoPopup.cs — вешается один раз на UIManager
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class ObjectInfoPopup : MonoBehaviour
{
    [Header("UI Elements")]
    public RectTransform infoPanel;
    public TMP_Text titleLabel;      // ← ЗАГОЛОВОК
    public TMP_Text descriptionText; // ← ОПИСАНИЕ
    public Image infoImage;
    public CanvasGroup canvasGroup;

    [Header("Animation")]
    public float appearDuration = 0.6f;
    public float disappearDuration = 0.3f;
    public float startOffsetY = 600f;

    private Vector2 targetPanelPosition;

    void Start()
    {
        if (infoPanel == null) return;

        targetPanelPosition = infoPanel.anchoredPosition;
        ResetPanel();

        // Найти и назначить кнопку "Выйти"
        var buttons = infoPanel.GetComponentsInChildren<Button>(true);
        foreach (var btn in buttons)
        {
            if (btn.name == "CloseButton" || btn.CompareTag("CloseButton"))
            {
                btn.onClick.AddListener(HidePanel);
                break;
            }
        }
    }

    // ← НОВАЯ ПЕРЕГРУЗКА: принимает заголовок, описание и изображение
    public void ShowPanel(string title, string description, Sprite sprite)
    {
        // Обновляем заголовок
        if (titleLabel != null)
            titleLabel.text = title;

        // Обновляем описание
        if (descriptionText != null)
            descriptionText.text = description;

        // Обновляем изображение
        if (infoImage != null)
        {
            if (sprite != null)
            {
                infoImage.sprite = sprite;
                infoImage.gameObject.SetActive(true);
            }
            else
            {
                infoImage.gameObject.SetActive(false);
            }
        }

        // Готовим UI к показу
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.blocksRaycasts = true;
            canvasGroup.interactable = true;
        }

        infoPanel.gameObject.SetActive(true);
        infoPanel.anchoredPosition = new Vector2(targetPanelPosition.x, targetPanelPosition.y + startOffsetY);

        // Анимация появления с пружинкой
        infoPanel.DOAnchorPos(targetPanelPosition, appearDuration)
                 .SetEase(Ease.OutElastic);

        if (canvasGroup != null)
            canvasGroup.DOFade(1f, appearDuration);
    }

    public void HidePanel()
    {
        if (canvasGroup != null)
        {
            canvasGroup.blocksRaycasts = false;
            canvasGroup.interactable = false;
        }

        infoPanel.DOAnchorPos(
            new Vector2(targetPanelPosition.x, targetPanelPosition.y + startOffsetY),
            disappearDuration
        ).SetEase(Ease.InSine).OnComplete(() =>
        {
            infoPanel.gameObject.SetActive(false);
        });

        if (canvasGroup != null)
            canvasGroup.DOFade(0f, disappearDuration);
    }

    void ResetPanel()
    {
        if (infoPanel == null) return;
        infoPanel.anchoredPosition = new Vector2(targetPanelPosition.x, targetPanelPosition.y + startOffsetY);
        if (canvasGroup != null) canvasGroup.alpha = 0f;
        infoPanel.gameObject.SetActive(false);
    }
}