using UnityEngine;

public class RobotBuddySpawner : MonoBehaviour
{
    [SerializeField] private GameObject robotBuddyPrefab;

    public void SpawnRobotBuddy()
    {
        Instantiate(robotBuddyPrefab, transform);
    }
}
