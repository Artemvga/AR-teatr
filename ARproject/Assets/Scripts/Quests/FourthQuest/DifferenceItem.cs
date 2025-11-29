// DifferenceItem.cs
using UnityEngine;

public class DifferenceItem : MonoBehaviour
{
    public static System.Action OnDifferenceClicked;

    private bool _isFound = false;

    void OnMouseDown()
    {
        if (_isFound) return;

        _isFound = true;
        gameObject.SetActive(false); // или GetComponent<Renderer>().enabled = false; Ч как хочешь

        OnDifferenceClicked?.Invoke();
    }
}