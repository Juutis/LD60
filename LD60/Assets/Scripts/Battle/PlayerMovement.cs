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

    private bool moveSlowed = false;
    private float moveSlowCoef = 0.15f;

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
        Vector2 move = moveAction.ReadValue<Vector2>();
        if (move.sqrMagnitude > 0.1f)
        {
            rb.linearVelocity = new Vector3(move.x, 0f, move.y) * moveSpeed * (moveSlowed ? moveSlowCoef : 1f);
            playerModel.rotation = Quaternion.RotateTowards(playerModel.rotation, Quaternion.LookRotation(new(move.x, 0f, move.y), transform.up), rotateSpeed);
            battleAnim.PlayState("Run");
        }
        else
        {
            rb.linearVelocity = Vector3.zero;
            battleAnim.PlayState("Idle");
        }
    }

    public void SlowMoving(bool isSlowed)
    {
        moveSlowed = isSlowed;
    }

    public Vector3 GetDirection()
    {
        return playerModel.transform.forward;
    }
}
