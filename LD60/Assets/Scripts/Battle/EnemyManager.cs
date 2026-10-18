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
                    attackingEnemies.Add(enemy);
                    enemy.GoAttackMode();
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
    }
}
