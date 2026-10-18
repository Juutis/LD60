using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class WorldMapPoi : MonoBehaviour
{
    [SerializeField]
    private string poiName;
    public string PoiName { get { return poiName; } }

    [SerializeField]
    private Sprite sprite;

    public Sprite Sprite { get { return sprite; } }

    [SerializeField]
    private LevelConfig level;

    [SerializeField]
    private TextMeshPro txtMesh;

    [SerializeField]
    private float minimumDistanceToEnter = 1f;

    [SerializeField]
    private float checkDistanceInterval = 0.2f;

    private float checkDistanceTimer = 0f;

    private bool playerIsClose = false;

    private Transform playerTransform;


    void Start()
    {
        playerTransform = WorldMapPlayerCharacter.main.transform;
    }

    void Update()
    {
        checkDistanceTimer += Time.deltaTime;
        if (checkDistanceTimer > checkDistanceInterval)
        {
            checkDistanceTimer = 0f;
            float distance = Vector2.Distance(playerTransform.position, transform.position);

            if (distance < minimumDistanceToEnter)
            {
                playerIsClose = true;
                txtMesh.gameObject.SetActive(true);
            }
            else
            {
                playerIsClose = false;
                txtMesh.gameObject.SetActive(false);
            }
        }
        if (playerIsClose)
        {
            if (Keyboard.current[Key.Space].wasPressedThisFrame)
            {
                GameManager.main.LoadLevel(level);
            }
        }
    }
}
