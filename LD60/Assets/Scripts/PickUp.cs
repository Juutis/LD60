using UnityEngine;

public class PickUp : MonoBehaviour
{
    [SerializeField]
    private PickUpType type;

    private Collider coll;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        coll = GetComponent<Collider>();
        coll.enabled = false;
        Invoke("EnablePickup", 1.0f);
    }

    public void EnablePickup()
    {
        coll.enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
    }

    public PickUpType Trigger()
    {
        switch(type)
        {
            case PickUpType.CASSETTE:
                GameManager.main.IncreaseMusics();
                Car.Instance.CasetteAcquired();
                BattleUIManager.instance.UpdateCasetteAcquired();
                break;
            case PickUpType.KEY:
                Car.Instance.KeyAcquired();
                BattleUIManager.instance.UpdateKeyAcquired();
                break;
            case PickUpType.BATTERY:
                GameManager.main.IncreaseBatteries();
                break;
        }
        Destroy(gameObject);
        return type;
    }
}

public enum PickUpType
{
    CASSETTE,
    KEY,
    BATTERY
}
