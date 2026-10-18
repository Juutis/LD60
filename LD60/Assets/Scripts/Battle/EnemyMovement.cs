using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField]
    private float rotateSpeed;
    [SerializeField]
    private float moveSpeed;
    [SerializeField]
    private float aggroRange;
    [SerializeField]
    private float waitingRange;
    [SerializeField]
    private float attackRange;

    private Rigidbody rb;

    private EnemyState state = EnemyState.Roaming;
    private Vector3 roamingTarget;
    private PlayerMovement player;
    private float aggroRangeSqr;
    private float waitingRangeSqr;
    private float attackRangeSqr;
    private bool dodging = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        roamingTarget = transform.position;
        rb = GetComponent<Rigidbody>();
        player = FindAnyObjectByType<PlayerMovement>();
        aggroRangeSqr = Mathf.Pow(aggroRange, 2);
        waitingRangeSqr = Mathf.Pow(waitingRange, 2);
        attackRangeSqr = Mathf.Pow(attackRange, 2);
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
            rb.rotation = Quaternion.RotateTowards(rb.rotation, Quaternion.LookRotation(zeroY(roamingTarget) - zeroY(transform.position), transform.up), rotateSpeed);

            if ((player.transform.position - transform.position).sqrMagnitude <= aggroRangeSqr)
            {
                state = EnemyState.Aggro;
                Debug.Log("Player found");
            }
        }
        else if (state == EnemyState.Aggro)
        {
            rb.rotation = Quaternion.RotateTowards(rb.rotation, Quaternion.LookRotation(zeroY(player.transform.position) - zeroY(transform.position), transform.up), rotateSpeed);
            rb.linearVelocity = transform.forward * moveSpeed;

            if ((player.transform.position - transform.position).sqrMagnitude <= waitingRangeSqr)
            {
                state = EnemyState.Waiting;
            }
        }
        else if(state == EnemyState.Waiting)
        {
            bool belowWaitingRange = (player.transform.position - transform.position).sqrMagnitude <= attackRange * 1.5f;
            bool aboveWaitingRange = (player.transform.position - transform.position).sqrMagnitude >= aggroRangeSqr;
            Debug.Log($"Waiting, moving? {belowWaitingRange} {aboveWaitingRange} {dodging}");
            Vector3 lookDir = zeroY(player.transform.position) - zeroY(transform.position);

            if (belowWaitingRange && !dodging)
            {
                dodging = true;
            }
            else if (aboveWaitingRange)
            {
                rb.linearVelocity = transform.forward * moveSpeed;
                dodging = false;
            }
            else if (dodging)
            {
                lookDir = -1 * lookDir;
                rb.linearVelocity = transform.forward * moveSpeed;
            }
            else
            {
                rb.linearVelocity = Vector3.zero;
            }

            rb.rotation = Quaternion.RotateTowards(rb.rotation, Quaternion.LookRotation(lookDir, transform.up), rotateSpeed);
        }
        else if(state == EnemyState.Attacking)
        {
            Debug.Log("Attacking");
            bool aboveAttackingRange = (player.transform.position - transform.position).sqrMagnitude >= attackRangeSqr;

            if (aboveAttackingRange)
            {
                rb.linearVelocity = transform.forward * moveSpeed;
            }

            Vector3 lookDir = zeroY(player.transform.position) - zeroY(transform.position);
            rb.rotation = Quaternion.RotateTowards(rb.rotation, Quaternion.LookRotation(lookDir, transform.up), rotateSpeed);
        }
    }

    private Vector3 zeroY(Vector3 pos)
    {
        return new Vector3(pos.x, 0, pos.z);
    }

    public void GoAttackMode()
    {
        state = EnemyState.Attacking;
    }

    public void GoAggroMode()
    {
        state = EnemyState.Aggro;
    }
}

public enum EnemyState
{
    Roaming,
    Aggro,
    Waiting,
    Attacking,
}
