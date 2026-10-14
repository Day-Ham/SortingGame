using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

public class RobotBuddySpawner : MonoBehaviour
{
    [SerializeField] private GameObject robotBuddyPrefab;
    [SerializeField] private List<Transform> patrolPoints;

    public void SpawnRobotBuddy()
    {
        GameObject buddy = Instantiate(robotBuddyPrefab);
        buddy.transform.SetParent(null);
        buddy.GetComponent<RobotBuddy>().patrolPoints = patrolPoints;
    }
}
