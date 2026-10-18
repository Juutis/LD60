using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField]
    private ParticleSystem testEffect;

    [SerializeField]
    private float maxHP;

    private float currentHP;
    private EnemyMovement enemyMovement;
    private PlayerMovement playerMovement;
    private BattleAnimator anim;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentHP = maxHP;
        enemyMovement = GetComponent<EnemyMovement>();
        playerMovement = GetComponent<PlayerMovement>();
        anim = GetComponentInChildren<BattleAnimator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (currentHP <= 0)
        {
            if (gameObject.layer == LayerMask.NameToLayer("Enemy"))
            {
                EnemyManager.instance.SetKilled(enemyMovement);
                enemyMovement.Die();
            }
            else
            {
                playerMovement.Die();
            }
            anim.Die();
        }
    }

    public void TakeDamage(float damage)
    {
        currentHP -= damage;
        testEffect.Play();

        if (gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            anim.Hurt();
            enemyMovement.GoAttackMode();
            BattleUIManager.instance.UpdateEnemyHealth(maxHP, currentHP);
        }
        else if (playerMovement != null)
        {
            BattleUIManager.instance.UpdatePlayerHealth(maxHP, currentHP);
        }
    }
}
