using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using System.Linq;
using UnityEngine;

public class GridManager : Singleton<GridManager>
{
    [ShowInInspector] Dictionary<Vector2Int, Tile> tiles = new ();
    public int Count => tiles.Count;

    private Tile cachedHoveringTile;
    private float cachedHoveringTileTime;
    
    protected override void Awake()
    {
        base.Awake();
        GetComponentsInChildren<Tile>().ForEach(
            x => AddTile(FixCoords(x.transform.position), x));
    }

    public static Vector2Int FixCoords(Vector3 position) 
        => new Vector2Int(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.y));

    public void AddTile(Vector2Int position, Tile tile)
    {
        if (!tiles.TryAdd(position, tile))
        {
            Debug.Log("the slot " + position + " is already occupied");
            return;
        }

        tile.Initialize(position);
    }
    
    public void Remove(Tile tile, bool destroy = false)
        => Remove(tiles.FirstOrDefault(x => x.Value == tile).Key, destroy);
    public void Remove(Vector2Int position, bool destroy = false)
    {
        if(destroy) 
            Destroy(tiles[position].gameObject);
        tiles.Remove(position);
    }
    
    public void Clear()
    {
        while (tiles.Count > 0)
        {
            var t = tiles.First().Value;
            Remove(t);
            Destroy(t.gameObject);
        }
    }
    
    public bool Contains(Vector2Int position) => tiles.ContainsKey(position);
    public bool Contains(Tile tile) => tiles.ContainsValue(tile);
    
    public IEnumerable<Tile> GetAll() => tiles.Values;
    
    public bool TryGetTile(Vector2Int position, out Tile tile) => tiles.TryGetValue(position, out tile);
    public bool TryGetTile(Vector3 position, out Tile tile) => tiles.TryGetValue(FixCoords(position), out tile);
    public Tile Get(Vector3 position) => Get(FixCoords(position));
    public Tile Get(Vector2Int position) => tiles.GetValueOrDefault(position);
    
    public Tile GetHoveringTile()
    {
        if (Time.time.Equals(cachedHoveringTileTime))
            return cachedHoveringTile;

        var mousePos = Helpers.Camera.ScreenToWorldPoint(Input.mousePosition);
        cachedHoveringTile = Get(mousePos);
        cachedHoveringTileTime = Time.time;
        return cachedHoveringTile;
    }
    
    public List<Tile> GetAdjacentTiles(Vector2Int position)
    {
        List<Tile> neighbors = new List<Tile>();

        if (tiles.TryGetValue(position + Vector2Int.right, out Tile right))
            neighbors.Add(right);
        if (tiles.TryGetValue(position + Vector2Int.left, out Tile left))
            neighbors.Add(left);
        if (tiles.TryGetValue(position + Vector2Int.up, out Tile up))
            neighbors.Add(up);
        if (tiles.TryGetValue(position + Vector2Int.down, out Tile down))
            neighbors.Add(down);

        return neighbors;
    }
    
    public IEnumerable<Tile> GetTilesInCircle(Vector3 center, float range)
    {
        var rangeSquared = range * range;
        for (int x = Mathf.FloorToInt(center.x - range); x < Mathf.CeilToInt(center.x + range); x++)
        {
            for (int y = Mathf.FloorToInt(center.y - range); y < Mathf.CeilToInt(center.y + range); y++)
            {
                var dx = x - center.x;
                var dy = y - center.y;
                
                if (dx * dx + dy * dy < rangeSquared)
                    if(TryGetTile(new Vector2Int(x, y), out Tile tile))
                        yield return tile;
            }
        }
    }
}
