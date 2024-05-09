using UnityEngine;

public class ContainerInitialiser : MonoBehaviour
{
    public GameObject slotPrefab;
    public int numberOfSlots;

    public void InitializeSlots(int numberOfSlots)
    {
        this.numberOfSlots = numberOfSlots;
        for (int i = 0; i < numberOfSlots; i++)
        {
            Instantiate(slotPrefab, transform);
        }
    }
}