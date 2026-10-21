using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class RobotBuddy : MonoBehaviour
{
    private NavMeshAgent agent;
    private float baseOffset;
    private RobotAudio robotAudio;

    [Header("Pick Up")]
    [SerializeField] private Transform holdPoint;
    [SerializeField] private float pickupSpeed;

    [Header("Put Down")]
    [SerializeField] private float stoppingDistFromShelf;
    [SerializeField] private float putDownSpeed;

    private Vector3 targetDestination;

    [Header("Target Item")]
    [SerializeField] private List<Item> foundItems = new List<Item>();
    public Item targetItem; /*{ get; private set; }*/
    [SerializeField] private GameObject heldObject;
    private Rigidbody heldRigidbody;

    [Header("Target Shelf")]
    [SerializeField] private GameObject targetShelf;
    private ItemChecker targetItemChecker;

    [Header("State")]
    [SerializeField] private bool isPickingUp;
    [SerializeField] private bool isPlacing;
    [SerializeField] private bool isMovingVertically;
    [SerializeField] private bool isResetingVertically;
    [SerializeField] private bool isChasingStolenItem;
    [SerializeField] private bool isRoaming;

    [Header("Random Roaming")]
    //[SerializeField] private float roamRange = 25f;
    [SerializeField] private float roamWaitTime = 1f;
    [SerializeField] private float roamTimer;
    public List<Transform> patrolPoints;
    private int currentPatrolIndex = 0;

    ItemSpawner itemSpawner;
    PlayerInteraction player;

    private void Awake()
    {
        itemSpawner = FindAnyObjectByType<ItemSpawner>();
        player = FindAnyObjectByType<PlayerInteraction>();
    }

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        baseOffset = agent.baseOffset;
        robotAudio = GetComponent<RobotAudio>();
    }

    private void Update()
    {     
        if (targetItem != null && targetItem.HeldBy == player.transform && !isChasingStolenItem)
        {
            Debug.Log($"Robot lost {targetItem.name} to another holder. Initiating chase!");
            isChasingStolenItem = true;
            ResetToPickUp();
        }
        else if (targetItem != null && targetItem.HeldBy != player.transform && targetItem.HeldBy != transform && targetItem.HeldBy != null && isChasingStolenItem)
        {
            isChasingStolenItem = false;
            CancelPickup();
        }

        if (isResetingVertically)
            ResetBaseOffset();

        if (isPlacing) return;

        if (targetItem == null)
        {
            if (heldObject == null)
            {
                isChasingStolenItem = false;
                FindItem();

                if (targetItem == null)
                {
                    StartRoaming();

                    if (agent.isOnNavMesh)
                    {
                        roamTimer += Time.deltaTime;

                        if (!agent.pathPending &&
                            (!agent.hasPath ||
                             agent.remainingDistance <= agent.stoppingDistance))
                        {
                            if (roamTimer >= roamWaitTime)
                            {
                                ChooseNextPatrolPoint();
                                roamTimer = 0f;
                            }
                        }
                    }
                    return;
                }
            }
            return;
        }

        if (isRoaming)
        {
            isRoaming = false;
            roamTimer = 0f;

            if (agent.isOnNavMesh)
                agent.ResetPath();
        }

        if (isChasingStolenItem)
        {
            targetDestination = targetItem.transform.position;
        }

        if (isPickingUp)
        {
            if (heldObject == null)
            {
                CancelPickup();
                return;
            }

            PickupLerp();
            return;
        }

        if (MoveToTargetDestination())
        {
            if (targetItemChecker != null && !isPlacing)
            {
                isPlacing = true;
                StartCoroutine(PlaceItemOnShelf());
            }
            else
            {
                TryPickup(targetItem);
            } 
        }
    }

    private bool MoveToTargetDestination()
    {
        //moving vertically
        if (isMovingVertically)
        {
            float direction = Mathf.Sign(targetItemChecker.transform.position.y - heldObject.transform.position.y);
            agent.baseOffset += direction * agent.speed * Time.deltaTime;

            if (Mathf.Abs(targetItemChecker.transform.position.y - heldObject.transform.position.y) < 0.01f)
            {
                isMovingVertically = false;
                return true;
            }
            return false;
        }

        //moving horizontally
        Vector3 targetDestinationXZ = new Vector3(targetDestination.x, transform.position.y, targetDestination.z);

        agent.SetDestination(targetDestinationXZ);

        if (targetItemChecker == null && !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance) // item found, go get it
        {
            return true;
        }
        else if (targetItemChecker != null && !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance + stoppingDistFromShelf) //found shelf, try to move vertically
        {
            isMovingVertically = true;
        }
        return false;
    }

    private void FindItem()
    {
        if (heldObject != null || targetItem != null) return;

        foundItems.Clear();

        Item[] allItems = FindObjectsByType<Item>();
        RobotBuddy[] allBuddies = FindObjectsByType<RobotBuddy>();

        foreach (Item item in allItems)
        {
            if (item == null) continue;
            if (item.IsHeld) continue;
            if (!item.GetComponent<Rigidbody>().isKinematic) continue;
            if (item.GetComponentInParent<ItemChecker>()) continue;

            bool isTargetedByAnotherRobot = false;
            foreach (RobotBuddy buddy in allBuddies)
            {
                if (buddy != this && buddy.targetItem == item)
                {
                    isTargetedByAnotherRobot = true;
                    break;
                }
            }

            if (isTargetedByAnotherRobot) continue;

            ItemSpawner spawner = item.GetComponentInParent<ItemSpawner>();
            if (spawner == null) continue;

            foundItems.Add(item);
        }

        if (foundItems.Count == 0) return;

        targetItem = foundItems[Random.Range(0, foundItems.Count)];

        targetDestination = targetItem.transform.position;
    }

    private void TryPickup(Item item)
    {
        if (item == null) return;

        if (targetItem != null && targetItem.HeldBy == player.transform)
        {
            player.RemoveItemFromPlayer(targetItem.gameObject);
        }

        Rigidbody rb = item.GetComponent<Rigidbody>();
        if (rb == null) return;

        heldObject = item.gameObject;
        heldRigidbody = rb;

        item.Held(true, transform);

        robotAudio.PlayPickupSound();
        Debug.Log($" Robot picked up {item.name}. HeldBy = {item.HeldBy.name}");

        isPickingUp = true;
        isChasingStolenItem = false;

        if (agent.isOnNavMesh)
            agent.ResetPath();
    }

    private void PickupLerp()
    {
        if (heldObject == null)
        {
            CancelPickup();
            return;
        }

        heldObject.SetActive(true);
        Transform obj = heldObject.transform;

        obj.position = Vector3.Lerp(obj.position, holdPoint.position, pickupSpeed * Time.deltaTime);
        obj.rotation = Quaternion.Lerp( obj.rotation, holdPoint.rotation, pickupSpeed * Time.deltaTime);

        if (Vector3.Distance(obj.position, holdPoint.position) < 0.01f && Quaternion.Angle(obj.rotation, holdPoint.rotation) < 1f)
        {
            obj.position = holdPoint.position;
            obj.rotation = holdPoint.rotation;

            obj.SetParent(holdPoint, true);

            obj.localPosition = Vector3.zero;
            obj.localRotation = Quaternion.identity;

            isPickingUp = false;

            
            Debug.Log($"Robot is now holding {obj.name}");
            FindShelf();
        }
    }

    private void DropItem()
    {
        if (heldObject != null /*&& targetItem.HeldBy == transform*/)
        {
            heldObject.transform.SetParent(itemSpawner != null ? itemSpawner.transform : null);
            if (heldRigidbody != null)
                heldRigidbody.isKinematic = false;
        }

        CancelPickup();
        Debug.Log("There are no shelves available :(. Dropping the item.");
    }

    private bool FindShelf()
    {
        //arrange shelves based on number of children, makes sure to choose shelf with the most same item 
        ItemChecker[] shelves = FindObjectsByType<ItemChecker>()
            .OrderByDescending(shelf => shelf.heldItems.Count)
            .ToArray();

        if (shelves.Length == 0)
        {
            DropItem();
            return false;
        }

        foreach (ItemChecker shelf in shelves)
        {
            if (shelf.IsItemValid(targetItem))
            {
                if (shelf.isAssignedToBuddy)
                {
                    if (shelf.robotList[0].targetItem.Data.displayName != targetItem.Data.displayName)
                    {
                        continue;
                    }
                }

                targetItemChecker = shelf;
                targetShelf = shelf.gameObject;
                targetItemChecker.AssignToBuddy(true, this);
                break;
            }
        }

        if (targetItemChecker == null)
        {
            DropItem();
            return false;
        }

        targetDestination = targetItemChecker.transform.position;
        return true;
    }

    IEnumerator PlaceItemOnShelf()
    {
        yield return new WaitUntil(() => !targetItemChecker.isPlacingItem && targetItemChecker.currentItem == null && targetItemChecker.robotList[0] == this); // makes sure the shelf is not busy doing anything
       
        //change move speed of placing items of the shelf for slower effect;
        float initialShelfMoveSpeed = targetItemChecker.moveSpeed;
        targetItemChecker.moveSpeed = putDownSpeed;
        
        targetItemChecker.PlaceItem(targetItem);

        yield return new WaitUntil(() => !targetItem.isBeingPlaced); // waits for the placing action to finish before doing anything else
        
        //return initial move speed of placing items in shelf
        targetItemChecker.moveSpeed = initialShelfMoveSpeed;

        Rigidbody rb = targetItem.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.isKinematic = true;
        }

        Debug.Log($" Robot placed {targetItem.name} on {targetShelf.name}.");
        isResetingVertically = true; //go back to initial hover height

        ResetTargetShelf();
        CancelPickup();
        isPlacing = false;
        isChasingStolenItem = false;
    }

    private void CancelPickup()
    {
        if (targetItem != null)
        {
            if (targetItem.HeldBy == transform)
            {
                targetItem.Held(false);
                targetItem = null;
            }
            else if (targetItem.HeldBy != player.transform && targetItem.HeldBy != null)
            {
                targetItem.Held(false);
                targetItem = null;
            }
        }

        isPickingUp = false;

        heldObject = null;
        heldRigidbody = null;
    }

    private void ResetTargetShelf()
    {
        targetItemChecker.AssignToBuddy(false, this);
        targetShelf = null;
        targetItemChecker = null;
    }

    private void ResetBaseOffset()
    {
        agent.baseOffset = Mathf.MoveTowards(agent.baseOffset,baseOffset,agent.speed * Time.deltaTime);
        
        if (Mathf.Abs(agent.baseOffset - baseOffset) <= 0.01f)
        {
            agent.baseOffset = baseOffset;
            FindItem();
            isResetingVertically = false;
        }
    }

    private void ResetToPickUp()
    {
        if (targetItem == null) return;
       
        targetDestination = targetItem.transform.position;

        isPlacing = false;
        isPickingUp = false;
        isMovingVertically = false;
        isResetingVertically = true;
        ResetTargetShelf();

        if (heldObject != null)
        {
            heldObject.transform.SetParent(null);
            heldObject = null;
            heldRigidbody = null;
        }

        if (agent != null && agent.isOnNavMesh)
            agent.ResetPath();

        Debug.Log("robot was reset to chase after item");
    }

    private void StartRoaming()
    {
        if (isRoaming)
            return;

        isRoaming = true;
        roamTimer = 0f;

        if (agent.isOnNavMesh)
            agent.ResetPath();

        ChooseNextPatrolPoint();
    }

    private void ChooseNextPatrolPoint()
    {
        if (patrolPoints == null || patrolPoints.Count == 0)
        {
            Debug.LogWarning("No patrol points assigned!");
            return;
        }

        agent.SetDestination(patrolPoints[currentPatrolIndex].position);
        currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Count; //go back to zero at the end of the list  
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
    }
}