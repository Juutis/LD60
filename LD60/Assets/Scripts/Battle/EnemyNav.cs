using UnityEngine;
using UnityEngine.AI;

public class EnemyNav : MonoBehaviour
{
    private Vector3 target;
    private NavMeshAgent agent;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Vector2 v = Random.onUnitCircle * 3;
        target = new Vector3(v.x, 0f, v.y);
        agent = GetComponent<NavMeshAgent>();
        agent.SetDestination(target);
    }

    // Update is called once per frame
    void Update()
    {
        if ((transform.position - target).sqrMagnitude < 1.5f)
        {
            Debug.Log("Target found");
            Vector2 v = Random.onUnitCircle * 3;
            target = new Vector3(v.x, 0f, v.y);
            agent.SetDestination(target);
        }
    }
}
