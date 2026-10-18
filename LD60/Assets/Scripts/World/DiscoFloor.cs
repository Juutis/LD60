using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DiscoFloor : MonoBehaviour
{
    [SerializeField]
    private List<Material> materials;

    private List<Renderer> renderers =new List<Renderer>();

    private float interval = 0.5f;

    [SerializeField]
    private int width = 3;

    [SerializeField]
    private int length = 3;

    [SerializeField]
    private Renderer tilePrefab;

    private float tileSize = 1.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        generateFloor();
        randomizeTiles();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void generateFloor()
    {
        for(var i = 0; i < width; i++)
        {
            for (var j = 0; j < length; j++)
            {
                var tile = Instantiate(tilePrefab, transform);
                var offSet = new Vector3(i * tileSize, 0, j * tileSize);
                tile.transform.position = transform.position + offSet;
                renderers.Add(tile);
            }
        }
    }

    private void randomizeTiles()
    {
        renderers.ForEach(it => randomizeTile(it));
        Invoke("randomizeTiles", interval);
    }

    private void randomizeTile(Renderer rend)
    {
        var i = Random.Range(0, materials.Count);
        rend.material = materials[i];
    }
}
