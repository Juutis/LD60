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
    private GameObject fogOfWarPrefab;
    [SerializeField]
    private float fogOfWarBlockSize = 0.4f;
    [SerializeField]
    private float playerVisionRange = 0.2f;


    private Vector2 moveValue;
/*
    [SerializeField]
    private bool moveEnabled = true;*/

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
        fogOfWarContainer.transform.localPosition = new Vector2(-mapImage.bounds.extents.x, mapImage.bounds.extents.y);
        Debug.Log(mapImage.bounds);
        Vector2Int amountOfBlocks = new (
            Mathf.CeilToInt(mapImage.bounds.size.x / fogOfWarBlockSize) + 1,
            Mathf.CeilToInt(mapImage.bounds.size.y / fogOfWarBlockSize) + 1
        );
        for(int x = 0; x < amountOfBlocks.x; x += 1)
        {
            for (int y = 0; y < amountOfBlocks.y; y += 1)
            {
                FogOfWarBlock fogOfWarBlock = Instantiate(
                    fogOfWarPrefab,
                    new Vector3(
                        x * fogOfWarBlockSize,
                        -y * fogOfWarBlockSize,
                        0f
                    ),
                    Quaternion.identity
                ).GetComponent<FogOfWarBlock>();
                fogOfWarBlock.name = $"[X {x}][Y {y}]";
                fogOfWarBlock.Initialize(fogOfWarBlockSize, playerTransform, playerVisionRange);
                fogOfWarBlock.transform.SetParent(fogOfWarContainer, false);
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

    void Update()
    {
        /*
        if (!moveEnabled)
        {
            return;
        }*/
        // Gather Input
        /*var moveAction = InputSystem.actions.FindAction("Move");
        moveValue = moveAction.ReadValue<Vector2>();*/
    }

    void FixedUpdate()
    {
        /*
        if (!moveEnabled)
        {
            return;
        }*/
        // move player
        /*Vector2 newPosition = (Vector2)mapTransform.position + -(moveValue * speed * Time.deltaTime);
        SetPosition(newPosition);*/
        /*Vector2 clamped = new(
            Mathf.Clamp(newPosition.x, horizontalBounds.x, horizontalBounds.y),
            Mathf.Clamp(newPosition.y, verticalBounds.x, verticalBounds.y)
        );
        transform.position = clamped;*/
    }
}
