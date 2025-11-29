using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System.Collections.Generic;
using System.Linq;

public class InteractionManager : MonoBehaviour
{
    [System.Serializable]
    public class ModelButton
    {
        public Button button;
        public GameObject prefab;
        public string headerText;
        public string descriptionText;
        public Sprite imageSprite;
        public string correctZoneTag;
        public bool isPlacedCorrectly = false;
    }

    public List<ModelButton> modelButtons;

    [Header("Info Panel (успех)")]
    public GameObject infoPanel;
    public Image panelImage;
    public TMP_Text panelHeader;
    public TMP_Text panelDescription;
    public Button backButton;

    [Header("Инструкции и пример")]
    public GameObject hintPanel;
    public TMP_Text hintText;
    public GameObject exampleImage;

    private int surfaceLayerMask;
    private GameObject currentPlacedObject = null;
    private ModelButton currentButtonData = null;
    private bool isObjectSelected = false;
    private bool isPlacing = false;

    void Start()
    {
        infoPanel.SetActive(false);
        backButton.onClick.AddListener(HideInfoPanel);

        UpdateHintText(); // Показываем начальную подсказку
        if (exampleImage != null) exampleImage.SetActive(true);

        int layerIndex = LayerMask.NameToLayer("ARSurface");
        surfaceLayerMask = layerIndex == -1 ? Physics.AllLayers : (1 << layerIndex);

        foreach (var mb in modelButtons)
        {
            var localMb = mb;
            mb.button.onClick.AddListener(() => OnModelButtonClicked(localMb));
        }
    }

    void Update()
    {
        if (isObjectSelected && !isPlacing)
        {
            if (Input.touchCount > 0 || (Application.isEditor && Input.GetMouseButtonDown(0)))
            {
                TrySpawnObject();
            }
        }

        if (isPlacing && currentPlacedObject != null)
        {
            Vector2 inputPos = Input.touchCount > 0
                ? Input.GetTouch(0).position
                : Input.mousePosition;

            Ray ray = Camera.main.ScreenPointToRay(inputPos);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, surfaceLayerMask))
            {
                currentPlacedObject.transform.position = hit.point;
            }

            bool isHeld = Input.touchCount > 0 || (Application.isEditor && Input.GetMouseButton(0));
            if (!isHeld)
            {
                EndPlacement();
            }
        }
    }

    void OnModelButtonClicked(ModelButton buttonData)
    {
        if (currentPlacedObject != null)
        {
            Destroy(currentPlacedObject);
            currentPlacedObject = null;
        }

        currentButtonData = buttonData;
        isObjectSelected = true;
        isPlacing = false;
        UpdateHintText();
    }

    void TrySpawnObject()
    {
        Vector2 screenPos = Input.touchCount > 0
            ? Input.GetTouch(0).position
            : Input.mousePosition;

        Ray ray = Camera.main.ScreenPointToRay(screenPos);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, surfaceLayerMask))
        {
            currentPlacedObject = Instantiate(currentButtonData.prefab, hit.point, Quaternion.identity);
            isPlacing = true;
            isObjectSelected = false;
        }
    }

    void EndPlacement()
    {
        if (!isPlacing || currentPlacedObject == null) return;

        isPlacing = false;

        if (IsObjectInCorrectZone(currentPlacedObject, currentButtonData.correctZoneTag))
        {
            currentButtonData.isPlacedCorrectly = true;
            currentButtonData.button.gameObject.SetActive(false);

            ShowInfoPanel(currentButtonData);

            if (modelButtons.All(mb => mb.isPlacedCorrectly))
            {
                // ВСЁ ГОТОВО — удаляем инструкции и пример
                if (hintPanel != null) Destroy(hintPanel);
                if (exampleImage != null) Destroy(exampleImage);
            }
            else
            {
                UpdateHintText();
            }
        }
        else
        {
            Destroy(currentPlacedObject);
            UpdateHintText();
        }

        currentPlacedObject = null;
        currentButtonData = null;
    }

    bool IsObjectInCorrectZone(GameObject obj, string tag)
    {
        Collider[] cols = Physics.OverlapSphere(obj.transform.position, 0.05f);
        foreach (Collider c in cols)
        {
            if (c.isTrigger && c.CompareTag(tag))
                return true;
        }
        return false;
    }

    void UpdateHintText()
    {
        if (hintText == null) return;

        int total = modelButtons.Count;
        int placed = modelButtons.Count(mb => mb.isPlacedCorrectly);
        int remaining = total - placed;

        if (isObjectSelected)
        {
            hintText.text = "Проведите пальцем по экрану, чтобы переместить объект по поверхности\n(Отпустите, чтобы зафиксировать)";
        }
        else if (remaining == total)
        {
            hintText.text = "Нажмите на кнопку, чтобы выбрать первый объект";
        }
        else if (remaining == 1)
        {
            hintText.text = "Остался последний объект!\nВыберите его и разместите в нужной зоне";
        }
        else if (remaining > 1)
        {
            hintText.text = $"Уже размещено: {placed} из {total}\nТеперь выберите следующий объект";
        }
        else
        {
            hintText.text = "Все объекты размещены!";
        }
    }

    void ShowInfoPanel(ModelButton data)
    {
        panelImage.sprite = data.imageSprite;
        panelHeader.text = data.headerText;
        panelDescription.text = data.descriptionText;
        infoPanel.SetActive(true);

        RectTransform rt = infoPanel.GetComponent<RectTransform>();
        rt.anchoredPosition = new Vector2(-Screen.width, rt.anchoredPosition.y);
        rt.DOAnchorPosX(0, 0.5f).SetEase(Ease.OutQuint);
    }

    void HideInfoPanel()
    {
        RectTransform rt = infoPanel.GetComponent<RectTransform>();
        rt.DOAnchorPosX(-Screen.width, 0.4f)
            .SetEase(Ease.InQuint)
            .OnComplete(() => infoPanel.SetActive(false));
    }
}