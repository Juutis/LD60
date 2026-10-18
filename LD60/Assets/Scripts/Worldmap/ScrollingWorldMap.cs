using UnityEngine;
using UnityEngine.InputSystem;

public class ScrollingWorldMap : MonoBehaviour
{
    private Vector2 horizontalBounds;
    private Vector2 verticalBounds;

    [SerializeField]
    private Transform playerTransform;
    [SerializeField]
    private SpriteRenderer mapSprite;

    [SerializeField]
    private SpriteRenderer mask;

    [SerializeField]
    private Sprite mapImage;

    [SerializeField]
    private Transform mapTransform;

    [SerializeField]
    private float speed;

    [SerializeField]
    private Transform fogOfWarContainer;
    [SerializeField]
    private FogOfWarBlock fogOfWarPrefab;
    [SerializeField]
    private float fogOfWarBlockSize = 0.4f;
    [SerializeField]
    private float playerVisionRange = 0.2f;

    void Start()
    {
        mapSprite.sprite = mapImage;
        SetupTexture();
        GenerateFogOfWar();
    }

    private void SetupTexture()
    {
        Vector2 mapTexturePos = new(
            mapImage.bounds.extents.x - mask.bounds.extents.x,
            mask.bounds.extents.y - mapImage.bounds.extents.y
        );
        horizontalBounds = new(-mapTexturePos.x, mapTexturePos.x);
        verticalBounds = new(mapTexturePos.y, -mapTexturePos.y);
        SetPosition(mapTexturePos);
    }

    private void GenerateFogOfWar()
    {
        fogOfWarContainer.transform.localPosition = new Vector2(
            -mapImage.bounds.extents.x - fogOfWarBlockSize/2,
            mapImage.bounds.extents.y + fogOfWarBlockSize / 2
        );
        Debug.Log(mapImage.bounds);
        Vector2Int amountOfBlocks = new (
            Mathf.CeilToInt(mapImage.bounds.size.x / fogOfWarBlockSize) + 1,
            Mathf.CeilToInt(mapImage.bounds.size.y / fogOfWarBlockSize) + 1
        );
        for(int x = 0; x < amountOfBlocks.x; x += 1)
        {
            for (int y = 0; y < amountOfBlocks.y; y += 1)
            {
                //FogOfWarBlock fogOfWarBlock = Instantiate(fogOfWarPrefab).GetComponent<FogOfWarBlock>();
                FogOfWarBlock fogOfWarBlock = Instantiate(fogOfWarPrefab);
                fogOfWarBlock.Initialize(x, y, fogOfWarBlockSize, fogOfWarContainer, playerTransform, playerVisionRange);
            }
        }
        Debug.Log($"amount: {amountOfBlocks}");
    }

    private void SetPosition(Vector2 newPosition)
    {
        //Debug.Log($"Setting position to {newPosition}");
        Vector2 clamped = new(
            Mathf.Clamp(newPosition.x, horizontalBounds.x, horizontalBounds.y),
            Mathf.Clamp(newPosition.y, verticalBounds.x, verticalBounds.y)
        );
        mapTransform.position = clamped;
    }

    public Vector2 MoveMap(Vector2 moveValue)
    {
        Vector2 newPosition = (Vector2)mapTransform.position - (moveValue * speed * Time.deltaTime);
        SetPosition(newPosition);
        return (Vector2)mapTransform.position - newPosition;
    }
}
