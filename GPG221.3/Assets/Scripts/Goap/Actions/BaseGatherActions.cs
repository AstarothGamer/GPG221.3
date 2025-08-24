using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NPC;
using Resource;

namespace Goap
{
    public abstract class BaseGatherAction : Action
    {
        public abstract ResourceType TargetType { get; }
        public int amountPerTrip = 3;

        protected FollowPathMovement mover;
        protected Unit unit;
        protected GridManager grid => GridManager.Instance;

        protected Resource.Resource cachedRes;
        protected Tile cachedAdjTile;
        protected int cachedPathCost = int.MaxValue;

        protected override void Awake()
        {
            base.Awake();
            mover = GetComponent<FollowPathMovement>();
            unit  = GetComponent<Unit>();

            prerequisits ??= new List<Prerequisite>();
            string knownFact = WorldState.KnownFactName(TargetType);
            bool hasKnown = false;
            foreach (var p in prerequisits)
                if (p != null && p.kind == PrereqKind.Named && p.name == knownFact)
                {
                    hasKnown = true;
                    break;
                }
                
            if (!hasKnown)
            {
                prerequisits.Add(new Prerequisite
                {
                    kind = PrereqKind.Named,
                    name = knownFact
                });
            }

            effects ??= new List<Effect>();
            bool hasDelta = false;
            foreach (var e in effects)
                if (e != null && e.kind == EffectKind.ResourceDelta && e.resourceType == TargetType && e.amount > 0)
                { hasDelta = true; break; }
            if (!hasDelta)
            {
                effects.Add(new Effect
                {
                    kind = EffectKind.ResourceDelta,
                    resourceType = TargetType,
                    amount = Mathf.Max(1, amountPerTrip)
                });
            }
        }

        public override void ApplyEffects()
        {
            if (effects == null || worldState == null) return;
            foreach (var e in effects)
            {
                if (e == null || e.kind != EffectKind.Named) continue;

                bool already = false;
                foreach (var we in worldState.receivedEffects)
                    if (we != null && we.kind == EffectKind.Named && we.name == e.name)
                    {
                        already = true;
                        break;
                    }

                if (!already)
                    worldState.receivedEffects.Add(new Effect { kind = EffectKind.Named, name = e.name });
            }
        }

        public override float ComputeCost(GridManager g, Tile startTile)
        {
            int have = TargetType switch
            {
                ResourceType.Wood  => localState.wood,
                ResourceType.Stone => localState.stone,
                ResourceType.Steel => localState.steel,
                ResourceType.Food  => localState.food,
                _ => 0
            };
            int cap = TargetType switch
            {
                ResourceType.Wood  => localState.woodMax,
                ResourceType.Stone => localState.stoneMax,
                ResourceType.Steel => localState.steelMax,
                ResourceType.Food  => localState.foodMax,
                _ => 0
            };
            if (have >= cap) return float.PositiveInfinity;

            cachedRes = null;
            cachedAdjTile = null;
            cachedPathCost = int.MaxValue;

            if (!g || startTile == null) return float.PositiveInfinity;

            List<Tile> known = TargetType switch
            {
                ResourceType.Wood  => worldState?.knownWoodTiles,
                ResourceType.Stone => worldState?.knownStoneTiles,
                ResourceType.Steel => worldState?.knownSteelTiles,
                ResourceType.Food  => worldState?.knownFoodTiles,
                _ => null
            };
            if (known == null || known.Count == 0)
            {
                Debug.Log("dont have any known tiles with resources");
                return float.PositiveInfinity;
            }

            var knownSet = new HashSet<Tile>(known);
            var all = Object.FindObjectsOfType<Resource.Resource>();
            foreach (var r in all)
            {
                if (!r || r.resourceType != TargetType) continue;
                if (r.StockPile <= 0) continue;
                if (r.Tile == null || !r.Tile.Discovered) continue;
                if (!knownSet.Contains(r.Tile)) continue;
                if (ResourceReservationService.IsReserved(r)) continue;

                Tile bestAdj = null;
                int bestCost = int.MaxValue;
                foreach (var adj in g.GetAdjacentTiles(r.Tile.position))
                {
                    if (adj == null || !adj.IsWalkable) continue;
                    var path = Pathfinder.FindPath(g, startTile, adj);
                    if (path == null) continue;
                    int cost = path.Count;
                    if (cost < bestCost)
                    {
                        bestCost = cost;
                        bestAdj = adj;
                    }
                }

                if (bestAdj != null && bestCost < cachedPathCost)
                {
                    cachedRes = r;
                    cachedAdjTile = bestAdj;
                    cachedPathCost = bestCost;
                }
            }

            if (cachedRes == null) return float.PositiveInfinity;
            targetPosition = cachedRes.gameObject;
            return cachedPathCost;
        }

        public override Tile PredictPostActionTile(GridManager g, Tile startTile)
            => cachedAdjTile ?? startTile;

        public override IEnumerator DoAction()
        {
            Debug.Log("Getting resources");
            wasSuccesful = false;
            if (!grid) yield break;

            var startTile = grid.Get(transform.position);
            if (startTile == null) yield break;

            var cost = ComputeCost(grid, startTile);
            if (float.IsInfinity(cost) || cachedRes == null || cachedAdjTile == null) yield break;

            if (!ResourceReservationService.TryReserve(cachedRes, this)) yield break;

            bool reached = false, blocked = false;
            mover.StartGoTo(cachedAdjTile, unit ? unit.moveSpeed : 6f, false, true,
                () => reached = true, () => blocked = true);

            isMoving = true;
            while (!reached && !blocked) yield return null;
            isMoving = false;

            if (blocked || !cachedRes || cachedRes.Tile == null || cachedRes.StockPile <= 0)
            {
                ResourceReservationService.Release(cachedRes, this);
                yield break;
            }

            int before = TargetType switch
            {
                ResourceType.Wood  => localState.wood,
                ResourceType.Stone => localState.stone,
                ResourceType.Steel => localState.steel,
                ResourceType.Food  => localState.food,
                _ => 0
            };
            int cap = TargetType switch
            {
                ResourceType.Wood  => localState.woodMax,
                ResourceType.Stone => localState.stoneMax,
                ResourceType.Steel => localState.steelMax,
                ResourceType.Food  => localState.foodMax,
                _ => int.MaxValue
            };
            int space = Mathf.Max(0, cap - before);
            int willTake = Mathf.Clamp(amountPerTrip, 0, space);

            if (willTake <= 0)
            {
                ResourceReservationService.Release(cachedRes, this);
                yield break;
            }

            cachedRes.ConsumeStock(willTake);

            switch (TargetType)
            {
                case ResourceType.Wood:
                    localState.wood  = Mathf.Clamp(localState.wood  + willTake, 0, localState.woodMax);
                    break;
                case ResourceType.Stone:
                    localState.stone = Mathf.Clamp(localState.stone + willTake, 0, localState.stoneMax);
                    break;
                case ResourceType.Steel:
                    localState.steel = Mathf.Clamp(localState.steel + willTake, 0, localState.steelMax);
                    break;
                case ResourceType.Food:
                    localState.food  = Mathf.Clamp(localState.food  + willTake, 0, localState.foodMax);
                    break;
            }
            yield return new WaitForSeconds(2f);

            ApplyEffects();

            ResourceReservationService.Release(cachedRes, this);
            wasSuccesful = true;
        }
    }
}
