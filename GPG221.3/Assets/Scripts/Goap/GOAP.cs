using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Goap
{
    public class GOAP : MonoBehaviour
    {
        public WorldState worldState;
        public LocalState localState;

        public string Goal;

        public enum ResourceGoalScope
        {
            LocalBackpack,
            WorldBase
        }

        public bool useResourceGoal = false;
        public Resource.ResourceType goalResourceType;

        public int goalMinAmount = 0;
        public ResourceGoalScope resourceGoalScope = ResourceGoalScope.WorldBase;

        public List<Action> actions;
        public List<Action> finalPlan;
        public List<Action> possibleActions;
        public List<Action> plan;
        public List<Action> failedActions = new();

        private GridManager grid => GridManager.Instance;
        private NPC.Unit unit;

        void Awake()
        {
            if (!localState) localState = GetComponent<LocalState>();
            if (!worldState)
            {
                if (BaseWarehouse.Instance)
                    worldState = BaseWarehouse.Instance.GetComponent<WorldState>();
                if (!worldState)
                    worldState = FindObjectOfType<WorldState>();
            }
            unit = GetComponent<NPC.Unit>();
        }

        void Start()
        {
            actions = new List<Action>(GetComponents<Action>());
            CheckingActions();
        }

        public void CheckingActions()
        {
            possibleActions ??= new List<Action>();
            possibleActions.Clear();

            foreach (var a in actions)
            {
                if (!a) continue;

                if (!useResourceGoal)
                {
                    foreach (var e in a.effects)
                    {
                        if (e == null) continue;
                        if (e.kind == EffectKind.Named && e.name == Goal)
                        {
                            possibleActions.Add(a);
                            break;
                        }
                    }
                }
                else
                {
                    if (resourceGoalScope == ResourceGoalScope.LocalBackpack)
                    {
                        foreach (var e in a.effects)
                        {
                            if (e == null) continue;
                            if (e.kind == EffectKind.ResourceDelta && e.resourceType == goalResourceType && e.amount > 0)
                            {
                                possibleActions.Add(a);
                                break;
                            }
                        }
                    }
                    else 
                    {
                        if (a is BaseDepositAction dep && dep.TargetType == goalResourceType)
                            possibleActions.Add(a);
                    }
                }
            }
            FinalPlan();
        }

        public void FinalPlan()
        {
            finalPlan ??= new List<Action>();
            finalPlan.Clear();

            var startTile = unit?.currentTile ?? grid?.Get(transform.position);
            if (!grid || startTile == null)
            {
                Debug.LogError("GOAP: grid/Unit.currentTile not set");
                return;
            }

            float bestCost = float.PositiveInfinity;
            List<Action> bestPath = null;

            foreach (var root in possibleActions)
            {
                if (root == null || failedActions.Contains(root)) continue;

                var tmpPath = new List<Action>();
                if (PlanPath(root, startTile, tmpPath, out float cost, out Tile _)
                    && cost < bestCost)
                {
                    bestCost = cost;
                    bestPath = tmpPath;
                }
            }

            if (bestPath != null)
            {
                finalPlan = bestPath;
                StartCoroutine(ExecutePlanCoroutine());
                return; 
            }

            var explore = actions.Find(a => a is ExploreAction);
            if (explore != null)
            {
                var explorePath = new List<Action>();
                if (PlanPath(explore, startTile, explorePath, out float expCost, out Tile _)
                    && !float.IsInfinity(expCost))
                {
                    finalPlan = explorePath;
                    StartCoroutine(ExecutePlanCoroutine());
                    return;
                }
            }

            Debug.LogWarning("GOAP: no plan and no explore fallback available.");
        }

        bool PlanPath(Action action, Tile startTile, List<Action> path,
                      out float totalCost, out Tile endTile)
        {
            totalCost = 0f;
            endTile = startTile;

            if (useResourceGoal && IsResourceGoalSatisfied())
            {
                Debug.Log("Resource goal satisf");
                return true;
            }
                

            foreach (var pre in action.prerequisits)
            {
                bool satisfied = false;
                if (pre.kind == PrereqKind.Named)
                {
                    if (worldState != null)
                        foreach (var we in worldState.receivedEffects)
                            if (we != null && we.name == pre.name)
                            {
                                satisfied = true;
                                break;
                            }
                }
                else
                {
                    int haveLocal = pre.resourceType switch
                    {
                        Resource.ResourceType.Wood  => localState.wood,
                        Resource.ResourceType.Stone => localState.stone,
                        Resource.ResourceType.Steel => localState.steel,
                        Resource.ResourceType.Food  => localState.food,
                        _ => 0
                    };
                    satisfied = haveLocal >= pre.minAmount;
                }

                if (!satisfied)
                {
                    Action bestSub = null;
                    float bestSubCost = float.PositiveInfinity;
                    List<Action> bestSubPath = null;
                    Tile bestSubEnd = endTile;

                    foreach (var a in actions)
                    {
                        if (!a || failedActions.Contains(a)) continue;

                        bool provides = false;
                        foreach (var ef in a.effects)
                        {
                            if (ef == null) continue;

                            if (pre.kind == PrereqKind.Named &&
                                ef.kind == EffectKind.Named && ef.name == pre.name)
                            {
                                provides = true;
                                break;
                            }

                            if (pre.kind == PrereqKind.ResourceAmount &&
                                ef.kind == EffectKind.ResourceDelta &&
                                ef.resourceType == pre.resourceType && ef.amount > 0)
                            {
                                provides = true;
                                break;
                            }
                        }
                        if (!provides) continue;

                        var subPath = new List<Action>();
                        if (PlanPath(a, endTile, subPath, out float subCost, out Tile subEnd)
                            && subCost < bestSubCost)
                        {
                            bestSub = a;
                            bestSubCost = subCost;
                            bestSubPath = subPath;
                            bestSubEnd = subEnd;
                        }
                    }

                    if (bestSub == null) return false;

                    path.AddRange(bestSubPath);
                    totalCost += bestSubCost;
                    endTile = bestSubEnd;
                }
            }

            float selfCost = action.ComputeCost(grid, endTile);
            if (float.IsInfinity(selfCost))
            {
                totalCost = float.PositiveInfinity;
                return false;
            }

            path.Add(action);
            totalCost += selfCost;
            endTile = action.PredictPostActionTile(grid, endTile);
            return true;
        }

        private bool IsResourceGoalSatisfied()
        {
            int have = resourceGoalScope == ResourceGoalScope.WorldBase
                ? goalResourceType switch
                {
                    Resource.ResourceType.Wood  => worldState.wood,
                    Resource.ResourceType.Stone => worldState.stone,
                    Resource.ResourceType.Steel => worldState.steel,
                    Resource.ResourceType.Food  => worldState.food,
                    _ => 0
                }
                : goalResourceType switch
                {
                    Resource.ResourceType.Wood  => localState.wood,
                    Resource.ResourceType.Stone => localState.stone,
                    Resource.ResourceType.Steel => localState.steel,
                    Resource.ResourceType.Food  => localState.food,
                    _ => 0
                };
            return have >= goalMinAmount;
        }

        private IEnumerator ExecutePlanCoroutine()
        {
            while (finalPlan.Count > 0)
            {
                for (int i = 0; i < finalPlan.Count; i++)
                {
                    var current = finalPlan[i];
                    current.isGuaranteed = true;

                    if (!current.TryDoAction())
                    {
                        if (!failedActions.Contains(current)) failedActions.Add(current);
                        yield return new WaitForSeconds(0.05f);
                        CheckingActions();
                        yield break;
                    }

                    yield return current.DoAction();

                    if (!current.wasSuccesful)
                    {
                        if (!failedActions.Contains(current)) failedActions.Add(current);
                        yield return new WaitForSeconds(0.05f);
                        CheckingActions();
                        yield break;
                    }

                    if (useResourceGoal && IsResourceGoalSatisfied())
                    {
                        finalPlan.Clear(); 
                        break;
                    }
                }
                Debug.Log("Finished executing plan.");
                if (!IsResourceGoalSatisfied())
                {
                    CheckingActions();
                }
                yield break;
            }
        }
    }
}

