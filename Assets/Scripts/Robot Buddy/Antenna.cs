using UnityEngine;

public class Antenna : MonoBehaviour
{
    [SerializeField] private float rotSpeed;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.up * rotSpeed * Time.deltaTime, Space.Self);
    }
}
