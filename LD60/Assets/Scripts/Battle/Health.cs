using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField]
    private ParticleSystem testEffect;

    [SerializeField]
    private float maxHP;

    private float currentHP;
    private EnemyMovement enemyMovement;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentHP = maxHP;
        enemyMovement = GetComponent<EnemyMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        if (currentHP <= 0)
        {
            if (gameObject.layer == LayerMask.NameToLayer("Enemy"))
            {
                EnemyManager.instance.SetKilled(enemyMovement);
            }
            Destroy(gameObject);
        }
    }

    public void TakeDamage(float damage)
    {
        currentHP -= damage;
        testEffect.Play();
    }
}
