using NPC;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public Vector2Int position;
    public bool IsWalkable = true;
    public virtual bool CanStandOn => IsWalkable && !unit;
    public TileGFX gfx;

    [SerializeField] protected Transform contentParent;
    public Unit unit;
    
    private void Awake()
    {
        if(contentParent == null)
            contentParent = transform.Find("Content");
        gfx = GetComponent<TileGFX>();
    }

    public virtual void Initialize(Vector2Int pos)
    {
        this.position = pos;
    }

    private void OnDestroy()
    {
        GridManager.Instance?.Remove(this);
    }

    public void PlaceUnit(Unit unit)
    {
        if (!unit) return;
        if (this.unit && this.unit != unit)
        {
            Debug.LogError("Tile is already occupied!");
            return;
        }

        this.unit = unit;
        this.unit.transform.parent = contentParent;
        this.unit.transform.position = contentParent.position;
        unit.currentTile = this;
    }
}

