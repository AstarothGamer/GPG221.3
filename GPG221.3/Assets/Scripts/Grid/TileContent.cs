using System;
using UnityEngine;

public abstract class TileContent : MonoBehaviour
{
    public abstract bool CanWalkOn { get; }
    public abstract Tile Tile { get; protected set; }

    protected virtual void Start()
    {
        // automatically assign nearest tile if it's not already initialized
        if(!Tile && GridManager.Instance.TryGetTile(transform.position, out Tile t))
            SetTile(t);
    }

    public virtual void SetTile(Tile tile)
    {
        this.Tile = tile;
    }
}