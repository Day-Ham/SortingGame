using UnityEngine;

public class TitleScreenRobotBuddy : MonoBehaviour
{
    [SerializeField] private int boughtRobotBuddy = 0;
    [SerializeField] private int maxRobotBuddy = 8;
    private RobotBuddySpawner robotBuddySpawner;

    private void Start()
    {
        robotBuddySpawner = FindAnyObjectByType<RobotBuddySpawner>();

        for (int i = 0; i < maxRobotBuddy; i++)
        {
            BuyARobotBuddy();
        }
    }
    public void BuyARobotBuddy()
    {
        if (boughtRobotBuddy >= maxRobotBuddy) return;

        boughtRobotBuddy++;
        robotBuddySpawner.SpawnRobotBuddy();
    }
}
