using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [SerializeField]
    private int minEnemyAttackers;
    [SerializeField]
    private int maxEnemyAttackers;

    public static EnemyManager instance;

    private List<EnemyMovement> enemies = new();
    private List<EnemyMovement> aggroedEnemies = new();
    private List<EnemyMovement> attackingEnemies = new();

    private PlayerMovement player;

    private Dictionary<int, List<Vector3>> enemyFormations = new() {
        { 0, new() { Vector3.forward, -Vector3.forward } },
        { 1, new() { Vector3.forward, Vector3.right, -Vector3.right } },
        { 2, new() { Vector3.right, -Vector3.right } }
    };

    private List<Vector3> currentFormation = new();
    private int currentAttackingEnemies = 1;

    [SerializeField]
    private GameObject keyPrefab;
    private bool keySpawned = false;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(instance.gameObject);
        }

        instance = this;
    }

    void Start()
    {
        player = FindAnyObjectByType<PlayerMovement>();
        enemies = FindObjectsByType<EnemyMovement>().ToList();
    }

    void Update()
    {
        if (attackingEnemies.Count < currentAttackingEnemies)
        {
            currentAttackingEnemies = Mathf.Clamp(currentAttackingEnemies + Mathf.RoundToInt(Random.value * 2f - 1f), minEnemyAttackers, maxEnemyAttackers);

            for (int i = 0; i < (currentAttackingEnemies - attackingEnemies.Count); i++)
            {
                EnemyMovement enemy = aggroedEnemies.FirstOrDefault(x => !attackingEnemies.Contains(x));

                if (enemy != null)
                {
                    aggroedEnemies.Remove(enemy);
                    attackingEnemies.Add(enemy);
                }
                else
                {
                    EnemyMovement roamingEnemy = enemies.FirstOrDefault(x => !aggroedEnemies.Contains(x));

                    if (roamingEnemy != null)
                    {
                        aggroedEnemies.Add(roamingEnemy);
                        roamingEnemy.GoAggroMode(); 
                    }
                }
            }
        }

        foreach (EnemyMovement enemy in attackingEnemies )
        {
            enemy.GoAttackMode();
        }
    }

    public void SetAggroed(EnemyMovement enemy)
    {
        if (aggroedEnemies.Contains(enemy))
        {
            return;
        }

        aggroedEnemies.Add(enemy);
    }

    public void SetKilled(EnemyMovement enemy)
    {
        enemies.Remove(enemy);
        aggroedEnemies.Remove(enemy);
        attackingEnemies.Remove(enemy);
        if (enemies.Count == 0 && !keySpawned)
        {
            var key = Instantiate(keyPrefab);
            key.transform.position = enemy.transform.position;
            keySpawned = true;
        }
    }
}
