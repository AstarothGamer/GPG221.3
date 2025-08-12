using System;
using System.Collections;
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

        private IEnumerator Start()
        {
            if (!currentTile)
                GridManager.Instance.Get(transform.position)?.PlaceUnit(this);
        
            //example
            var moveTo = GridManager.Instance.Get(new Vector2Int(5, 4));
            var path = Pathfinder.FindPath(GridManager.Instance, currentTile, moveTo, false, false);
            var moveTo2 = GridManager.Instance.Get(new Vector2Int(0, 0));
            var path2 = Pathfinder.FindPath(GridManager.Instance, moveTo, moveTo2, false, true);

            yield return movement.GoToCoroutine(moveTo, 2, true, () => { Debug.Log("reached goal"); },
                () => { Debug.Log("path blocked"); });
            yield return movement.FollowPathCoroutine(path2, 2, true, () => { Debug.Log("reached goal"); },
                () => { Debug.Log("path blocked"); });
        }
        private void OnDestroy()
        {
            UnitManager.Instance?.RemoveUnit(this);
        }


        public void SetTile(Tile tile) => tile.PlaceUnit(this);

    }
}
