using UnityEngine;

public class StatsManager : MonoBehaviour
{
    [SerializeField] public int curExperiencePoints = 0;

    public void AddExperiencePoints(int expPointsToAdd)
    {
        curExperiencePoints += expPointsToAdd;
    }
}