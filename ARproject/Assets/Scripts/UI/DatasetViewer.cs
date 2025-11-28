// DatasetViewer.cs
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class DatasetViewer : MonoBehaviour
{
    [Header("UI Elements")]
    public TMP_Dropdown datasetDropdown;
    public RawImage[] previewImages = new RawImage[9];
    public Button useDatasetButton;
    public TMP_Text statusText;

    [Header("Dependencies")]
    public SceneController sceneController;
    public DatasetNavigator datasetNavigator; // Для возврата к выбору

    private string[] datasetIds;

    void Start()
    {
        datasetDropdown.onValueChanged.AddListener(OnDatasetSelected);
        useDatasetButton.onClick.AddListener(OnUseDataset);
        useDatasetButton.gameObject.SetActive(false);

        LoadDatasetList(); // Загрузка при старте
    }

    // 👇 НОВЫЙ МЕТОД — для вызова из DatasetCreator
    public void LoadList()
    {
        LoadDatasetList();
    }

    void LoadDatasetList()
    {
        CloudAPI.GetJsonList("/datasets", (ids) =>
        {
            datasetIds = ids ?? new string[0];
            datasetDropdown.ClearOptions();
            var options = new System.Collections.Generic.List<TMP_Dropdown.OptionData>
        {
            // ✅ Короткий текст, который точно поместится
            new TMP_Dropdown.OptionData("— Датасет —")
        };
            foreach (string id in datasetIds)
                options.Add(new TMP_Dropdown.OptionData(id));
            datasetDropdown.AddOptions(options);
            datasetDropdown.value = 0;
            datasetDropdown.RefreshShownValue();
        });
    }

    void OnDatasetSelected(int index)
    {
        if (index == 0) { ClearPreviews(); return; }

        string datasetId = datasetIds[index - 1];
        string jsonUrl = $"{CloudAPI.BaseUrl}/datasets/{datasetId}/dataset.json";

        CloudAPI.GetJson<DatasetInfo>(jsonUrl, (info) =>
        {
            if (info?.marks?.Length == 9)
            {
                for (int i = 0; i < 9; i++)
                    StartCoroutine(LoadPreview(info.marks[i], previewImages[i]));
                useDatasetButton.gameObject.SetActive(true);
                statusText.text = $"✅ {info.name} (v{info.version})";
            }
            else
            {
                ClearPreviews();
                useDatasetButton.gameObject.SetActive(false);
                statusText.text = "❌ Ошибка: требуется 9 меток";
            }
        });
    }

    IEnumerator LoadPreview(string relativeUrl, RawImage target)
    {
        string fullUrl = CloudAPI.BaseUrl + relativeUrl;
        using (UnityWebRequest req = UnityWebRequestTexture.GetTexture(fullUrl))
        {
            yield return req.SendWebRequest();
            if (req.result == UnityWebRequest.Result.Success)
                target.texture = ((DownloadHandlerTexture)req.downloadHandler).texture;
        }
    }

    void ClearPreviews()
    {
        foreach (RawImage img in previewImages)
            img.texture = null;
    }

    void OnUseDataset()
    {
        string selectedName = datasetDropdown.options[datasetDropdown.value].text;
        DatasetManager.ActiveDatasetName = selectedName;
        statusText.text = $"✅ Активен: {selectedName}";
    }

    public void OnBackToChooser()
    {
        datasetNavigator?.ShowChooser();
    }
}