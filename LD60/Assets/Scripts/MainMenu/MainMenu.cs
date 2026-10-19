using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenu : MonoBehaviour
{
    [SerializeField]
    private float fontMoveSpeed = 2f;

    [SerializeField]
    private float extraSpeedWhenKeyIsPressed = 5f;

    [SerializeField]
    private float waitBeforeStartDuration = 2f;


    [SerializeField]
    private TextMeshProUGUI startText;

    private bool isWaiting = true;
    private bool isScrolling = false;
    private bool startEnabled = false;

    private float timer = 0f;

    private InputAction interactAction;

    [SerializeField]
    private RectTransform textContent;

    [SerializeField]
    private Animator animator;

    float endpointY;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        endpointY = textContent.sizeDelta.y;
        interactAction = InputSystem.actions.FindAction("interact");

        startText.text = $"- Press [{interactAction.GetBindingDisplayString(0)}] to start the game -";
    }

    public void EnableStart()
    {
        startEnabled = true;
        
    }

    // Update is called once per frame
    void Update()
    {
        if (startEnabled)
        {
            if (interactAction.WasPressedThisFrame())
            {
                SceneManager.LoadScene("worldmap");
            }
        }
        if (isWaiting)
        {
            timer += Time.deltaTime;
            if (timer > waitBeforeStartDuration)
            {
                timer = 0f;
                isWaiting = false;
                isScrolling = true;
            }
            return;
        }
        if (!isScrolling)
        {
            return;
        }

        bool keyIsPressed = false;

        if (Keyboard.current.anyKey.isPressed)
        {
            keyIsPressed = true;
        }

        textContent.anchoredPosition = new (
            textContent.anchoredPosition.x,
            textContent.anchoredPosition.y + fontMoveSpeed + (keyIsPressed ? extraSpeedWhenKeyIsPressed : 0)
        );

        if (textContent.anchoredPosition.y > endpointY)
        {
            isScrolling = false;
            animator.Play("ShowMainMenu");
        }
    }

}
