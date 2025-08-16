using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;

namespace NPC
{
    public class FollowPathMovement : MonoBehaviour
    {
        private VisionSource visionSource;
        private Unit unit;
        
        private void Awake()
        {
            visionSource = GetComponent<VisionSource>();
            unit = GetComponent<Unit>();
        }

        public void StartGoTo(
            Tile destination, 
            float speed, 
            bool stopAtAdjacent = false, 
            bool findNewPathIfBlocked = true,
            Action targetReachedCallback = null, 
            Action pathBlockedCallback = null)
        {
            StartCoroutine(GoToCoroutine(destination, speed, findNewPathIfBlocked, 
                stopAtAdjacent, targetReachedCallback, pathBlockedCallback));
        }

        public void StartFollowPath(
            List<Tile> path, 
            float speed, 
            bool findNewPathIfBlocked = true,
            Action targetReachedCallback = null, 
            Action pathBlockedCallback = null)
        {
            StartCoroutine(FollowPathCoroutine(path, speed, findNewPathIfBlocked, 
                targetReachedCallback, pathBlockedCallback));
        }


        public IEnumerator GoToCoroutine(
            Tile destination, 
            float speed, 
            bool stopAtAdjacent = false, 
            bool findNewPathIfBlocked = true,
            Action targetReachedCallback = null, 
            Action pathBlockedCallback = null)
        {
            var current = GridManager.Instance.Get(transform.position);
            var path = Pathfinder.FindPath(GridManager.Instance, current, destination, 
                                    includeStartTile: false, stopOneTileEarly: stopAtAdjacent);

            if (path.IsNullOrEmpty())
            {
                yield return GoToNearestStandable(current, speed, pathBlockedCallback);
                yield break;
            }

            yield return FollowPathCoroutine(path, speed, findNewPathIfBlocked,
                targetReachedCallback, pathBlockedCallback, stopAtAdjacent ? destination : null);
        }
        
        [ShowInInspector, ReadOnly] List<Tile> currentPath; // for debug only
        public IEnumerator FollowPathCoroutine(
            List<Tile> path, 
            float speed, 
            bool findNewPathIfBlocked = true,
            Action targetReachedCallback = null, 
            Action pathBlockedCallback = null, 
            Tile stopAtAdjacentIfFail = null, 
            bool ignoreWalkable = false)
        {
            if (path.IsNullOrEmpty())
            {
                Debug.LogWarning("Path is empty", this);
                pathBlockedCallback?.Invoke();
                yield break;
            }
            
            unit?.SetTile(null); // Remove unit tile
            currentPath = path; // Show path in the inspector
            
            int currentIndex = 0;
            var nextTilePos = path[currentIndex].transform.position;

            while (currentIndex <= path.Count)
            {
                var moveBy = speed * Time.deltaTime;
                float distance = Vector3.Distance(transform.position, nextTilePos);

                while (moveBy >= distance)
                {
                    transform.position = nextTilePos;
                    currentIndex++;
                    
                    visionSource?.CheckVision(); // UPDATE FOG OF WAR

                    //Reached destination
                    if (currentIndex >= path.Count)
                    {
                        //Cannot stop on current destination
                        if (path.LastOrDefault() && !path.LastOrDefault()!.CanStandOn)
                        {
                            yield return FindStandableTile(speed, findNewPathIfBlocked, 
                                targetReachedCallback, pathBlockedCallback, stopAtAdjacentIfFail);
                            yield break;
                        }

                        unit?.SetTile(path.LastOrDefault()); // Set unit tile
                        targetReachedCallback?.Invoke();
                        yield break;
                    }
                    
                    //Path blocked
                    if (!path[currentIndex] || (!ignoreWalkable && !path[currentIndex].IsWalkable))
                    {
                        if (findNewPathIfBlocked)
                        {
                            yield return GoToCoroutine(stopAtAdjacentIfFail ?? path.LastOrDefault(), speed, 
                                stopAtAdjacentIfFail, findNewPathIfBlocked, targetReachedCallback, pathBlockedCallback);
                            yield break;
                        }
                        
                        yield return FindStandableTile(speed, findNewPathIfBlocked, 
                            pathBlockedCallback, pathBlockedCallback, null);
                        yield break;
                    }

                    nextTilePos = path[currentIndex].transform.position;
                    moveBy -= distance;
                    distance = Vector3.Distance(transform.position, nextTilePos);
                }

                Vector3 dir = (nextTilePos - transform.position).normalized;
                transform.position += dir * moveBy;
                yield return null;
            }

            Debug.LogWarning("Something weird happened. Should not have reached this part of code", this);
        }

        protected IEnumerator FindStandableTile(
            float speed, 
            bool findNewPathIfBlocked,
            Action targetReachedCallback, 
            Action pathBlockedCallback, 
            Tile stopAtAdjacentIfPossible)
        {
            var currentTile = GridManager.Instance.Get(transform.position);
            if (!stopAtAdjacentIfPossible)
            {
                // No available path
                yield return GoToNearestStandable(currentTile, speed, pathBlockedCallback);
                yield break;
            }

            // Find path to different neighbour of final tile
            var newPath = Pathfinder.FindPath(GridManager.Instance, currentTile, stopAtAdjacentIfPossible,
                includeStartTile: true, stopOneTileEarly: true, ignoreCanStandOn: false, ignoreWalkable: false);
            if (!newPath.IsNullOrEmpty())
            {
                yield return FollowPathCoroutine(newPath, speed, findNewPathIfBlocked, 
                    targetReachedCallback, pathBlockedCallback, stopAtAdjacentIfPossible, false);
                yield break;
            }

            // No available path
            yield return GoToNearestStandable(currentTile, speed, pathBlockedCallback);
        }
        
        IEnumerator GoToNearestStandable(Tile currentTile, float speed, Action pathBlockedCallback)
        {
            if (TryGetPathToNearestStandable(currentTile, out List<Tile> path, out bool ignoreWalkable))
            {
                yield return FollowPathCoroutine(path, speed, false, 
                    pathBlockedCallback, pathBlockedCallback, ignoreWalkable: ignoreWalkable);
                yield break;
            }
            
            Debug.Log("Path blocked", this);
            unit?.SetTile(GridManager.Instance.Get(transform.position)); // Set unit tile
        }

        bool TryGetPathToNearestStandable(Tile currentTile, out List<Tile> path, out bool ignoreWalkable)
        {
            path = null;
            ignoreWalkable = false;
            var nearestStandable = Pathfinder.GetNearestTile(GridManager.Instance, currentTile, 
                                                criteria: t => t.CanStandOn, ignoreWalkable: false);
            
            if (!nearestStandable) // if no standable tile nearby, retry ignoring walkable tiles
            {
                nearestStandable = Pathfinder.GetNearestTile(GridManager.Instance, currentTile, 
                                                criteria: t => t.CanStandOn, ignoreWalkable: true);
                if (nearestStandable)
                {
                    path = Pathfinder.FindPath(GridManager.Instance, currentTile, nearestStandable, 
                        includeStartTile: true, stopOneTileEarly: false, ignoreCanStandOn: false, ignoreWalkable: true);
                    ignoreWalkable = true;
                }
            }
            else
            {
                path = Pathfinder.FindPath(GridManager.Instance, currentTile, nearestStandable, 
                    includeStartTile: true, stopOneTileEarly: false, ignoreCanStandOn: false, ignoreWalkable: false);
            }

            if (path.IsNullOrEmpty())
                return false;
            return true;
        }

    }
}