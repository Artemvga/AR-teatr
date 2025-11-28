// DatasetCreator.cs
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DatasetCreator : MonoBehaviour
{
    [Header("UI Elements")]
    public TMP_InputField datasetNameInput;
    public TMP_InputField pinInput;
    public Button createButton;
    public Button[] imageButtons = new Button[9];
    public RawImage[] previewImages = new RawImage[9];

    [Header("Dependencies")]
    public DatasetNavigator datasetNavigator;
    public DatasetViewer datasetViewer;

    private byte[][] selectedImages = new byte[9][];
    private string[] imageExtensions = new string[9];

    void Start()
    {
        pinInput.characterValidation = TMP_InputField.CharacterValidation.Integer;
        pinInput.characterLimit = 4;

        for (int i = 0; i < 9; i++)
        {
            int index = i;
            imageButtons[i].onClick.AddListener(() => PickImage(index));
        }

        createButton.onClick.AddListener(CreateDataset);
    }

    void PickImage(int index)
    {
        // ✅ NativeGallery теперь работает
        NativeGallery.GetImageFromGallery((path) =>
        {
            if (string.IsNullOrEmpty(path)) return;

            Texture2D tex = NativeGallery.LoadImageAtPath(path, 512);
            if (tex != null)
            {
                previewImages[index].texture = tex;
                selectedImages[index] = File.ReadAllBytes(path);

                string ext = Path.GetExtension(path).TrimStart('.').ToLower();
                imageExtensions[index] = ext == "jpeg" ? "jpg" : ext;
            }
        }, "Выберите изображение", "image/png,image/jpeg");
    }

    void CreateDataset()
    {
        string name = datasetNameInput.text.Trim();
        string pin = pinInput.text.Trim();

        if (string.IsNullOrEmpty(name) || !IsValidDatasetName(name))
        {
            ShowMessage("Имя: латиница/цифры, без '_'");
            return;
        }

        if (pin.Length != 4 || !int.TryParse(pin, out _))
        {
            ShowMessage("PIN: 4 цифры");
            return;
        }

        for (int i = 0; i < 9; i++)
        {
            if (selectedImages[i] == null)
            {
                ShowMessage($"Изображение {i + 1} не загружено");
                return;
            }
        }

        SettingsManager.SetDatabasePin(pin);

        CloudAPI.CreateDataset(name, selectedImages, imageExtensions,
            onSuccess: (datasetId) =>
            {
                ShowMessage($"✅ Датасет создан: {datasetId}");
                datasetViewer?.LoadList(); // ✅ Теперь работает
                datasetNavigator?.ShowChooser(); // ✅ Теперь работает
            },
            onError: (error) =>
            {
                ShowMessage($"❌ Ошибка: {error}");
            });
    }

    bool IsValidDatasetName(string name)
    {
        return System.Text.RegularExpressions.Regex.IsMatch(name, @"^[a-zA-Z0-9]+$");
    }

    void ShowMessage(string msg)
    {
        Debug.Log(msg);
    }
}