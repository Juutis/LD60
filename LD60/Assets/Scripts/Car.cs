using UnityEngine;

public class Car : MonoBehaviour
{
    public static Car Instance;

    private bool locked = true;

    void Awake()
    {
        Instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Unlock()
    {
        locked = false;
    }

    public void TriggerPlayerNearby()
    {
        if (!locked)
        {
            Debug.Log("Level finished!");
        }
    }
}
