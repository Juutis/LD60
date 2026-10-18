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

    private Camera cam;
    private Vector3 camDir;
    private int pickupLayer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        moveAction = InputSystem.actions.FindAction("move");
        rb = GetComponent<Rigidbody>();
        battleAnim = GetComponentInChildren<BattleAnimator>();
        cam = Camera.main;
        pickupLayer = LayerMask.NameToLayer("Pickup");
    }

    // Update is called once per frame
    void Update()
    {
        var forward = cam.transform.forward;
        forward.y = 0.0f;
        forward.Normalize();
        if (forward.magnitude < 0.01f)
        {
            forward = camDir;
        }
        else
        {
            camDir = forward;
        }
        var left = Vector3.Cross(Vector3.up, forward);
        Vector2 move = moveAction.ReadValue<Vector2>();
        if (move.sqrMagnitude > 0.1f)
        {
            var moveDir = (forward * move.y + left * move.x).normalized;
            rb.linearVelocity = moveDir * moveSpeed * (moveSlowed ? moveSlowCoef : 1f);
            playerModel.rotation = Quaternion.RotateTowards(playerModel.rotation, Quaternion.LookRotation(moveDir, transform.up), rotateSpeed);
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

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == pickupLayer)
        {
            var pickup = other.GetComponent<PickUp>();
            if (pickup != null)
            {
                pickup.Trigger();
            }
            var car = other.GetComponent<Car>();
            if (car != null)
            {
                car.TriggerPlayerNearby();
            }
        }
    }
}
