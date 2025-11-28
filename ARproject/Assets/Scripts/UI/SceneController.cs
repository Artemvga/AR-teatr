// Assets/Scripts/UI/SceneController.cs
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneController : MonoBehaviour
{
    [SerializeField] private GameObject uiPanel;
    [SerializeField] private GameObject loadingPanel;
    [SerializeField] private Slider slider;

    public void SceneLoad(int sceneIndex)
    {
        uiPanel.SetActive(false);
        loadingPanel.SetActive(true);
        StartCoroutine(LoadAsync(sceneIndex));
    }

    private IEnumerator LoadAsync(int sceneIndex)
    {
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneIndex);
        while (!op.isDone)
        {
            float progress = Mathf.Clamp01(op.progress / 0.9f);
            slider.value = progress;
            yield return null;
        }
    }
}