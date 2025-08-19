using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Resource;

namespace Goap
{
    public class Action : MonoBehaviour
    {
        [SerializeField] public TMP_Text currentActionText;
        public string actionName;

        public WorldState worldState;

        public LocalState localState;
        public List<Prerequisite> prerequisits = new();
        public List<Effect> effects = new();

        public GameObject targetPosition;
        public bool isGuaranteed = false;
        public bool isMoving = false;
        public bool wasSuccesful = false;

        protected virtual void Awake()
        {
            if (!localState) localState = GetComponent<LocalState>();
            if (!worldState)
            {
                if (BaseWarehouse.Instance)
                    worldState = BaseWarehouse.Instance.GetComponent<WorldState>();
                if (!worldState)
                    worldState = FindObjectOfType<WorldState>();
            }
        }

        public virtual IEnumerator DoAction() { yield return null; }

        public virtual bool TryDoAction()
        {
            if (!isGuaranteed) return false;

            foreach (var p in prerequisits)
            {
                if (!p) continue;

                if (p.kind == Prerequisite.Kind.Named)
                {
                    bool has = false;
                    if (worldState != null)
                        foreach (var eff in worldState.receivedEffects)
                            if (eff && eff.name == p.name) { has = true; break; }
                    if (!has)
                    { currentActionText?.SetText($"I cannot do {actionName}, missing {p.name}."); return false; }
                }
                else
                {
                    int have = GetAmount(localState, p.resourceType);
                    if (have < p.minAmount)
                    { currentActionText?.SetText($"I need {p.minAmount} {p.resourceType}, have {have}."); return false; }
                }
            }
            return true;
        }

        public virtual void ApplyEffects()
        {
            foreach (var e in effects)
            {
                if (!e) continue;

                if (e.kind == Effect.Kind.Named)
                {
                    bool already = false;
                    if (worldState != null)
                    {
                        foreach (var we in worldState.receivedEffects)
                            if (we && we.name == e.name) { already = true; break; }
                        if (!already) worldState.receivedEffects.Add(e);
                    }
                }
                else AddAmount(localState, e.resourceType, e.amount);
            }
        }

        public virtual float ComputeCost(GridManager grid, Tile startTile) => 1f;

        public virtual Tile PredictPostActionTile(GridManager grid, Tile startTile) => startTile;

        protected static int GetAmount(LocalState ls, ResourceType t) => t switch
        {
            ResourceType.Wood  => ls.wood,
            ResourceType.Stone => ls.stone,
            ResourceType.Steel => ls.steel,
            ResourceType.Food  => ls.food,
            _ => 0
        };
        protected static void AddAmount(LocalState ls, ResourceType t, int delta)
        {
            if (delta == 0) return;
            switch (t)
            {
                case ResourceType.Wood:
                    ls.wood  = Mathf.Clamp(ls.wood  + delta, 0, ls.woodMax);
                    break;
                case ResourceType.Stone:
                    ls.stone = Mathf.Clamp(ls.stone + delta, 0, ls.stoneMax);
                    break;
                case ResourceType.Steel:
                    ls.steel = Mathf.Clamp(ls.steel + delta, 0, ls.steelMax);
                    break;
                case ResourceType.Food:
                    ls.food  = Mathf.Clamp(ls.food  + delta, 0, ls.foodMax);
                    break;
            }
        }
    }
}
