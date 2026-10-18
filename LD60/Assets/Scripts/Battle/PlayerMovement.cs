using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private float rotateSpeed;
    [SerializeField]
    private float moveSpeed;
    [SerializeField]
    private Transform playerModel;

    private Rigidbody rb;
    private InputAction moveAction;

    private bool moveBlocked = false;

    private BattleAnimator battleAnim;

    private Camera cam;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        moveAction = InputSystem.actions.FindAction("move");
        rb = GetComponent<Rigidbody>();
        battleAnim = GetComponentInChildren<BattleAnimator>();
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (moveBlocked)
        {
            rb.linearVelocity = Vector3.zero;
            return;
        }

        var forward = cam.transform.forward;
        forward.y = 0.0f;
        forward.Normalize();
        var left = Vector3.Cross(Vector3.up, forward);
        Vector2 move = moveAction.ReadValue<Vector2>();
        if (move.sqrMagnitude > 0.1f)
        {
            var moveDir = (forward * move.y + left * move.x).normalized;
            rb.linearVelocity = moveDir * moveSpeed;
            playerModel.rotation = Quaternion.RotateTowards(playerModel.rotation, Quaternion.LookRotation(moveDir, transform.up), rotateSpeed);
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

    public Vector3 GetDirection()
    {
        return playerModel.transform.forward;
    }
}
