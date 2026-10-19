using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyManager : MonoBehaviour
{
    [SerializeField]
    private int minEnemyAttackers;
    [SerializeField]
    private int maxEnemyAttackers;
    [SerializeField]
    private List<int> spawnEnemyWaves;
    [SerializeField]
    private GameObject enemyPrefab;
    [SerializeField]
    private List<SpawnPoint> spawnPoints;

    public static EnemyManager instance;

    private List<EnemyMovement> enemies = new();
    private List<EnemyMovement> aggroedEnemies = new();
    private List<EnemyMovement> attackingEnemies = new();

    private PlayerMovement player;

    private int currentAttackingEnemies = 1;

    private int totalEnemyCount = 0;
    private int enemiesKilled = 0;
    private int currentEnemyWave = 0;

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
        totalEnemyCount = enemies.Count + spawnEnemyWaves.Sum();
    }

    void Update()
    {
        if (attackingEnemies.Count < currentAttackingEnemies && SceneManager.GetActiveScene().name != "Tutorial")
        {
            currentAttackingEnemies = Mathf.Clamp(currentAttackingEnemies + Mathf.RoundToInt(Random.value * 2f - 1f), minEnemyAttackers, maxEnemyAttackers);

            for (int i = 0; i < (currentAttackingEnemies - attackingEnemies.Count); i++)
            {
                EnemyMovement enemy = aggroedEnemies.OrderBy(x => (x.transform.position - player.transform.position).sqrMagnitude).FirstOrDefault(x => !attackingEnemies.Contains(x));

                if (enemy != null)
                {
                    aggroedEnemies.Remove(enemy);
                    attackingEnemies.Add(enemy);
                }
                else
                {
                    EnemyMovement roamingEnemy = enemies.OrderBy(x => (x.transform.position - player.transform.position).sqrMagnitude).FirstOrDefault(x => !aggroedEnemies.Contains(x));

                    if (roamingEnemy != null)
                    {
                        SetAggroed(roamingEnemy);
                    }
                }
            }
        }

        foreach (EnemyMovement enemy in attackingEnemies )
        {
            enemy.GoAttackMode();
        }

        if (enemies.Count == 0 && currentEnemyWave < spawnEnemyWaves.Count)
        {
            int enemySpawnCount = spawnEnemyWaves[currentEnemyWave];
            var spawners = spawnPoints
                .OrderBy(x => (x.transform.position - player.transform.position).sqrMagnitude)
                .Take(enemySpawnCount)
                .ToList();

            for (int i = 0; i < enemySpawnCount; i++)
            {
                GameObject enemy = Instantiate(enemyPrefab, Vector3.zero, Quaternion.identity, null);
                enemy.SetActive(false);
                spawners[i].Spawn(enemy);
                enemies.Add(enemy.GetComponent<EnemyMovement>());
            }

            currentEnemyWave++;
        }
    }

    public void SetAggroed(EnemyMovement enemy)
    {
        if (aggroedEnemies.Contains(enemy))
        {
            return;
        }

        aggroedEnemies.Add(enemy);
        enemy.GoAggroMode();
        
        if (SceneManager.GetActiveScene().name == "Tutorial")
        {
            enemy.GoAttackMode();
        }
    }

    public void SetKilled(EnemyMovement enemy)
    {
        if (enemies.Contains(enemy))
        {
            enemiesKilled++;
        }

        enemies.Remove(enemy);
        aggroedEnemies.Remove(enemy);
        attackingEnemies.Remove(enemy);

        Debug.Log($"Enemies killed {enemiesKilled} out of {totalEnemyCount}");
        if (enemiesKilled >= totalEnemyCount && !keySpawned)
        {
            var key = Instantiate(keyPrefab);
            key.transform.position = enemy.transform.position;
            keySpawned = true;
        }
    }
}
