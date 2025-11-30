using UnityEngine;

public class DollClickHandler : MonoBehaviour
{
    public FormationController formationController;

    private void OnMouseDown()
    {
        if (formationController != null)
        {
            formationController.OnDollClicked(transform);
        }
        else
        {
            Debug.LogError("FormationController не назначен на кукле: " + name);
        }
    }
}