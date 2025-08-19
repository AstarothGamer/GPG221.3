using UnityEngine;

public class BaseWarehouse : MonoBehaviour
{
    public static BaseWarehouse Instance { get; private set; }

    public Tile entryTile;

    private void Awake()
    {
        if (Instance && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }
}
