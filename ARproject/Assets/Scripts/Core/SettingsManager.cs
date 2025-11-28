// Assets/Scripts/Core/SettingsManager.cs
using UnityEngine;

public static class SettingsManager
{
    public static string DatabasePin => PlayerPrefs.GetString("DB_PIN", "0000");
    public static void SetDatabasePin(string pin) => PlayerPrefs.SetString("DB_PIN", pin);
    public static void SetCloudUrl(string url)
    {
        CloudAPI.BaseUrl = url.TrimEnd('/');
        PlayerPrefs.SetString("CLOUD_URL", CloudAPI.BaseUrl);
    }
    public static void Initialize()
    {
        if (PlayerPrefs.HasKey("CLOUD_URL"))
            CloudAPI.BaseUrl = PlayerPrefs.GetString("CLOUD_URL");
    }
}