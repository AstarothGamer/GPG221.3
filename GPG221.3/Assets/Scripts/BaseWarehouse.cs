using Goap;
using UnityEngine;

public class BaseWarehouse : MonoBehaviour
{
    public static BaseWarehouse Instance;
    public Tile entryTile;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
