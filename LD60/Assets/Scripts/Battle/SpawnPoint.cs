using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    [SerializeField]
    private ParticleSystem beamParticles;
    [SerializeField]
    private GameObject tempVisualAid;

    private GameObject enemy;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Destroy(tempVisualAid);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Spawn(GameObject e)
    {
        if (e == null)
        {
            return;
        }

        enemy = e;
        beamParticles.Play();
        Debug.Log("Enemy spawn started");
        Invoke(nameof(ActivateEnemy), 2f);
        Vector3 pos = transform.position;
        enemy.transform.position = new Vector3(pos.x, 0f, pos.z);
    }

    private void ActivateEnemy()
    {
        if (enemy != null)
        {
            Debug.Log("Enemy spawned");
            enemy.SetActive(true);
            enemy = null;
        }
    }
}
