using UnityEngine;

/// <summary>
/// Делает 3D-объект кликабельным через OnMouseDown().
/// Вызывает допрос через InterrogationManager.
/// </summary>
public class SuspectInteractable : MonoBehaviour
{
    public int suspectIndex = 0;

    void OnMouseDown()
    {
        InterrogationManager manager = FindObjectOfType<InterrogationManager>();
        if (manager == null)
        {
            Debug.LogError("SuspectInteractable: InterrogationManager не найден!");
            return;
        }

        manager.StartInterrogationFromClick(suspectIndex);
    }
}