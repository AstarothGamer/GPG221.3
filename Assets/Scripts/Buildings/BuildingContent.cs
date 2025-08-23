using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingContent : TileContent
{
    //Content of the building cell

    //You can't walk on it. The cell is fixed.

    public override bool CanWalkOn => false;

    public override Tile Tile { get; protected set; }

    public override void SetTile(Tile tile)
    {

        base.SetTile(tile);

        Tile = tile;

    }




    void Update()
    {
        

    }



}
