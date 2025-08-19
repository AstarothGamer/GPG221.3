using Sirenix.OdinInspector;
using UnityEngine;
using Resource;

public class GridGenerator : MonoBehaviour
{
    public bool resetOnStart;
    public GameObject tile;
    public GameObject obstacleTile;
    public GameObject treeTile;
    public Vector2Int size;
    public int obstacleChance = 20;
    public int treeChance = 2;

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
        for (int x = -size.x / 2; x < size.x / 2; x++)
        {
            for (int y = -size.y / 2; y < size.y / 2; y++)
            {
                if (Random.Range(0, 100) < obstacleChance)
                {
                    var t = Instantiate(obstacleTile, new Vector3(x, y), Quaternion.identity, transform);
                    t.name = $"Obstacle";
                }
                else if (Random.Range(0, 100) < treeChance)
                {
                    var t = Instantiate(treeTile, new Vector3(x, y), Quaternion.identity, transform);
                    t.name = $"Tree";
                    ResourceManager.Instance.Resources[ResourceType.Wood].Add(t);
                    // Optionally add a tree content script or component here
                    
                }
                else
                {
                    var t = Instantiate(tile, new Vector3(x, y), Quaternion.identity, transform);
                    t.name = $"Ground";
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
