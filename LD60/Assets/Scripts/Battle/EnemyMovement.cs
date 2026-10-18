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
    [SerializeField]
    private float attackMinDelay;
    [SerializeField]
    private float attackMaxDelay;
    [SerializeField]
    private string attackName;

    private float lastHit = 0f;
    private float currentAttackDelay = 0f;
    private bool hitting = false;

    private Rigidbody rb;
    private NavMeshAgent agent;

    private EnemyState state = EnemyState.Roaming;
    private Vector3 roamingTarget;
    private Vector3 roamingLookDir;
    private PlayerMovement player;
    private float aggroRangeSqr;
    private float waitingRangeSqr;
    private float attackRangeSqr;
    private bool dodging = false;
    private bool alive = true;
    private Collider coll;
    private BattleAnimator battleAnim;

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
        coll = GetComponent<Collider>();
        battleAnim = GetComponentInChildren<BattleAnimator>();
        currentAttackDelay = attackMaxDelay;
    }

    // Update is called once per frame
    void Update()
    {
        if (!alive)
        {
            rb.linearVelocity = Vector3.zero;
            agent.enabled = false;
            GetComponent<CapsuleCollider>().enabled = false;
            return;
        }
        if (state == EnemyState.Roaming)
        {
            if ((roamingTarget - transform.position).sqrMagnitude <= 2f)
            {
                Vector2 t2 = Random.onUnitCircle * 4f;
                roamingTarget = transform.position + new Vector3(t2.x, 0f, t2.y);
                NavMesh.SamplePosition(roamingTarget, out NavMeshHit navHit, 3f, NavMesh.AllAreas);
                roamingLookDir = zeroY(roamingTarget) - zeroY(transform.position);
                roamingTarget = navHit.position;
                agent.SetDestination(navHit.position);
                Debug.Log(navHit.position);
            }

            Vector3 newLookDir = zeroY(roamingTarget) - zeroY(transform.position);

            if (newLookDir != Vector3.zero)
            {
                roamingLookDir = newLookDir;
            }

            // rb.linearVelocity = transform.forward * moveSpeed;
            rb.rotation = Quaternion.RotateTowards(rb.rotation, Quaternion.LookRotation(roamingLookDir, transform.up), rotateSpeed);

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
        else if (state == EnemyState.Waiting)
        {
            bool belowWaitingRange = (player.transform.position - transform.position).sqrMagnitude <= attackRange * 1.5f;
            bool aboveWaitingRange = (player.transform.position - transform.position).sqrMagnitude >= aggroRangeSqr;
            Vector3 lookDir = (zeroY(player.transform.position - transform.position)).normalized;

            NavMesh.SamplePosition(zeroY(player.transform.position - lookDir * waitingRange), out NavMeshHit navHit, 3f, NavMesh.AllAreas);
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
        else if (state == EnemyState.Attacking)
        {
            bool aboveAttackingRange = (player.transform.position - transform.position).sqrMagnitude > attackRangeSqr;

            Vector3 lookDir = (zeroY(player.transform.position - transform.position)).normalized;
            rb.rotation = Quaternion.RotateTowards(rb.rotation, Quaternion.LookRotation(lookDir, transform.up), rotateSpeed);
            if (aboveAttackingRange)
            {
                NavMesh.SamplePosition(zeroY(player.transform.position - lookDir * attackRange), out NavMeshHit navHit, 3f, NavMesh.AllAreas);
                agent.SetDestination(navHit.position);
            }
            else
            {
                if (Time.time - lastHit > currentAttackDelay && !hitting)
                {
                    // hit
                    Debug.Log("HIT");
                    battleAnim.PlayState(attackName, true);
                    hitting = true;
                }
            }
        }

        if (!hitting)
        {
            if (agent.velocity.magnitude > 0.1f)
            {
                battleAnim.PlayState("Run");
            }
            else
            {
                battleAnim.PlayState("Idle");
            }
        }
    }

    public void Die()
    {
        alive = false;
        coll.enabled = false;
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
        if (state != EnemyState.Attacking)
        {
            state = EnemyState.Attacking;
            lastHit = Time.time;
            currentAttackDelay = GetAttackDelay();
        }
    }

    public void GoAggroMode()
    {
        state = EnemyState.Aggro;
    }

    private float GetAttackDelay()
    {
        return attackMinDelay + Random.value * (attackMaxDelay - attackMinDelay);
    }

    public void AttackHit()
    {
        Vector3 hitCastDir = transform.forward;
        Vector3 hitCastStart = transform.position + 0.5f * Vector3.up + hitCastDir * 0.85f;
        float sphereRadius = 0.5f;

        // Physics.Raycast(hitCastStart, hitCastDir, out RaycastHit hit, 4f, LayerMask.GetMask("Enemy"));
        Collider[] cols = Physics.OverlapSphere(hitCastStart, sphereRadius, LayerMask.GetMask("Player"));
        Debug.DrawLine(hitCastStart + Vector3.up, hitCastStart - Vector3.up, Color.magenta, 5f);
        Debug.DrawLine(hitCastStart + Vector3.up + Vector3.forward * 0.2f, hitCastStart + Vector3.up - Vector3.forward * 0.2f, Color.magenta, 5f);
        Debug.DrawLine(hitCastStart + Vector3.up + Vector3.right * 0.2f, hitCastStart + Vector3.up - Vector3.right * 0.2f, Color.magenta, 5f);

        foreach (Collider col in cols)
        {
            if (col != null && col.TryGetComponent(out Health playerHealth))
            {
                Debug.Log("Hit!");
                playerHealth.TakeDamage(1);
            }
        }
    }

    public void AttackDone()
    {
        Debug.Log("Attack done");
        lastHit = Time.time;
        hitting = false;
        currentAttackDelay = GetAttackDelay();
    }
}

public enum EnemyState
{
    Roaming,
    Aggro,
    Waiting,
    Attacking,
}
