using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.Utilities;
using UnityEngine;


public class GridManager : Singleton<GridManager>
{
    [ShowInInspector] Dictionary<Vector2Int, Tile> tiles = new ();

    public int Count => tiles.Count;


    protected override void Awake()
    {
        base.Awake();
        GetComponentsInChildren<Tile>().ForEach(
            x => AddTile(FixCoordinates(x.transform.position), x));
    }


    public void AddTile(Vector2Int position, Tile tile)
    {
        if (!tiles.TryAdd(position, tile))
        {
            Debug.Log("the slot " + position + " is already occupied");
            return;
        }

        tile.Initialize(position);
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


    public static Vector2Int FixCoordinates(Vector3 position) => new Vector2Int(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.y));
    public Tile Get(Vector3 position) => Get(FixCoordinates(position));
    public Tile Get(Vector2Int position)
    {
        if (tiles.ContainsKey(position))
            return tiles[position];
        return null;
    }

    public IEnumerable<Tile> GetAll() => tiles.Values;

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

    Tile cachedHoveringTile;
    float cachedHoveringTileTime;
    public Tile GetHoveringTile()
    {
        if (Time.time.Equals(cachedHoveringTileTime))
            return cachedHoveringTile;

        var mousePos = Helpers.Camera.ScreenToWorldPoint(Input.mousePosition);
        cachedHoveringTile = Get(mousePos);
        cachedHoveringTileTime = Time.time;
        return cachedHoveringTile;
    }
}
