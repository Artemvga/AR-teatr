// Assets/Scripts/AR/ImageLoader.cs
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ImageLoader : MonoBehaviour
{
    public RawImage[] arMarkers = new RawImage[7];

    void Start()
    {
        DatasetManager.LoadActiveDataset(dataset => {
            if (dataset == null) return;
            for (int i = 0; i < 7; i++)
                StartCoroutine(Load(dataset.marks[i], arMarkers[i]));
        });
    }

    IEnumerator Load(string url, RawImage target)
    {
        using (var req = UnityWebRequestTexture.GetTexture(CloudAPI.BaseUrl + url))
        {
            yield return req.SendWebRequest();
            if (req.result == UnityWebRequest.Result.Success)
                target.texture = ((DownloadHandlerTexture)req.downloadHandler).texture;
        }
    }
}