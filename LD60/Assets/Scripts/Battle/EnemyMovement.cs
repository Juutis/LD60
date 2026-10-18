using UnityEngine;
using UnityEngine.AI;

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
    private NavMeshAgent agent;

    private EnemyState state = EnemyState.Roaming;
    private Vector3 roamingTarget;
    private PlayerMovement player;
    private float aggroRangeSqr;
    private float waitingRangeSqr;
    private float attackRangeSqr;
    private bool dodging = false;
    private bool alive = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
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
        if (!alive)
        {
            rb.linearVelocity = Vector3.zero;
            agent.destination = transform.position;
            return;
        }
        if (state == EnemyState.Roaming)
        {
            if ((roamingTarget - transform.position).sqrMagnitude <= 2f)
            {
                Vector2 t2 = Random.onUnitCircle * 4f;
                roamingTarget = transform.position + new Vector3(t2.x, 0f, t2.y);
                NavMesh.SamplePosition(roamingTarget, out NavMeshHit navHit, 3f, NavMesh.AllAreas);
                roamingTarget = navHit.position;
                agent.SetDestination(navHit.position);
                Debug.Log(navHit.position);
            }

            // rb.linearVelocity = transform.forward * moveSpeed;
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
            //rb.linearVelocity = transform.forward * moveSpeed;
            agent.SetDestination(player.transform.position);


            if ((player.transform.position - transform.position).sqrMagnitude <= waitingRangeSqr)
            {
                state = EnemyState.Waiting;
            }
        }
        else if(state == EnemyState.Waiting)
        {
            bool belowWaitingRange = (player.transform.position - transform.position).sqrMagnitude <= attackRange * 1.5f;
            bool aboveWaitingRange = (player.transform.position - transform.position).sqrMagnitude >= aggroRangeSqr;
            Vector3 lookDir = (zeroY(player.transform.position - transform.position)).normalized;

            NavMesh.SamplePosition(zeroY(player.transform.position - lookDir * waitingRange), out NavMeshHit navHit, 3f, NavMesh.AllAreas);
            Debug.Log($"W {zeroY(player.transform.position - lookDir * waitingRange)} -- {navHit.position}");
            // DrawDebug(zeroY(player.transform.position - lookDir * waitingRange));
            if (navHit.position != Vector3.negativeInfinity || navHit.position != Vector3.positiveInfinity)
            {
                agent.SetDestination(navHit.position);
            }

            // if (belowWaitingRange && !dodging)
            // {
            //     dodging = true;
            // }
            // else if (aboveWaitingRange)
            // {
            //     //rb.linearVelocity = transform.forward * moveSpeed;
            //     NavMesh.SamplePosition(player.transform.position - lookDir * waitingRange, out NavMeshHit navHit, 3f, NavMesh.AllAreas);
            //     agent.SetDestination(navHit.position);
            //     dodging = false;
            // }
            // else if (dodging)
            // {
            //     lookDir = -1 * lookDir;
            //     rb.linearVelocity = transform.forward * moveSpeed;
            // }
            // else
            // {
            //     rb.linearVelocity = Vector3.zero;
            // }

            rb.rotation = Quaternion.RotateTowards(rb.rotation, Quaternion.LookRotation(lookDir, transform.up), rotateSpeed);
        }
        else if(state == EnemyState.Attacking)
        {
            bool aboveAttackingRange = (player.transform.position - transform.position).sqrMagnitude >= attackRangeSqr;

            if (aboveAttackingRange)
            {
                rb.linearVelocity = transform.forward * moveSpeed;
            }

            Vector3 lookDir = zeroY(player.transform.position) - zeroY(transform.position);
            rb.rotation = Quaternion.RotateTowards(rb.rotation, Quaternion.LookRotation(lookDir, transform.up), rotateSpeed);
        }
    }

    public void Die()
    {
        alive = false;
    }


    private void DrawDebug(Vector3 v)
    {
        Debug.DrawLine(v + Vector3.up * 2f, v + Vector3.down * 2f, Color.blue, 5f);
        Debug.DrawLine(v + Vector3.up + Vector3.right, v + Vector3.up - Vector3.right, Color.blue, 5f);
        Debug.DrawLine(v + Vector3.up + Vector3.forward, v + Vector3.up - Vector3.forward, Color.blue, 5f);
        Debug.DrawLine(v + Vector3.up * 2f + Vector3.right, v + Vector3.down * 2f + Vector3.right, Color.blue, 5f);
        Debug.DrawLine(v + Vector3.up * 2f - Vector3.right, v + Vector3.down * 2f - Vector3.right, Color.blue, 5f);
        Debug.DrawLine(v + Vector3.up * 2f + Vector3.forward, v + Vector3.down * 2f + Vector3.forward, Color.blue, 5f);
        Debug.DrawLine(v + Vector3.up * 2f - Vector3.forward, v + Vector3.down * 2f - Vector3.forward, Color.blue, 5f);
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
