// ImageLoader.cs
// Загружает 9 изображений из активного датасета и назначает их на Renderer поверх ImageTarget

using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class ImageLoader : MonoBehaviour
{
    [Header("Renderers (на ImageTarget'ах)")]
    public Renderer[] renderers = new Renderer[9]; // 9 Renderer'ов, каждый поверх ImageTarget

    void Start()
    {
        DatasetManager.LoadActiveDataset((dataset) =>
        {
            if (dataset == null)
            {
                Debug.Log("Используется дефолтный набор меток.");
                return;
            }

            for (int i = 0; i < 9; i++)
            {
                StartCoroutine(LoadImage(dataset.marks[i], renderers[i]));
            }
        });
    }

    IEnumerator LoadImage(string relativeUrl, Renderer targetRenderer)
    {
        string fullUrl = CloudAPI.BaseUrl + relativeUrl;
        using (UnityWebRequest req = UnityWebRequestTexture.GetTexture(fullUrl))
        {
            yield return req.SendWebRequest();

            if (req.result == UnityWebRequest.Result.Success)
            {
                Texture2D texture = ((DownloadHandlerTexture)req.downloadHandler).texture;

                // Назначаем текстуру на Renderer
                if (targetRenderer != null && targetRenderer.material != null)
                {
                    targetRenderer.material.mainTexture = texture;
                }
            }
            else
            {
                Debug.LogError($"❌ Ошибка загрузки изображения: {fullUrl}");
            }
        }
    }
}