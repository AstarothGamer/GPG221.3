using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Goap;



public class DecideBuildingAction : Action
{
    BuildBlackboard bb;


    protected override void Awake()
    {

        base.Awake();

        bb = GetComponent<BuildBlackboard>() ?? gameObject.AddComponent<BuildBlackboard>();


        //effects.RemoveAll(e => e == null);

        //var eff = effects.Find(e => e && e.kind == Effect.Kind.Named && e.name == "HasConstructionPlan");


        //if (!eff)
        //{

        //    eff = gameObject.AddComponent<Effect>();

        //    eff.kind = Effect.Kind.Named;

        //    eff.effectName = "HasConstructionPlan";

        //    eff.name = "HasConstructionPlan";

        //    effects.Add(eff);



        //}



        //effects.RemoveAll(e => e && e.kind == Effect.Kind.Named && e.name != "HasConstructionPlan");



        bool has = false;  //The effect of having a plan so that subsequent actions can depend on it

        foreach (var e in effects)
        {
            if (e && e.kind == Effect.Kind.Named && e.effectName == "HasConstructionPlan") { has = true; break; }


        }

        if (!has)
        {
            var e = gameObject.AddComponent<Effect>();

            e.kind = Effect.Kind.Named;

            e.effectName = "HasConstructionPlan";

            

            effects.Add(e);

        }


    }

    public override float ComputeCost(GridManager g, Tile startTile)
    {

        if (!BuildingsGeneralManager.Instance || !BuildingsGeneralManager.Instance.HasSlot)  //There is no planning if there are no more slots or no access to the warehouse
        {

            return float.PositiveInfinity;

        }
        if (!worldState)
        {

            return float.PositiveInfinity;

        }

        var order = new[] { BuildingType.House, BuildingType.Forge, BuildingType.Tower }; 

        int a = Random.Range(0, order.Length), b = Random.Range(0, order.Length);  //order randomization

        (order[a], order[b]) = (order[b], order[a]);


        foreach (var t in order)     //Select the first type available in the warehouse and fix it in the blackboard
        {

            if (BuildingsGeneralManager.Instance.CanAfford(worldState, t))
            {

                bb.selectedType = t;

                return 1f;

            }



        }

        return float.PositiveInfinity;
    }
        
    public override Tile PredictPostActionTile(GridManager g, Tile startTile) => startTile;


    public override System.Collections.IEnumerator DoAction()
    {

        wasSuccesful = BuildingsGeneralManager.Instance && BuildingsGeneralManager.Instance.HasSlot && worldState && BuildingsGeneralManager.Instance.CanAfford(worldState, bb.selectedType);  //The action is successful if there are slots and the warehouse can pay for the selected type

        if (wasSuccesful) 
        {

            ApplyEffects();   //   issue "HasConstructionPlan"


        }


        yield break;
    }




}


  



