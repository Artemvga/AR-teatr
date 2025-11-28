// Assets/Scripts/UI/GameManager.cs
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Button buttonStart;
    public Button buttonMarks;
    public Button buttonExit;

    public SceneController sceneController;
    public ExitController exitController;

    void Start()
    {
        if (buttonStart == null) buttonStart = FindButton("Button_Start");
        if (buttonMarks == null) buttonMarks = FindButton("Button_Marks");
        if (buttonExit == null) buttonExit = FindButton("Button_Exit");

        if (buttonStart != null) buttonStart.onClick.AddListener(OnStartGame);
        if (buttonMarks != null) buttonMarks.onClick.AddListener(OnOpenDatasetViewer);
        if (buttonExit != null) buttonExit.onClick.AddListener(exitController.Exit);
    }

    Button FindButton(string name)
    {
        GameObject go = GameObject.Find(name);
        return go != null ? go.GetComponent<Button>() : null;
    }

    void OnStartGame()
    {
        if (!DatasetManager.HasActiveDataset)
            DatasetManager.ActiveDatasetName = "-1";
        sceneController.SceneLoad(2);
    }

    void OnOpenDatasetViewer()
    {
        sceneController.SceneLoad(1);
    }
}