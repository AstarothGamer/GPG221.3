using Sirenix.OdinInspector;
using UnityEngine;
using Resource;
using Resource;
using UnityEngine.Serialization;

public class GridGenerator : MonoBehaviour
{
    
    [Header("Grid Settings")]
    public Vector2Int size;
    public bool resetOnStart;
    
    [Header("Tiles")]
    public GameObject tile;
    public GameObject obstacleTile;
    
    [Header("Resources")]
    public GameObject treePrefab;
    public GameObject stonePrefab;
    public GameObject steelPrefab;
    public GameObject foodPrefab;
    public GameObject warehousePrefab;
    
    [Header("Resource Chances")]
    public int treeChance = 2;
    public int stoneChance = 3;
    public int steelChance = 1;
    public int foodChance = 5;
    
    
    

    private void Awake()
    {
        if (resetOnStart)
            ResetGrid();
    }

    [Button]
    public void ResetGrid()
    {
        ClearChildren();
        CreateGrid();
    }

    [Button]
    public void CreateGrid()
    {
        var house = Instantiate(warehousePrefab, new Vector3(Random.Range(0,5),Random.Range(0,5),0), Quaternion.identity, transform);
        for (int x = -size.x / 2; x < size.x / 2; x++)
        {
            for (int y = -size.y / 2; y < size.y / 2; y++)
            {
                var t = Instantiate(tile, new Vector3(x, y), Quaternion.identity, transform);
                t.name = $"Ground";

                GameObject prefab = null;
                if (Random.Range(0, 100) < treeChance)
                {
                    prefab = treePrefab;
                }
                else if (Random.Range(0, 100) < stoneChance)
                {
                    prefab = stonePrefab;
                }
                else if (Random.Range(0, 100) < steelChance)
                {
                    prefab = steelPrefab;
                }
                else if (Random.Range(0, 100) < foodChance)
                {
                    prefab = foodPrefab;
                }

                if (prefab != null)
                {
                    var instantiatedResource = Instantiate(prefab, new Vector3(x, y), Quaternion.identity, transform);
                    ResourceManager.Instance.AddResource(instantiatedResource.GetComponent<Resource.Resource>().resourceType, instantiatedResource);
                }

            }
        }
    }
    [Title("clear")]
    [Button]
    public void ClearChildren()
    {
        while(transform.childCount > 0)
        {
            DestroyImmediate(transform.GetChild(0).gameObject);
        }
    }
}
