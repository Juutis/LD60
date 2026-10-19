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

    private bool visited = false;

    [SerializeField]
    private LevelConfig level;

    [SerializeField]
    private TextMeshPro txtMesh;

    [SerializeField]
    private TextMeshPro nameText;

    [SerializeField]
    private float minimumDistanceToEnter = 1f;

    [SerializeField]
    private float minimumDistanceToShowName = 1f;

    [SerializeField]
    private float checkDistanceInterval = 0.2f;

    private float checkDistanceTimer = 0f;

    private bool playerIsClose = false;

    private Transform playerTransform;

    private InputAction interactAction;


    void Start()
    {
        playerTransform = WorldMapPlayerCharacter.main.transform;
        nameText.text = poiName;

        interactAction = InputSystem.actions.FindAction("interact");

        txtMesh.text = $"- Press [{interactAction.GetBindingDisplayString(0)}] to enter -";
    }

    public void Clear()
    {
        visited = true;
        txtMesh.text = "- Location cleared - ";
    }

    void Update()
    {
        if (visited)
        {
            return;
        }
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
            if (distance < minimumDistanceToShowName)
            {
                nameText.gameObject.SetActive(true);
            } else
            {
                nameText.gameObject.SetActive(false);
            }
        }
        if (playerIsClose)
        {
            if (interactAction.WasPressedThisFrame())
            {
                GameManager.main.LoadLevel(level, delegate
                {
                    Clear();
                });
            }
        }
    }
}
