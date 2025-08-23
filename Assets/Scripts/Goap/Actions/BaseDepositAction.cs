using System.Collections;
using UnityEngine;
using NPC;

namespace Goap
{
    public abstract class BaseDepositAction : Action
    {
        public abstract Resource.ResourceType TargetType { get; }

        private FollowPathMovement mover;
        private Unit unit;
        private GridManager grid => GridManager.Instance;

        protected override void Awake()
        {
            base.Awake();
            mover = GetComponent<FollowPathMovement>();
            unit  = GetComponent<Unit>();

            bool hasPre = false;
            foreach (var p in prerequisits)
                if (p && p.kind == Prerequisite.Kind.ResourceAmount && p.resourceType == TargetType) { hasPre = true; break; }
            if (!hasPre)
            {
                var pre = gameObject.AddComponent<Prerequisite>();
                pre.kind = Prerequisite.Kind.ResourceAmount;
                pre.resourceType = TargetType;
                pre.minAmount = 1;
                prerequisits.Add(pre);
            }
        }

        public override float ComputeCost(GridManager g, Tile startTile)
        {
            if (!BaseWarehouse.Instance || !BaseWarehouse.Instance.entryTile) return float.PositiveInfinity;
            if (!g || startTile == null) return float.PositiveInfinity;

            int have = TargetType switch
            {
                Resource.ResourceType.Wood  => localState.wood,
                Resource.ResourceType.Stone => localState.stone,
                Resource.ResourceType.Steel => localState.steel,
                Resource.ResourceType.Food  => localState.food,
                _ => 0
            };
            if (have <= 0) return float.PositiveInfinity;

            var path = Pathfinder.FindPath(g, startTile, BaseWarehouse.Instance.entryTile);
            return path == null ? float.PositiveInfinity : path.Count;
        }

        public override Tile PredictPostActionTile(GridManager g, Tile startTile)
            => BaseWarehouse.Instance && BaseWarehouse.Instance.entryTile ? BaseWarehouse.Instance.entryTile : startTile;

        public override IEnumerator DoAction()
        {
            wasSuccesful = false;
            if (!BaseWarehouse.Instance || !BaseWarehouse.Instance.entryTile) yield break;
            if (!grid) yield break;

            var start = grid.Get(transform.position);
            if (start == null) yield break;

            int have = TargetType switch
            {
                Resource.ResourceType.Wood  => localState.wood,
                Resource.ResourceType.Stone => localState.stone,
                Resource.ResourceType.Steel => localState.steel,
                Resource.ResourceType.Food  => localState.food,
                _ => 0
            };
            if (have <= 0) yield break;

            bool reached = false, blocked = false;
            mover.StartGoTo(BaseWarehouse.Instance.entryTile, unit ? unit.moveSpeed : 6f, false, true,
                () => reached = true, () => blocked = true);

            isMoving = true;
            while (!reached && !blocked) yield return null;
            isMoving = false;

            if (blocked) yield break;

            switch (TargetType)
            {
                case Resource.ResourceType.Wood:
                    worldState.wood  += localState.wood;
                    localState.wood  = 0;
                    break;
                case Resource.ResourceType.Stone:
                    worldState.stone += localState.stone;
                    localState.stone = 0;
                    break;
                case Resource.ResourceType.Steel:
                    worldState.steel += localState.steel;
                    localState.steel = 0;
                    break;
                case Resource.ResourceType.Food:
                    worldState.food  += localState.food;
                    localState.food  = 0;
                    break;
            }

            wasSuccesful = true;
        }
    }
}

