using UnityEngine;

public class Rotator : MonoBehaviour
{
    public float speed = 60;

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.up, Time.deltaTime * speed, Space.World);
    }
}
