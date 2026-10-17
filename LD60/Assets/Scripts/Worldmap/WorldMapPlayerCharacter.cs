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

    private Vector2 moveValue;

    [SerializeField]
    private bool moveEnabled = false;

    void Start()
    {
        
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
        rigidbody2D.linearVelocity = moveAmount * speed * Time.deltaTime;
    }
}
