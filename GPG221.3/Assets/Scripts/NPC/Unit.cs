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
                GridManager.Instance.Get(transform.position)?.SetUnit(this);
            
            // test
            var moveTo = GridManager.Instance.Get(new Vector2Int(5, 4));
            
            yield return movement.GoToCoroutine(moveTo, 2, true, () => { Debug.Log("reached goal"); },
                () => { Debug.Log("path blocked"); });
            
            var moveTo2 = GridManager.Instance.Get(new Vector2Int(0, 0));
            var path2 = Pathfinder.FindPath(GridManager.Instance, currentTile, moveTo2, false, true);
            
            yield return movement.FollowPathCoroutine(path2, 2, true, () => { Debug.Log("reached goal"); },
                () => { Debug.Log("path blocked"); });
            /////////
            
        }
        private void OnDestroy()
        {
            UnitManager.Instance?.RemoveUnit(this);
        }


        public void SetTile(Tile tile)
        {
            if (!tile)
            {
                if (!currentTile) return;
                
                if(currentTile.unit == this)
                    currentTile.SetUnit(null);
                currentTile = null;
                return;
            }
            
            tile.SetUnit(this);
        }

    }
}
