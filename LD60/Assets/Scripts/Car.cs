using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class Car : MonoBehaviour
{
    public static Car Instance;

    [SerializeField]
    private TextMeshPro tooltipText;

    private bool keyAcquired = false;
    private PlayerMovement player;

    [SerializeField]
    private float minimumDistanceToEnter = 1f;

    [SerializeField]
    private float checkDistanceInterval = 0.2f;

    private float checkDistanceTimer = 0f;

    private bool playerIsClose = false;

    private InputAction interactAction;

    void Awake()
    {
        Instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        interactAction = InputSystem.actions.FindAction("interact");
        
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null)
        {
            player = FindAnyObjectByType<PlayerMovement>();
            return;
        }
        checkDistanceTimer += Time.deltaTime;
        if (checkDistanceTimer > checkDistanceInterval)
        {
            checkDistanceTimer = 0f;
            float distance = Vector3.Distance(player.transform.position, transform.position);
            //Debug.Log($"{distance} = Distance({player.transform.position}, {transform.position})");
            if (distance < minimumDistanceToEnter)
            {
                playerIsClose = true;
                tooltipText.gameObject.SetActive(true);
                if (keyAcquired)
                {
                    tooltipText.text = $"- Press [{interactAction.GetBindingDisplayString(0)}] to go to world map -";
                }
                else
                {
                    tooltipText.text = "- Need to find the key! -";
                }
            }
            else
            {
                playerIsClose = false;
                tooltipText.gameObject.SetActive(false);
            }
        }
        if (playerIsClose && keyAcquired)
        {
            if (interactAction.WasPressedThisFrame())
            {
                keyAcquired = false;
                GameManager.main.LoadWorldMap();
            }
        }
    }

    public void KeyAcquired()
    {
        keyAcquired = true;
    }

}
