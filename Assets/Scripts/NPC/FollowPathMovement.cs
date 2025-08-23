using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
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

        public void StartGoTo(Tile destination, float speed, bool stopAtAdjacent = false, 
            bool findNewPathIfBlocked = true, Action targetReachedCallback = null, Action pathBlockedCallback = null)
        {
            StartCoroutine(GoToCoroutine(destination, speed, findNewPathIfBlocked, 
                stopAtAdjacent, targetReachedCallback, pathBlockedCallback));
        }

        public void StartFollowPath(List<Tile> path, float speed, bool findNewPathIfBlocked = true,
            Action targetReachedCallback = null, Action pathBlockedCallback = null)
        {
            StartCoroutine(FollowPathCoroutine(path, speed, findNewPathIfBlocked, 
                targetReachedCallback, pathBlockedCallback));
        }


        public IEnumerator GoToCoroutine(Tile destination, float speed, bool stopAtAdjacent = false, bool findNewPathIfBlocked = true,
            Action targetReachedCallback = null, Action pathBlockedCallback = null)
        {
            var current = GridManager.Instance.Get(transform.position);
            var path = Pathfinder.FindPath(GridManager.Instance, current, destination);

            if (path == null || path.Count == 0)
            {
                unit?.SetTile(GridManager.Instance.Get(transform.position));
                pathBlockedCallback?.Invoke();
                yield break;
            }

            yield return FollowPathCoroutine(path, speed, findNewPathIfBlocked,
                targetReachedCallback, pathBlockedCallback, stopAtAdjacent ? destination : null);
        }
        
        [ShowInInspector, ReadOnly] List<Tile> currentPath; // for debug only
        public IEnumerator FollowPathCoroutine(List<Tile> path, float speed, bool findNewPathIfBlocked = true,
            Action targetReachedCallback = null, Action pathBlockedCallback = null, Tile stopAtAdjacentIfFail = null)
        {
            if (path == null || path.Count == 0)
            {
                Debug.LogWarning("Path is empty", this);
                pathBlockedCallback?.Invoke();
                yield break;
            }
            
            unit?.SetTile(null);
            currentPath = path;
            
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
                    if (currentIndex == path.Count)
                    {
                        unit?.SetTile(path.LastOrDefault());
                        targetReachedCallback?.Invoke();
                        yield break;
                    }

                    //Path blocked
                    if (!path[currentIndex] || !path[currentIndex].IsWalkable)
                    {
                        if (findNewPathIfBlocked)
                        {
                            yield return GoToCoroutine(stopAtAdjacentIfFail ?? path.LastOrDefault(), speed, 
                                stopAtAdjacentIfFail, findNewPathIfBlocked, targetReachedCallback, pathBlockedCallback);
                            yield break;
                        }
                        
                        Debug.Log("Path blocked", this);
                        unit?.SetTile(GridManager.Instance.Get(transform.position));
                        
                        pathBlockedCallback?.Invoke();
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
    }
}