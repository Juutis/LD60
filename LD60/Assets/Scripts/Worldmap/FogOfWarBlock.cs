using UnityEngine;

public class FogOfWarBlock : MonoBehaviour
{
    private Transform playerTransform;
    private float playerVisionRange;
    public void Initialize(float x, float y, float size, Transform parentContainer, Transform player, float playerVision)
    {
        transform.localScale = new(size, size);
        transform.position = new(x * size, -y * size);
        transform.SetParent(parentContainer, false);
        name = $"[X {x}][Y {y}]";
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
