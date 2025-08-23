using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NPC;
using Goap;




public class WithdrawMaterialsAction : Action                   //The action of getting materials from the warehouse into the backpack
{

    FollowPathMovement mover;

    Unit unit;

    BuildBlackboard bb;

    GridManager grid => GridManager.Instance;


    protected override void Awake()
    {

        base.Awake();

        mover = GetComponent<FollowPathMovement>();

        unit = GetComponent<Unit>();

        bb = GetComponent<BuildBlackboard>() ?? gameObject.AddComponent<BuildBlackboard>();

        //prerequisits.RemoveAll(p => p == null);

        //effects.RemoveAll(e => e == null);

        //var pre = prerequisits.Find(p => p && p.kind == Prerequisite.Kind.Named && p.name == "HasConstructionPlan");

        //if (!pre)
        //{
        //    pre = gameObject.AddComponent<Prerequisite>();

        //    pre.kind = Prerequisite.Kind.Named;

        //    pre.effectName = "HasConstructionPlan";

        //    pre.name = "HasConstructionPlan";

        //    prerequisits.Add(pre);



        //}



        //prerequisits.RemoveAll(p => p && p.kind == Prerequisite.Kind.Named && p.name != "HasConstructionPlan");


        //var eff = effects.Find(e => e && e.kind == Effect.Kind.Named && e.name == "HasBuildingMaterials");



        //if (!eff)
        //{
        //    eff = gameObject.AddComponent<Effect>();

        //    eff.kind = Effect.Kind.Named;

        //    eff.effectName = "HasBuildingMaterials";

        //    eff.name = "HasBuildingMaterials";

        //    effects.Add(eff);


        //}

        //effects.RemoveAll(e => e && e.kind == Effect.Kind.Named && e.name != "HasBuildingMaterials");


        ////////////////

        //A prerequisite - must be a plan

        bool hasPre = false;

        foreach (var p in prerequisits)
        {

            if (p && p.kind == Prerequisite.Kind.Named && p.effectName == "HasConstructionPlan")
            {

                hasPre = true; break;

            }

        }
        if (!hasPre)
        {

            var pre = gameObject.AddComponent<Prerequisite>();

            pre.kind = Prerequisite.Kind.Named;

            pre.effectName = "HasConstructionPlan";

            
            prerequisits.Add(pre);

        }


        //Effect - there are materials

        bool hasEff = false;

        foreach (var e in effects)
        {

            if (e && e.kind == Effect.Kind.Named && e.effectName == "HasBuildingMaterials")
            {

                hasEff = true; break;

            }

        }
        if (!hasEff)
        {

            var e = gameObject.AddComponent<Effect>();

            e.kind = Effect.Kind.Named;

            e.effectName = "HasBuildingMaterials";

            

            effects.Add(e);
        }

        ///////////////////////
    }


    public override float ComputeCost(GridManager g, Tile startTile)
    {
        //There must be a warehouse with entryTile

        if (!BaseWarehouse.Instance || !BaseWarehouse.Instance.entryTile)
        {

            return float.PositiveInfinity;

        }
        if (!g || !startTile)
        {

            return float.PositiveInfinity;

        }
        if (!BuildingsGeneralManager.Instance || !BuildingsGeneralManager.Instance.HasSlot)
        {

            return float.PositiveInfinity;

        }
        if (!worldState || !localState || !bb)
        {

            return float.PositiveInfinity;

        }
        if (!BuildingsGeneralManager.Instance.CanAfford(worldState, bb.selectedType))   //is there a chance to pay for the selected type from the warehouse
        {

            return float.PositiveInfinity;

        }



        var path = Pathfinder.FindPath(g, startTile, BaseWarehouse.Instance.entryTile);   //Cost = path to warehouse entrance

        return path == null ? float.PositiveInfinity : path.Count;

    }

    public override Tile PredictPostActionTile(GridManager g, Tile startTile) => BaseWarehouse.Instance && BaseWarehouse.Instance.entryTile ? BaseWarehouse.Instance.entryTile : startTile;


    public override IEnumerator DoAction()
    {

        wasSuccesful = false;

        if (!grid || !bb || !worldState || !localState)
        {

            yield break;

        }
        if (!BaseWarehouse.Instance || !BaseWarehouse.Instance.entryTile)
        {

            yield break;

        }


        var start = grid.Get(transform.position);

        if (!start)
        {

            yield break;

        }

        bool reached = false, blocked = false;

        StartCoroutine(mover.GoToCoroutine(BaseWarehouse.Instance.entryTile, unit ? unit.moveSpeed : 6f, false, true, () => reached = true, () => blocked = true));   //go to the entrance cell of the warehouse

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
        if (!BuildingsGeneralManager.Instance.WithdrawToBackpack(worldState, localState, bb.selectedType)) // trying to write off the cost of the recipe from the warehouse and put it into LocalState
        {

            yield break;

        }


        ApplyEffects();       //issue "HasBuildingMaterials"

        wasSuccesful = true;


    }







}
