using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainSpawner : MonoBehaviour
{
    public GameObject Terrain;
    public Vector2Int GridSize;
    public Ore[] Ores;
    // Start is called before the first frame update
    void Start()
    {
        for(var i=0; i< Ores.Length; ++i)
        {
            Ores[i].Seed = Random.Range(0f, 1000f);
        }

        float Seed = Random.Range(0f, 1000f);

        for (var y = 0; y < GridSize.y; ++y)
        {
            for(var x =0; x < GridSize.x; ++ x)
            {
                Transform obj = Instantiate(Terrain, new Vector3(32 * x, 0, 32 * y), Quaternion.identity).transform;

                obj.transform.parent = this.transform;
                //obj.transform.GetComponent<TerrainGeneration>().Offset = new Vector2Int(x * 16, y * 16);
                obj.transform.GetComponent<TerrainGeneration>().Ores = Ores;
                obj.transform.GetComponent<TerrainGeneration>().Seed = Seed;
                obj.transform.GetComponent<TerrainGeneration>().Initiate();
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
