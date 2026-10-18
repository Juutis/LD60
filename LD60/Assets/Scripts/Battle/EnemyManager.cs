using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager instance;

    private List<EnemyMovement> enemies = new();
    private List<EnemyMovement> aggroedEnemies = new();

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
        
    }

    void Update()
    {
        
    }

    public void SetAggroed(EnemyMovement enemy)
    {
        if (aggroedEnemies.Contains(enemy))
        {
            return;
        }

        aggroedEnemies.Add(enemy);
    }
}
