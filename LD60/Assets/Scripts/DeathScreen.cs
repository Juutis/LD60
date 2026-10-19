using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.SceneManagement;


public class DeathScreen : MonoBehaviour
{
    private InputAction interactAction;

    [SerializeField]
    private TextMeshProUGUI textMeshProUGUI;


    [SerializeField]
    private Animator animator;

    private bool startEnabled = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        interactAction = InputSystem.actions.FindAction("interact");

        textMeshProUGUI.text = $"- Press [{interactAction.GetBindingDisplayString(0)}] to continue -";
        animator.Play("ShowDeathScreen");
    }

    public void EnableStart()
    {
        startEnabled = true;
        Debug.Log("StartEnabled");
    }

    // Update is called once per frame
    void Update()
    {
        if (startEnabled)
        {
            if (interactAction.WasPressedThisFrame())
            {
                GameManager.main.LoadWorldMap();
            }
        }
    }
}
