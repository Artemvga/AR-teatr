// Assets/Scripts/Core/DatasetManager.cs
using System;
using UnityEngine;

[Serializable]
public class DatasetInfo
{
    public string name;
    public int version;
    public string[] marks; // ровно 9
}

public static class DatasetManager
{
    private const string KEY = "ActiveDataset";
    public static string ActiveDatasetName
    {
        get => PlayerPrefs.GetString(KEY, "-1");
        set { PlayerPrefs.SetString(KEY, value); PlayerPrefs.Save(); }
    }

    public static bool HasActiveDataset => ActiveDatasetName != "-1";

    public static void LoadActiveDataset(Action<DatasetInfo> onComplete)
    {
        if (!HasActiveDataset) { onComplete?.Invoke(null); return; }

        string url = $"{CloudAPI.BaseUrl}/datasets/{ActiveDatasetName}/dataset.json";
        CloudAPI.GetJson<DatasetInfo>(url, info => {
            if (info?.marks?.Length == 9)
                onComplete?.Invoke(info);
            else
                onComplete?.Invoke(null);
        });
    }
}