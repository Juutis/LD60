using UnityEngine;

public class PickUp : MonoBehaviour
{
    [SerializeField]
    private PickUpType type;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Trigger()
    {
        switch(type)
        {
            case PickUpType.CASSETTE:
                break;
            case PickUpType.KEY:
                Car.Instance.Unlock();
                break;
            case PickUpType.BATTERY:
                break;
        }
        Destroy(gameObject);
    }
}

public enum PickUpType
{
    CASSETTE,
    KEY,
    BATTERY
}
