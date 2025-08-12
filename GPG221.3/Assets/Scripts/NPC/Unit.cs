using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace NPC
{
    [RequireComponent(typeof(FollowPathMovement))]

    public class Unit : MonoBehaviour
    {
        public Tile currentTile;
        protected FollowPathMovement movement;
        public float moveSpeed = 1;
    
        private void Awake()
        {
            movement = GetComponent<FollowPathMovement>();
            UnitManager.Instance.AddUnit(this);
        }

        private void Start()
        {
            if (!currentTile)
                GridManager.Instance.Get(transform.position)?.PlaceUnit(this);
        
            //example
            var moveTo = GridManager.Instance.Get(new Vector2Int(5, 4));
            var path = Pathfinder.FindPath(GridManager.Instance, currentTile, moveTo, false, 2);
            FollowPath(path);
        }
        private void OnDestroy()
        {
            UnitManager.Instance?.RemoveUnit(this);
        }


        public void FollowPath(List<Tile> path, bool findNewPathIfBlocked = false)
        {
            movement.FollowPath(path, moveSpeed, findNewPathIfBlocked, targetReachedCallback: () => SetTile(path.LastOrDefault()));
        }

        void SetTile(Tile tile) => tile.PlaceUnit(this);

    }
}
