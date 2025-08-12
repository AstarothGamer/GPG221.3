using Sirenix.OdinInspector;
using UnityEngine;

public class GridGenerator : MonoBehaviour
{
    public bool resetOnStart;
    public GameObject tile;
    public GameObject obstacleTile;
    public Vector2Int size;
    public int obstacleChance = 20;

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
