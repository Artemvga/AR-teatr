// TMP_DropdownAutoSize.cs
using UnityEngine;
using TMPro;

[RequireComponent(typeof(TMP_Dropdown))]
public class TMP_DropdownAutoSize : MonoBehaviour
{
    [Header("Настройки")]
    public float minFontSize = 10f;
    public float maxFontSize = 24f;
    public float padding = 10f;

    private TMP_Dropdown dropdown;
    private TMP_Text captionText;
    private float originalFontSize;

    void Awake()
    {
        dropdown = GetComponent<TMP_Dropdown>();
        captionText = dropdown.captionText;

        if (captionText == null)
        {
            Debug.LogError("TMP_DropdownAutoSize: CaptionText не найден!");
            return;
        }

        originalFontSize = captionText.fontSize;
        maxFontSize = originalFontSize;

        // Запрещаем перенос строк
        captionText.enableWordWrapping = false;

        dropdown.onValueChanged.AddListener(OnDropdownValueChanged);
        AdjustFontSize();
    }

    void OnDropdownValueChanged(int index)
    {
        AdjustFontSize();
    }

    void AdjustFontSize()
    {
        if (captionText == null || dropdown.options == null || dropdown.value >= dropdown.options.Count)
            return;

        string text = dropdown.options[dropdown.value].text;
        captionText.text = text;

        float fontSize = maxFontSize;
        captionText.fontSize = fontSize;

        var renderedValues = captionText.GetRenderedValues(false);
        float textWidth = renderedValues.x;

        float containerWidth = dropdown.captionImage.rectTransform.rect.width - padding * 2;

        if (textWidth <= containerWidth)
        {
            captionText.fontSize = maxFontSize;
            return;
        }

        float ratio = containerWidth / textWidth;
        float newFontSize = Mathf.Max(minFontSize, fontSize * ratio);
        captionText.fontSize = newFontSize;
    }
}