using System.Collections;
using UnityEngine;
using NPC;
using Sirenix.Utilities;
using System.Collections.Generic;

namespace Goap
{
    public class ExploreAction : Action
    {
        private FollowPathMovement mover;
        private Unit unit;
        private VisionSource vision;

        protected override void Awake()
        {
            base.Awake();
            mover = GetComponent<FollowPathMovement>();
            unit = GetComponent<Unit>();
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
            if (!GridManager.Instance) yield break;

            var start = GridManager.Instance.Get(transform.position);
            if (!start) yield break;
            var path = Pathfinder.FindPathToNearest(GridManager.Instance, start, x => !x.Discovered);
            if (path.IsNullOrEmpty()) yield break;
            
            isMoving = true;
            yield return mover.FollowPathCoroutine(path, findNewPathIfBlocked: false);
            isMoving = false;

            if (worldState != null)
            {
                float range = vision ? vision.VisionRange : 2f;
                var tilesEnum = GridManager.Instance.GetTilesInCircle(transform.position, range);
                var tilesInSight = tilesEnum == null ? null : new List<Tile>(tilesEnum);
                
                if (tilesInSight != null && tilesInSight.Count > 0)
                {
                    var inSight = new List<Tile>(tilesInSight);
                    var allResources = FindObjectsOfType<Resource.Resource>();
                    foreach (var res in allResources)
                    {
                        if (res == null || res.Tile == null) continue;
                        if (!res.Tile.Discovered) continue;
                        if (!inSight.Contains(res.Tile)) continue;
                        worldState.RegisterResourceTile(res);
                    }
                }
            }
            
            wasSuccesful = true;
        }
    }
}

