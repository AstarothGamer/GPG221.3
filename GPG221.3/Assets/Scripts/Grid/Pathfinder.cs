using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Pathfinder
{

    public static List<Tile> FindPath(GridManager grid, Tile startTile, Tile endTile, bool includeStartTile = false, bool stopOneTileEarly = false)
    {
        Dictionary<Tile, PathfinderTileData> tileData = new();
        Heap<PathfinderTileData> openSet = new (grid.Count);
        HashSet<Tile> closedSet = new HashSet<Tile>();

        tileData.Add(startTile, new PathfinderTileData(startTile, 0, GetDistance(startTile, endTile), null));
        openSet.Add(tileData[startTile]);

        while (openSet.Count > 0)
        {
            var currentTile = openSet.RemoveFirst();
            closedSet.Add(currentTile.Original);

            if (currentTile.Original == endTile)
                    return RetracePath(currentTile, includeStartTile);
            
            foreach (Tile neighbour in grid.GetAdjacentTiles(currentTile.Original.position))
            {
                if(stopOneTileEarly && neighbour == endTile)
                    return RetracePath(currentTile, includeStartTile);
                
                if (!neighbour || !neighbour.IsWalkable || closedSet.Contains(neighbour))
                    continue;
                int newMovementCostToNeighbour = currentTile.GCost + GetDistance(currentTile.Original, neighbour);

                if (!tileData.ContainsKey(neighbour))
                {
                    tileData.Add(neighbour, new PathfinderTileData(neighbour, newMovementCostToNeighbour, GetDistance(neighbour, endTile), currentTile));
                    openSet.Add(tileData[neighbour]);
                }

                if (newMovementCostToNeighbour < tileData[neighbour].GCost)
                {
                    tileData[neighbour].GCost = newMovementCostToNeighbour;
                    tileData[neighbour].Parent = currentTile;
                }
            }
        }

        return null;
    }

    private static List<Tile> RetracePath(PathfinderTileData endTile, bool includeStartTile)
    {
        List<Tile> path = new List<Tile>();
        PathfinderTileData currentTile = endTile;
        
        while (currentTile.Parent != null || currentTile == endTile)
        {
            path.Add(currentTile.Original);
            currentTile = currentTile.Parent;
        }
        if (includeStartTile)
            path.Add(currentTile.Original);
        path.Reverse();
        return path;
    }
    
    public static int GetDistance(Tile t1, Tile t2)
    {
        return Mathf.RoundToInt(Mathf.Abs(t1.position.x - t2.position.x) + Mathf.Abs(t1.position.y - t2.position.y));
    }

    
    class PathfinderTileData : IHeapItem<PathfinderTileData>
    {
        public int HeapIndex { get; set; }
        public int GCost;
        public int HCost;
        public int FCost => GCost + HCost;

        public readonly Tile Original;
        public PathfinderTileData Parent;

        public PathfinderTileData(Tile tile, int gCost, int hCost, PathfinderTileData parent)
        {
            Original = tile;
            this.GCost = gCost;
            this.HCost = hCost;
            this.Parent = parent;
        }
        
        public int CompareTo(PathfinderTileData other)
        {
            int compare = FCost.CompareTo(other.FCost);
            if (compare == 0)
                compare = HCost.CompareTo(other.HCost);
            return -compare;
        }
    }
}