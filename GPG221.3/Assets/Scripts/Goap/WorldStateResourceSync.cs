using Goap;
using UnityEngine;
using static Resource.Resource;
using Resource;
public class WorldStateResourceSync : MonoBehaviour
{
    [SerializeField] private WorldState worldState;

    private void OnEnable()
    {
        if (!worldState) worldState = GetComponent<WorldState>();
        OnResourceDepleted += HandleDepleted;
    }

    private void OnDisable()
    {
        OnResourceDepleted -= HandleDepleted;
    }

    private void HandleDepleted(Tile tile, ResourceType type)
    {
        if (!tile || !worldState) return;

        switch (type)
        {
            case ResourceType.Wood:  worldState.knownWoodTiles.Remove(tile);  break;
            case ResourceType.Stone: worldState.knownStoneTiles.Remove(tile); break;
            case ResourceType.Steel: worldState.knownSteelTiles.Remove(tile); break;
            case ResourceType.Food:  worldState.knownFoodTiles.Remove(tile);  break;
        }
    }
}