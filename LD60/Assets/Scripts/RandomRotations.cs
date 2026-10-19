using UnityEngine;

public class RandomRotations : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        transform.Rotate(Vector3.up, Random.Range(0.0f, 360.0f), Space.World);   
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
