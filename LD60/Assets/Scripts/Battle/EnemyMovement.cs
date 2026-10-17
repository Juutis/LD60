using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField]
    private float rotateSpeed;
    [SerializeField]
    private float moveSpeed;
    [SerializeField]
    private float aggroRange;

    private Rigidbody rb;

    private EnemyState state = EnemyState.Roaming;
    private Vector3 roamingTarget;
    private PlayerMovement player;
    private float aggroRangeSqr;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        roamingTarget = transform.position;
        rb = GetComponent<Rigidbody>();
        player = FindAnyObjectByType<PlayerMovement>();
        aggroRangeSqr = Mathf.Pow(aggroRange, 2);
    }

    // Update is called once per frame
    void Update()
    {
        if (state == EnemyState.Roaming)
        {
            if ((roamingTarget - transform.position).sqrMagnitude <= 0.3f)
            {
                Vector2 t2 = Random.onUnitCircle * 3f;
                roamingTarget = transform.position + new Vector3(t2.x, 0f, t2.y);
            }

            rb.linearVelocity = transform.forward * moveSpeed;
            rb.rotation = Quaternion.RotateTowards(rb.rotation, Quaternion.LookRotation(roamingTarget - transform.position, transform.up), rotateSpeed);

            if ((player.transform.position - transform.position).sqrMagnitude <= aggroRangeSqr)
            {
                state = EnemyState.Aggro;
                Debug.Log("Player found");
            }
        }
        else if (state == EnemyState.Aggro)
        {
            rb.rotation = Quaternion.RotateTowards(rb.rotation, Quaternion.LookRotation(player.transform.position - transform.position, transform.up), rotateSpeed);
            rb.linearVelocity = transform.forward * moveSpeed;
        }
        else if(state == EnemyState.Attacking)
        {

        }
    }
}

public enum EnemyState
{
    Roaming,
    Aggro,
    Attacking,
}
