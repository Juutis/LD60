using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField]
    private ParticleSystem testEffect;

    [SerializeField]
    private float maxHP;

    private float currentHP;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentHP = maxHP;
    }

    // Update is called once per frame
    void Update()
    {
        if (currentHP <= 0)
        {
            Destroy(gameObject);
        }
    }

    public void TakeDamage(float damage)
    {
        currentHP -= damage;
        testEffect.Play();
    }
}
