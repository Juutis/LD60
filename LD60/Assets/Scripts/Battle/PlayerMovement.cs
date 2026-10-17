using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private float rotateSpeed;
    [SerializeField]
    private float moveSpeed;

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
        if (Mathf.Abs(move.y) > 0.1f)
        {
            rb.linearVelocity = transform.forward * move.y * moveSpeed;
            battleAnim.PlayState("Run");
        }
        else
        {
            rb.linearVelocity = Vector3.zero;
            battleAnim.PlayState("Idle");
        }

        if (Mathf.Abs(move.x) > 0.1f)
        {
            rb.rotation = Quaternion.RotateTowards(rb.rotation, Quaternion.LookRotation((move.x)*transform.right, transform.up), rotateSpeed);
        }
    }

    public void BlockMoving(bool isBlocked)
    {
        moveBlocked = isBlocked;
    }
}
