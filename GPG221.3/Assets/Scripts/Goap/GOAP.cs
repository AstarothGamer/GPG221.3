using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Goap
{
    public class GOAP : MonoBehaviour
    {
        public WorldState worldState;
        public List<Action> actions;

        public List<Action> finalPlan;
        public List<Action> possibleActions;
        public List<Action> plan;
        public List<Action> failedActions = new();

        [SerializeField] public string Goal;

        void Start()
        {
            actions = new List<Action>(GetComponents<Action>());
            CheckingActions();
        }

        public void CheckingActions()
        {
            possibleActions.Clear();
            for (int i = 0; i < actions.Count; i++)
            {
                for (int e = 0; e < actions[i].effects.Count; e++)
                {
                    if (actions[i].effects[e].name == Goal)
                    {
                        if (!possibleActions.Contains(actions[i]))
                        {
                            possibleActions.Add(actions[i]);
                            break;
                        }
                    }
                }
            }
            FinalPlan();
        }

        public void FinalPlan()
        {
            finalPlan.Clear();
            int bestLength = int.MaxValue;
            List<Action> bestPath = new();

            for (int i = 0; i < possibleActions.Count; i++)
            {
                if (failedActions.Contains(possibleActions[i]))
                {
                    continue;
                }

                List<string> visited = new();
                List<Action> currentPath = new();

                if (PlanPath(possibleActions[i], currentPath, visited))
                {
                    if (currentPath.Count < bestLength)
                    {
                        bestLength = currentPath.Count;
                        bestPath = currentPath;
                    }
                }
            }

            if (bestPath != null)
            {
                finalPlan = bestPath;
                Debug.Log("final plan:");

                for (int i = 0; i < finalPlan.Count; i++)
                {
                    Debug.Log(finalPlan[i].actionName);
                }

                StartCoroutine(ExecutePlanCoroutine());
            }
            else
            {
                Debug.LogError("Goal can not be achieved");
            }
        }

        public bool PlanPath(Action goapAction, List<Action> path, List<string> visited)
        {
            for (int i = 0; i < goapAction.prerequisits.Count; i++)
            {
                for (int j = 0; j < visited.Count; j++)
                {
                    if (visited[j] == goapAction.prerequisits[i].name)
                    {
                        return false;
                    }
                }

                bool hasEffect = false;
                for (int j = 0; j < worldState.receivedEffects.Count; j++)
                {
                    if (goapAction.prerequisits[i].name == worldState.receivedEffects[j].name)
                    {
                        hasEffect = true;
                        break;
                    }
                }

                if (!hasEffect)
                {
                    Action subGoapAction = null;
                    for (int a = 0; a < actions.Count; a++)
                    {
                        if (failedActions.Contains(actions[a]))
                        {
                            continue;
                        }
                    
                        for (int e = 0; e < actions[a].effects.Count; e++)
                        {
                            if (actions[a].effects[e].name == goapAction.prerequisits[i].name)
                            {
                                subGoapAction = actions[a];
                                break;
                            }
                        }

                        if (subGoapAction != null)
                        {
                            break;
                        }
                    }

                    if (subGoapAction == null)
                    {
                        return false;
                    }

                    visited.Add(goapAction.prerequisits[i].name);

                    if (!PlanPath(subGoapAction, path, visited))
                    {
                        return false;
                    }
                }

            }

            path.Add(goapAction);
            return true;
        }
    

        private IEnumerator ExecutePlanCoroutine()
        {
            while (finalPlan.Count > 0)
            {
                for (int i = 0; i < finalPlan.Count; i++)
                {
                    Action currentGoapAction = finalPlan[i];
                    yield return currentGoapAction.DoAction();

                    if (!currentGoapAction.wasSuccesful)
                    {
                        Debug.LogError("Action " + currentGoapAction.actionName + " failed. Replanning.");

                        if (!failedActions.Contains(currentGoapAction))
                        {
                            failedActions.Add(currentGoapAction);
                        }

                        yield return new WaitForSeconds(1f);

                        CheckingActions();
                        yield break;
                    }
                }

                Debug.Log("Finished executing plan.");
                yield break;
            }
        }
    }
}

