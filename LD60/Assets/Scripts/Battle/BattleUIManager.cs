using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleUIManager : MonoBehaviour
{
    public static BattleUIManager instance;

    [SerializeField]
    private Image playerHealthBar;

    [SerializeField]
    private Image enemyHealthBar;

    [SerializeField]
    private TextMeshProUGUI batteryCounter;

    [SerializeField]
    private GameObject enemyContainer;

    private float playerHealth = 10f;
    private float playerMaxHealth = 10f;

    private float enemyHealth = 10f;
    private float enemyMaxHealth = 10f;
    private float enemyDiedTime = 0f;
    private float enemyDeadDelay = 1f;
    private bool showEnemyBar = false;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(instance.gameObject);
        }

        instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        playerHealthBar.fillAmount = playerHealth / playerMaxHealth;
        enemyHealthBar.fillAmount = enemyHealth / enemyMaxHealth;

        if (enemyHealth <= 0)
        {
            if (Time.time - enemyDiedTime > enemyDeadDelay)
            {
                showEnemyBar = false;
            }
        }

        enemyContainer.SetActive(showEnemyBar);
    }

    public void UpdatePlayerHealth(float maxHp, float currentHp)
    {
        playerMaxHealth = maxHp;
        playerHealth = currentHp;
    }

    public void UpdateEnemyHealth(float maxHp, float currentHp)
    {
        enemyMaxHealth = maxHp;
        enemyHealth = currentHp;
        showEnemyBar = true;

        if (enemyHealth <= 0)
        {
            enemyDiedTime = Time.time;
        }
    }

    public void UpdateBatteryCounter(int count)
    {
        batteryCounter.text = count.ToString();
    }
}
