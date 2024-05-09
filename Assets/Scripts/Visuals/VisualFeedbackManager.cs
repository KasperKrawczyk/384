using UnityEngine;

public class VisualFeedbackManager : MonoBehaviour
{
    [SerializeField] private GameObject outlinePrefab; // A prefab with a red outline square of the size of a tile
    private GameObject outlineInstance;

    public void ShowOutline(bool show)
    {
        if (show)
        {
            if (outlineInstance == null)
            {
                outlineInstance = Instantiate(outlinePrefab, transform.position, Quaternion.identity, transform);
            }
            outlineInstance.SetActive(true);
        }
        else
        {
            if (outlineInstance != null)
            {
                outlineInstance.SetActive(false);
            }
        }
    }
}