using Sirenix.OdinInspector;
using Sirenix.Utilities;
using System.Collections.Generic;
using UnityEngine;

public class TileDisplayManager : MonoBehaviour
{
    [SerializeField] Color SelectedTileColor;
    [SerializeField] Color movementRangeColor;
    [SerializeField] Color movementRangeNoMovesColor;
    [SerializeField] Color errorHoveringTileColor;
    [SerializeField] Color attackRangeColor;
    [SerializeField] Color attackRangeNoAttacksColor;
    [SerializeField] Color enemyTileColor;
    [SerializeField] Color allyTileColor;
    [SerializeField] Color allyNoMovesColor;

    /*
    private Tile hoveredTileLastFrame;
    private Tile lastSelectedTile;

    bool forceUpdateTiles;

    bool changedSelectedTile;
    bool changedHoveringTile;
    List<Tile> cachedWalkableTiles = new();
    List<Tile> cachedAttackableTiles = new();
    List<Tile> cachedPath = new();

    public void ForceUpdateTilesNextFrame() => forceUpdateTiles = true;

    
    void Update()
    {
        Unit selectedUnit = UnitManager.Instance.SelectedUnit;
        changedSelectedTile = lastSelectedTile != selectedUnit?.currentTile;
        lastSelectedTile = selectedUnit?.currentTile;
        
        Tile hovering = Helpers.IsOverUI ? null : GridManager.Instance.Get(Helpers.Camera.ScreenToWorldPoint(Input.mousePosition));
        changedHoveringTile = hoveredTileLastFrame != hovering;
        hoveredTileLastFrame = hovering;

        if (!changedSelectedTile && !changedHoveringTile && !forceUpdateTiles)
            return;
        forceUpdateTiles = false;

        if (changedSelectedTile && selectedUnit?.currentTile)
            cachedWalkableTiles = selectedUnit.GetWalkableTiles();
        
        GridManager.Instance.GetAll().ForEach(x => x.gfx.ResetGraphics());

        ColorUnits();

        if(selectedUnit)
            PreviewSelecetdUnit(selectedUnit, hovering);

    }

    void PreviewSelecetdUnit(Unit selectedUnit, Tile hovering)
    {
        Tile selectedTile = selectedUnit?.currentTile;

        if (!selectedUnit || !selectedTile)
            return;

        selectedTile.gfx.SetOuterBorderColor(SelectedTileColor);

        bool canMove = selectedUnit.movesAvailable > 0;

        //Draw walkable tiles
        cachedWalkableTiles.ForEach(x => x.gfx.SetBorderColor(canMove ? movementRangeColor : movementRangeNoMovesColor));

        //If not hovering a tile, draw attack range around the unit
        if (!hovering)
        {
            if (selectedTile)
                PreviewAttack(selectedUnit, selectedTile, !changedSelectedTile);
            return;
        }

        //Walk and attack preview
        if (cachedWalkableTiles.Contains(hovering) && canMove)
        {
            if(changedHoveringTile || changedSelectedTile)
            {
                cachedPath = Pathfinder.FindPath(GridManager.Instance, selectedTile, hovering);
                cachedPath.Insert(0, selectedTile);
            }

            PreviewPath(cachedPath);
            hovering.gfx.SetBorderColor(SelectedTileColor);
            hovering.gfx.SetOuterBorderColor(SelectedTileColor);
            hovering.gfx.SetPathDestination();

            PreviewAttack(selectedUnit, hovering, !changedHoveringTile);
            return;
        }
        
        PreviewAttack(selectedUnit, selectedTile, !changedSelectedTile && !changedHoveringTile);

        //hovering ally unit
        if (hovering.unit && !hovering.unit.isEnemy && (hovering != selectedTile))
        {
            hovering.gfx.SetOuterBorderColor(allyTileColor);
        }
        //hovering attackable enemy
        if (hovering.unit && hovering.unit.isEnemy && cachedAttackableTiles.Contains(hovering))
        {
            hovering.gfx.SetBorderColor(new Color(0, 0, 0, 0));
            hovering.gfx.SetAimingColor(errorHoveringTileColor);
        }
        //hovering uninteractable slot
        else if (hovering != selectedTile && hovering.unit == null)
        { 
            hovering.gfx.SetFillColor(errorHoveringTileColor);
            hovering.gfx.SetBorderColor(errorHoveringTileColor);
        }
    }


    void ColorUnits()
    {
        foreach (var unit in UnitManager.Instance.Units)
        {
            if (unit.isEnemy)
                unit.currentTile?.gfx.SetBorderColor(enemyTileColor);
            else
                unit.currentTile?.gfx.SetBorderColor(unit.CanDoAction() ? allyTileColor : allyNoMovesColor);
        }
    }


    void PreviewAttack(Unit unit, Tile center, bool useCache)
    {
        bool canAttack = unit.attacksAvailable > 0;
        Color color = canAttack ? attackRangeColor : attackRangeNoAttacksColor;

        if(!useCache)
            cachedAttackableTiles = unit.GetAttackableTiles(center);

        cachedAttackableTiles.ForEach(x => { 
            if(x.unit && x.unit.isEnemy)
                x.gfx.SetBorderColor(color);
            x.gfx.SetInnerBorderColor(color); });
    }



    void PreviewPath(List<Tile> path)
    {
        for (int i = 1; i < path.Count - 1; i++)
        {
            Vector2Int previous = path[Mathf.Max(i - 1, 0)].position - path[i].position;
            Vector2Int next = path[Mathf.Min(i + 1, path.Count - 1)].position - path[i].position;

            path[i].gfx.SetPathPreview(previous, next);
            if (path[i].unit == null)
                path[i].gfx.SetBorderColor(SelectedTileColor);
        }
    }*/
}
