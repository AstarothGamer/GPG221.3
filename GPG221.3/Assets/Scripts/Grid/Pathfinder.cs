using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

public class Pathfinder
{
    // A* Pathfinding
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
                int newMovementCostToNeighbour = currentTile.GCost + GetDistance(currentTile.Original, neighbour) + (neighbour.Discovered ? 0 : 3);

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
        
        do 
        {
            path.Add(currentTile.Original);
            currentTile = currentTile.Parent;
        } 
        while (currentTile?.Parent != null);
        
        if (includeStartTile && currentTile != null)
            path.Add(currentTile.Original);
        
        path.Reverse();
        return path;
    }
    
    public static int GetDistance(Tile t1, Tile t2)
    {
        return Mathf.RoundToInt(Mathf.Abs(t1.position.x - t2.position.x) + Mathf.Abs(t1.position.y - t2.position.y));
    }
    
    
    // Dijkstra to get the nearest tile of a certain type
    public static Tile GetNearestTile(GridManager grid, Tile startTile, Func<Tile, bool> criteria,
        bool ignoreWalkable = false)
    {
        Heap<DijkstraTileData> queue = new(Mathf.CeilToInt(grid.Count * .7f));
        queue.Add(new DijkstraTileData(startTile, 0));
        HashSet<Tile> visited = new() { startTile };

        while (queue.Count > 0)
        {
            var current =  queue.RemoveFirst();
            
            if(criteria.Invoke(current.tile))
                return current.tile;

            foreach (Tile neighbour in grid.GetAdjacentTiles(current.tile.position))
            {
                if (!visited.Contains(neighbour) && (ignoreWalkable || neighbour.IsWalkable))
                {
                    visited.Add(neighbour);
                    queue.Add(new DijkstraTileData(neighbour, current.cost + GetDistance(current.tile, neighbour)));
                }
            }
        }
        return null;
    }
    
    // BFS to get all tiles within a certain range
    public static List<Tile> GetReachableTiles(GridManager grid, Tile startTile, float range, Func<Tile, bool> returnCriteria = null)
    {
        Queue<(Tile tile, float dist)> queue = new();
        queue.Enqueue((startTile, 0));
        HashSet<Tile> visited = new() { startTile };
        List<Tile> inRange = new();

        while (queue.Count > 0)
        {
            var current =  queue.Dequeue();
            if(current.dist > range)
                continue;
            
            if(returnCriteria?.Invoke(current.tile) ?? true)
                inRange.Add(current.tile);

            foreach (Tile neighbour in grid.GetAdjacentTiles(current.tile.position))
            {
                if (!visited.Contains(neighbour) && neighbour.IsWalkable)
                {
                    visited.Add(neighbour);
                    queue.Enqueue((neighbour, current.dist + GetDistance(current.tile, neighbour)));
                }
            }
        }
        return inRange;
    }



    class DijkstraTileData : IHeapItem<DijkstraTileData>
    {
        public Tile tile;
        public float cost;

        public DijkstraTileData(Tile tile, float cost)
        {
            this.tile = tile;
            this.cost = cost;
        }
        
        public int HeapIndex { get; set; }
        public int CompareTo(DijkstraTileData other) 
            => cost.CompareTo(other.cost);
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