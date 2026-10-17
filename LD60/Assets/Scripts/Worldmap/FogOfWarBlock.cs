using UnityEngine;

public class FogOfWarBlock : MonoBehaviour
{
    private Transform playerTransform;
    private float playerVisionRange;
    public void Initialize(float size, Transform player, float playerVision)
    {
        transform.localScale = new Vector2(size, size);
        playerTransform = player;
        playerVisionRange = playerVision;
    }

    void Update()
    {
        if (Vector2.Distance(transform.position, playerTransform.position) < playerVisionRange)
        {
            Kill();
        }
    }

    public void Kill()
    {
        Destroy(gameObject);
    }
}
