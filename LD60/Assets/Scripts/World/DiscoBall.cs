using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DiscoBall : MonoBehaviour
{
    private float interval = 0.5f;

    [SerializeField]
    private GameObject rayParent;

    [SerializeField]
    private List<Material> materials;

    private List<Renderer> renderers =new List<Renderer>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        renderers = rayParent.GetComponentsInChildren<Renderer>().Where(it => it.name != "Sphere").ToList();
        randomizeMaterials();
    }

    // Update is called once per frame
    void Update()
    {
        var dir = new Vector3(Mathf.Sin(Time.time), Mathf.Cos(Time.time), Mathf.Sin(Time.time * 2.312f));
        transform.Rotate(dir, Time.deltaTime * 60);
    }
    
    private void randomizeMaterials()
    {
        renderers.ForEach(it => randomizeMaterial(it));
        Invoke("randomizeMaterials", interval);
    }

    private void randomizeMaterial(Renderer rend)
    {
        var i = Random.Range(0, materials.Count);
        rend.material = materials[i];
    }
}
