// DatasetNavigator.cs
using UnityEngine;
using UnityEngine.UI;

public class DatasetNavigator : MonoBehaviour
{
    [Header("Панели")]
    public GameObject panelChooser;
    public GameObject panelMain;
    public GameObject panelCreate;

    [Header("Кнопки")]
    public Button buttonSelect;
    public Button buttonCreate;
    public Button buttonBackToMenu;
    public Button buttonBackToChooser1;
    public Button buttonBackToChooser2;

    [Header("Зависимости")]
    public SceneController sceneController;

    void Start()
    {
        panelMain.SetActive(false);
        panelCreate.SetActive(false);
        panelChooser.SetActive(true);

        buttonSelect.onClick.AddListener(ShowMain);
        buttonCreate.onClick.AddListener(ShowCreate);
        buttonBackToMenu.onClick.AddListener(OnBackToMenu);
        buttonBackToChooser1.onClick.AddListener(ShowChooser);
        buttonBackToChooser2.onClick.AddListener(ShowChooser);
    }

    void ShowMain()
    {
        panelChooser.SetActive(false);
        panelCreate.SetActive(false);
        panelMain.SetActive(true);
    }

    void ShowCreate()
    {
        panelChooser.SetActive(false);
        panelMain.SetActive(false);
        panelCreate.SetActive(true);
    }

    // ✅ Сделали public!
    public void ShowChooser()
    {
        panelMain.SetActive(false);
        panelCreate.SetActive(false);
        panelChooser.SetActive(true);
    }

    void OnBackToMenu()
    {
        sceneController.SceneLoad(0);
    }
}