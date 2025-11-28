// Assets/Scripts/Core/CloudAPI.cs
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;

public static class CloudAPI
{
    // ⚠️ ЗАМЕНИ ЭТОТ URL НА СВОЙ!
    public static string BaseUrl = "https://Gfarew.pythonanywhere.com";

    public static void GetJsonList(string path, Action<string[]> callback)
    {
        string url = BaseUrl + path;
        UnityWebRequest req = UnityWebRequest.Get(url);
        ApiHelper.StartRequest(req, (text) =>
        {
            string[] result = JsonConvert.DeserializeObject<string[]>(text);
            callback?.Invoke(result);
        });
    }

    public static void GetJson<T>(string url, Action<T> callback)
    {
        UnityWebRequest req = UnityWebRequest.Get(url);
        ApiHelper.StartRequest(req, (text) =>
        {
            T result = JsonConvert.DeserializeObject<T>(text);
            callback?.Invoke(result);
        });
    }

    // ✅ ГАРАНТИРУЕТ ЗАГРУЗКУ РОВНО 9 ИЗОБРАЖЕНИЙ
    public static void CreateDataset(
        string name,
        byte[][] images,
        string[] extensions,
        Action<string> onSuccess,
        Action<string> onError)
    {
        // 🔥 ВАЖНО: именно 9, не 8!
        if (images?.Length != 9 || extensions?.Length != 9)
        {
            onError?.Invoke("Expected 9 images and 9 extensions.");
            return;
        }

        WWWForm form = new WWWForm();
        form.AddField("name", name);

        // 🔥 Цикл по 9 изображениям
        for (int i = 0; i < 9; i++)
        {
            if (images[i] == null)
            {
                onError?.Invoke($"Image {i} is missing.");
                return;
            }
            string ext = extensions[i].ToLower();
            string mime = ext == "png" ? "image/png" : "image/jpeg";
            form.AddBinaryData($"mark_{i}", images[i], $"mark_{i}.{ext}", mime);
        }

        UnityWebRequest req = UnityWebRequest.Post($"{BaseUrl}/dataset/create", form);
        req.SetRequestHeader("X-Auth-Pin", SettingsManager.DatabasePin);

        ApiHelper.StartRequest(req, (text) =>
        {
            var response = JsonConvert.DeserializeObject<CreateResponse>(text);
            onSuccess?.Invoke(response.dataset_id);
        }, onError);
    }

    [Serializable]
    private class CreateResponse
    {
        public string dataset_id;
    }
}

// Вспомогательный класс для корутин
public class ApiHelper : MonoBehaviour
{
    public static void StartRequest(
        UnityWebRequest req,
        Action<string> onSuccess,
        Action<string> onError = null)
    {
        GameObject go = new GameObject("CloudAPI_Helper");
        ApiHelper helper = go.AddComponent<ApiHelper>();
        helper.StartCoroutine(helper.DoRequest(req, onSuccess, onError));
    }

    private IEnumerator DoRequest(
        UnityWebRequest req,
        Action<string> onSuccess,
        Action<string> onError = null)
    {
        yield return req.SendWebRequest();

        if (req.result == UnityWebRequest.Result.Success)
        {
            onSuccess?.Invoke(req.downloadHandler.text);
        }
        else
        {
            string errorMsg = req.error ?? "Unknown error";
            Debug.LogError($"CloudAPI Error: {errorMsg}");
            onError?.Invoke(errorMsg);
        }

        Destroy(gameObject);
    }
}