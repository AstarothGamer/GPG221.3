using System.Collections.Generic;
using UnityEngine;

namespace Goap
{
    public class WorldState : MonoBehaviour
    {
        public List<Effect> receivedEffects = new();

        public int wood;
        public int stone;
        public int steel;
        public int food;
        
        public List<Tile> knownWoodTiles  = new();
        public List<Tile> knownStoneTiles = new();
        public List<Tile> knownSteelTiles = new();
        public List<Tile> knownFoodTiles  = new();

        public static string KnownFactName(Resource.ResourceType t) => $"Known_{t}";

        public void RegisterResourceTile(Resource.ResourceType type, Tile tile)
        {
            if (!tile || !tile.Discovered) return;

            List<Tile> list = type switch
            {
                Resource.ResourceType.Wood  => knownWoodTiles,
                Resource.ResourceType.Stone => knownStoneTiles,
                Resource.ResourceType.Steel => knownSteelTiles,
                Resource.ResourceType.Food  => knownFoodTiles,
                _ => null
            };
            if (list == null) return;
            if (!list.Contains(tile)) list.Add(tile);

            EnsureKnownFact(type); // ставим флаг Known_{Type}
        }

        public void RegisterResourceTile(Resource.Resource res)
        {
            if (res == null || res.Tile == null) return;
            RegisterResourceTile(res.resourceType, res.Tile);
        }

        public void EnsureKnownFact(Resource.ResourceType type)
        {
            string fact = KnownFactName(type);
            foreach (var e in receivedEffects)
                if (e != null && e.kind == EffectKind.Named && e.name == fact)
                    return;

            receivedEffects.Add(new Effect { kind = EffectKind.Named, name = fact });
        }

        public bool IsKnownResourceTile(Resource.ResourceType type, Tile tile)
        {
            if (!tile) return false;
            return type switch
            {
                Resource.ResourceType.Wood  => knownWoodTiles.Contains(tile),
                Resource.ResourceType.Stone => knownStoneTiles.Contains(tile),
                Resource.ResourceType.Steel => knownSteelTiles.Contains(tile),
                Resource.ResourceType.Food  => knownFoodTiles.Contains(tile),
                _ => false
            };
        }
    }
}
