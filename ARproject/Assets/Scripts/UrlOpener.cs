using UnityEngine;
using TMPro;

public class UrlOpener : MonoBehaviour
{
    public string url;
    public string nameUrl;
    public GameObject textOpen;
    public TextMeshProUGUI textOpenTMP;
    public void OpenUrl(string url)
    {
        textOpen.SetActive(true);
        textOpenTMP.text = nameUrl;
        Application.OpenURL(url);
    }
}