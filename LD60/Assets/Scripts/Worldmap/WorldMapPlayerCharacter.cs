using UnityEngine;
using UnityEngine.InputSystem;

public class WorldMapPlayerCharacter : MonoBehaviour
{
    [SerializeField]
    private Rigidbody2D rigidbody2D;
    [SerializeField]
    private float speed;
    [SerializeField]
    private float mapNotMovingFactor;

    [SerializeField]
    private ScrollingWorldMap scrollingWorldMap;

    private Vector2 previousPosition;

    private Vector2 moveValue;

    [SerializeField]
    private bool moveEnabled = false;

    private float movedDistance = 0f;
    public float MovedDistance { get { return movedDistance; } }

    void Start()
    {
        previousPosition = transform.position;
    }

    void Update()
    {
        if (!moveEnabled)
        {
            return;
        }
        // Gather Input
        var moveAction = InputSystem.actions.FindAction("Move");
        moveValue = moveAction.ReadValue<Vector2>();
    }

    void FixedUpdate()
    {
        if (!moveEnabled)
        {
            return;
        }
        // move map, if map is not moved move player slightly faster
        Vector2 mapMovement = scrollingWorldMap.MoveMap(moveValue);
        Vector2 moveAmount = moveValue + (mapMovement * mapNotMovingFactor);
        Vector2 vel = moveAmount * speed * Time.deltaTime;

        rigidbody2D.linearVelocity = vel;

        movedDistance += Vector2.Distance(transform.position, previousPosition);
        previousPosition = transform.position;
    }
}
