using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private float rotateSpeed;
    [SerializeField]
    private float moveSpeed;
    [SerializeField]
    private GameObject playerModel;

    private Rigidbody rb;
    private InputAction moveAction;

    private bool moveBlocked = false;

    private BattleAnimator battleAnim;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        moveAction = InputSystem.actions.FindAction("move");
        rb = GetComponent<Rigidbody>();
        battleAnim = GetComponentInChildren<BattleAnimator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (moveBlocked)
        {
            rb.linearVelocity = Vector3.zero;
            return;
        }

        Vector2 move = moveAction.ReadValue<Vector2>();
        if (move.sqrMagnitude > 0.1f)
        {
            rb.linearVelocity = new Vector3(move.x, 0f, move.y) * moveSpeed;
            rb.rotation = Quaternion.RotateTowards(rb.rotation, Quaternion.LookRotation(new(move.x, 0f, move.y), transform.up), rotateSpeed);
            battleAnim.PlayState("Run");
        }
        else
        {
            rb.linearVelocity = Vector3.zero;
            battleAnim.PlayState("Idle");
        }
    }

    public void BlockMoving(bool isBlocked)
    {
        moveBlocked = isBlocked;
    }
}
