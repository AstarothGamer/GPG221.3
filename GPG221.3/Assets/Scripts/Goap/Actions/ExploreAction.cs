using System.Collections;
using UnityEngine;
using NPC;

namespace Goap
{
    public class ExploreAction : Action
    {
        private FollowPathMovement mover;
        private Unit unit;

        protected override void Awake()
        {
            base.Awake();
            mover = GetComponent<FollowPathMovement>();
            unit  = GetComponent<Unit>();
        }

        public override float ComputeCost(GridManager g, Tile startTile)
        {
            if (!BaseWarehouse.Instance || !BaseWarehouse.Instance.entryTile) return float.PositiveInfinity;
            if (!g || !startTile) return float.PositiveInfinity;

            var pathToNearestUnexplored = Pathfinder.FindPathToNearest(g, startTile, x => !x.Discovered);

            return pathToNearestUnexplored?.Count ?? float.PositiveInfinity;
        }

        public override Tile PredictPostActionTile(GridManager g, Tile startTile)
            => Pathfinder.GetNearestTile(g, startTile, x => !x.Discovered);

        public override IEnumerator DoAction()
        {
            wasSuccesful = false;
            if (!BaseWarehouse.Instance || !BaseWarehouse.Instance.entryTile) yield break;
            if (!GridManager.Instance) yield break;

            var start = GridManager.Instance.Get(transform.position);
            if (!start) yield break;

            isMoving = true;
            bool reached = false, blocked = false;
            yield return mover.GoToCoroutine(BaseWarehouse.Instance.entryTile, 
                targetReachedCallback: () => reached = true, pathBlockedCallback: () => blocked = true);
            isMoving = false;

            if (blocked) yield break;
            wasSuccesful = true;
        }
    }
}

