using UnityEngine;
using UnityEngine.AI;

public class RobotBuddyHover : MonoBehaviour
{
    private NavMeshAgent agent;
    [SerializeField] private float hoverAmount;
    [SerializeField] private float hoverSpeed;
    [SerializeField] private float tiltAngle;
    private float tiltAngleX;
    private float tiltAngleZ;
    [SerializeField] private float tiltSpeed;

    private void Start()
    {
        agent = transform.root.GetComponentInParent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.timeScale == 0f) return;
        HoverEffect();
    }

    private void HoverEffect()
    {
        //bounce
        float bounce = Mathf.Sin(Time.time * hoverSpeed) * hoverAmount;
        agent.baseOffset += bounce * 0.01f;

        //tilt rotate
        float tiltX = Mathf.Sin(Time.time * tiltSpeed) * tiltAngle;
        float tiltZ = Mathf.Cos(Time.time * tiltSpeed) * tiltAngle;

        Vector3 newRotation = new Vector3(tiltX, 0f, tiltZ);
        transform.localRotation = Quaternion.Euler(newRotation);
    }
}
