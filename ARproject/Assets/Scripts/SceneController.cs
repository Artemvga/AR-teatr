using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneController : MonoBehaviour
{
    [SerializeField] private GameObject uI;
    [SerializeField] private GameObject sliderLoad;
    [SerializeField] private Slider slider;

    public void SceneLoad(int index)
    {
        uI.SetActive(false);
        sliderLoad.SetActive(true);
        StartCoroutine(LoadAsynk(index));
    }

    private IEnumerator LoadAsynk(int index)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(index);
        while (!operation.isDone)
        {
            float progres = Mathf.Clamp01(operation.progress / 0.9f);
            slider.value = progres;
            yield return null;
        }
    }
}
