using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public enum BuildingType { House, Forge, Tower }

[System.Serializable]

public class BuildingRecipe
{

    public BuildingType type;   //type of building

    public GameObject prefab;  //prefab with sprite

    public int wood;

    public int stone;

    public int steel;

    public int food;


}



//stores recipes and prefabs, selects a suitable cell, transfers resources between the warehouse and the backpack, spawns content on the tile



public class BuildingsGeneralManager : MonoBehaviour
{

    public static BuildingsGeneralManager Instance { get; private set; }

    public int maxBuildings = 5;    //limit of buildings on stage

    public List<BuildingRecipe> recipes = new();

    int builtCount;    //how much has already been built



    void Awake()
    {

        if (Instance && Instance != this) { Destroy(gameObject); return; }

        Instance = this;

    }


    public bool HasSlot => builtCount < maxBuildings;

    BuildingRecipe GetRecipe(BuildingType t) => recipes.FirstOrDefault(r => r != null && r.type == t);  //Raise recipe by type


    public bool CanAfford(Goap.WorldState ws, BuildingType t)   //Are there enough resources in the warehouse for the specified recipe
    {
        var r = GetRecipe(t);

        if (!ws || r == null) return false;

        return ws.wood >= r.wood && ws.stone >= r.stone && ws.steel >= r.steel && ws.food >= r.food;


    }


    public bool WithdrawToBackpack(Goap.WorldState ws, Goap.LocalState ls, BuildingType t)    //Transfer recipe cost from storage to unit's backpack
    {

        var r = GetRecipe(t);

        if (!ws || !ls || r == null) return false;

        if (!CanAfford(ws, t)) return false;

        

        //warehouse tree

        

        ws.wood -= r.wood;   //remove from warehouse
        ls.wood = Mathf.Clamp(ls.wood + r.wood, 0, ls.woodMax);     //put it in the local inventory taking into account the caps

        ws.stone -= r.stone;
        ls.stone = Mathf.Clamp(ls.stone + r.stone, 0, ls.stoneMax);

        ws.steel -= r.steel;
        ls.steel = Mathf.Clamp(ls.steel + r.steel, 0, ls.steelMax);

        ws.food -= r.food;
        ls.food = Mathf.Clamp(ls.food + r.food, 0, ls.foodMax);


        return true;

    }

    public void ConsumeBackpackFor(Goap.LocalState ls, BuildingType t)   //Delete recipe cost from backpack after successful construction
    {

        var r = GetRecipe(t);

        if (!ls || r == null) return;

        ls.wood = Mathf.Max(0, ls.wood - r.wood);

        ls.stone = Mathf.Max(0, ls.stone - r.stone);

        ls.steel = Mathf.Max(0, ls.steel - r.steel);

        ls.food = Mathf.Max(0, ls.food - r.food);




    }

    public Tile PickBuildTileInRange()         //Select a cell for construction within a radius of 20 cells from the warehouse
    {

        if (!BaseWarehouse.Instance || !BaseWarehouse.Instance.entryTile)
        {

            return null;
        }

        var grid = GridManager.Instance;

        if (!grid) return null;



        //var tiles = grid.GetTilesInCircle(BaseWarehouse.Instance.entryTile.transform.position, 20f);

        var tilesEnum = grid.GetTilesInCircle(BaseWarehouse.Instance.entryTile.transform.position, 20f);

        var tiles = tilesEnum as List<Tile> ?? new List<Tile>(tilesEnum);  //make a list to mix it up


        for (int i = 0; i < tiles.Count; i++)
        {

            int j = Random.Range(i, tiles.Count);

            (tiles[i], tiles[j]) = (tiles[j], tiles[i]);

        }
        foreach (var t in tiles)
        {
            if (!t)
            {
                continue;
            }
            if (!t.IsWalkable || !t.CanStandOn)
            {
                continue;
            }
            if (t.content)
            {
                continue;
            }


            return t;

        }

        return null;

    }

    public bool TrySpawn(BuildingType t, Tile tile)     //Spawn a building on the specified cell
    {

        var r = GetRecipe(t);

        if (!HasSlot || r == null || !r.prefab || !tile)
        {

            return false;

        }

        var go = Instantiate(r.prefab);

        var content = go.GetComponent<TileContent>();    //guarantee that the object will have TileContent

        if (!content) content = go.AddComponent<BuildingContent>();

        tile.PlaceContent(content);   //cell binding


        builtCount++;



        return true;
    }


}
