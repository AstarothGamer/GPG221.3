using NPC;
using Sirenix.OdinInspector;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public Vector2Int position;
    
    [SerializeField] protected Transform contentParent;
    public TileGFX gfx;
    
    public Unit unit;
    public TileContent content;
    
    [ShowInInspector] public bool IsWalkable => !discovered || (isWalkable && (!content || content.CanWalkOn));
    [ShowInInspector] public virtual bool CanStandOn => !discovered || (IsWalkable && !unit);
    public bool Discovered => discovered; 
    
    [SerializeField] protected bool isWalkable = true;
    [ShowInInspector, ReadOnly] protected bool discovered;
    
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

    public void Discover()
    {
        discovered = true;
    }
    
    public void PlaceContent(TileContent content)
    {
        content.transform.SetParent(contentParent);
        content.transform.localPosition = Vector3.zero;
        content.SetTile(this);
    }
    
    public void SetUnit(Unit unit)
    {
        if (!unit)
        {
            this.unit = null;
            return;
        }
        
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

