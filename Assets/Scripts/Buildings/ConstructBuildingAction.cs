using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NPC;
using Goap;




public class ConstructBuildingAction : Action
{

    FollowPathMovement mover; //path motion component

    Unit unit;      //unit for speed and current tile

    BuildBlackboard bb;   //what is building

    GridManager grid => GridManager.Instance;


    protected override void Awake()
    {

        base.Awake();

        mover = GetComponent<FollowPathMovement>();

        unit = GetComponent<Unit>();

        bb = GetComponent<BuildBlackboard>() ?? gameObject.AddComponent<BuildBlackboard>();


        //prerequisits.RemoveAll(p => p == null);

        //effects.RemoveAll(e => e == null);

        //AddNamedPrereq("HasConstructionPlan");

        //AddNamedPrereq("HasBuildingMaterials");

        //prerequisits.RemoveAll(p => p && p.kind == Prerequisite.Kind.Named && p.name != "HasConstructionPlan" && p.name != "HasBuildingMaterials");

        //var eff = effects.Find(e => e && e.kind == Effect.Kind.Named && e.name == "ConstructedBuilding");

        //if (!eff)
        //{

        //    eff = gameObject.AddComponent<Effect>();

        //    eff.kind = Effect.Kind.Named;

        //    eff.effectName = "ConstructedBuilding";

        //    eff.name = "ConstructedBuilding";

        //    effects.Add(eff);


        //}

        //effects.RemoveAll(e => e && e.kind == Effect.Kind.Named && e.name != "ConstructedBuilding");


        //void AddNamedPrereq(string n)
        //{

        //    var p = prerequisits.Find(x => x && x.kind == Prerequisite.Kind.Named && x.name == n);

        //    if (!p)
        //    {

        //        p = gameObject.AddComponent<Prerequisite>();

        //        p.kind = Prerequisite.Kind.Named;

        //        p.effectName = n;

        //        p.name = n;

        //        prerequisits.Add(p);

        //    }

        //}


        //Prerequisites plan and materials should already be received

        /////////////////////////////

        bool hasPlan = false, hasMats = false;



        foreach (var p in prerequisits)
        {

            if (p && p.kind == Prerequisite.Kind.Named && p.effectName == "HasConstructionPlan")
            {

                hasPlan = true;

            }
            if (p && p.kind == Prerequisite.Kind.Named && p.effectName == "HasBuildingMaterials")
            {

                hasMats = true;

            }

        }
        if (!hasPlan)
        {

            var pre = gameObject.AddComponent<Prerequisite>();

            pre.kind = Prerequisite.Kind.Named;

            pre.effectName = "HasConstructionPlan";

           

            prerequisits.Add(pre);



        }
        if (!hasMats)
        {

            var pre2 = gameObject.AddComponent<Prerequisite>();

            pre2.kind = Prerequisite.Kind.Named;

            pre2.effectName = "HasBuildingMaterials";

           // pre2.name = "HasBuildingMaterials";

            prerequisits.Add(pre2);




        }


        //effect planner can set Goal


        bool hasGoal = false;

        foreach (var e in effects)
        {

            if (e && e.kind == Effect.Kind.Named && e.effectName == "ConstructedBuilding")
            {

                hasGoal = true; break;

            }

        }
        if (!hasGoal)
        {

            var e = gameObject.AddComponent<Effect>();

            e.kind = Effect.Kind.Named;

            e.effectName = "ConstructedBuilding";

            //e.name = "ConstructedBuilding";

            effects.Add(e);


        }

    }

    public override float ComputeCost(GridManager g, Tile startTile)
    {

        if (!g || !startTile)  //Without a grid or a start, action is impossible
        {

            return float.PositiveInfinity;

        }
        if (!BuildingsGeneralManager.Instance || !BuildingsGeneralManager.Instance.HasSlot)   // No manager or free slots
        {

            return float.PositiveInfinity;

        }
        if (!bb)    
        {

            return float.PositiveInfinity;

        }
        if (!bb.targetBuildTile)   //If the target has not yet been selected, select
        {

            bb.targetBuildTile = BuildingsGeneralManager.Instance.PickBuildTileInRange();

        }
        if (!bb.targetBuildTile)
        {

            return float.PositiveInfinity;

        }



        var path = Pathfinder.FindPath(g, startTile, bb.targetBuildTile);

        return path == null ? float.PositiveInfinity : path.Count;


    }

    //   where it will end up after the action on the construction cell

    public override Tile PredictPostActionTile(GridManager g, Tile startTile) => (bb != null && bb.targetBuildTile != null) ? bb.targetBuildTile : startTile;


    public override IEnumerator DoAction()
    {

        wasSuccesful = false;

        if (!grid || !bb || !BuildingsGeneralManager.Instance)
        {

            yield break;

        }
        if (!bb.targetBuildTile)
        {

            bb.targetBuildTile = BuildingsGeneralManager.Instance.PickBuildTileInRange();

        }
        if (!bb.targetBuildTile)
        {

            yield break;

        }

        

        bool reached = false, blocked = false;

        StartCoroutine(mover.GoToCoroutine(bb.targetBuildTile, unit ? unit.moveSpeed : 6f, false, true, () => reached = true, () => blocked = true));  //do not stop nearby, when blocking the path try to re-position, achievement callback, block callback

        // mover.StartGoTo(bb.targetBuildTile, unit ? unit.moveSpeed : 6f, false, true, () => reached = true, () => blocked = true);

        isMoving = true;

        while (!reached && !blocked)
        {

            yield return null;

        }

        isMoving = false;

        if (blocked)
        {

            yield break;

        }
        if (!bb.targetBuildTile || !bb.targetBuildTile.IsWalkable || bb.targetBuildTile.content)   //Checking if a cell is still valid
        {

            yield break;

        }
        if (!BuildingsGeneralManager.Instance.TrySpawn(bb.selectedType, bb.targetBuildTile))   //spawn a building
        {

            yield break;

        }


        BuildingsGeneralManager.Instance.ConsumeBackpackFor(localState, bb.selectedType);        //Write off materials from the unit's backpack according to the recipe


        bb.targetBuildTile = null;   //clean the target and give the effect

        ApplyEffects();


        wasSuccesful = true;




    }


















}
