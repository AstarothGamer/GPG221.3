using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;


namespace TowerDefense
{
    public class FollowPathMovement : MonoBehaviour
    {
        public void GoTo(Tile destination, float speed, bool findNewPathIfBlocked = true,
            Action targetReachedCallback = null, Action pathBlockedCallback = null)
        {
            var current = GridManager.Instance.Get(transform.position);
            var path = Pathfinder.FindPath(GridManager.Instance, current, destination);
            if (path == null || path.Count == 0)
            {
                pathBlockedCallback?.Invoke();
                return;
            }
            FollowPath(path, speed, findNewPathIfBlocked, targetReachedCallback, pathBlockedCallback);
        }

        public void FollowPath(List<Tile> path, float speed, bool findNewPathIfBlocked = true,
            Action targetReachedCallback = null, Action pathBlockedCallback = null)
        {
            if (path == null || path.Count == 0)
            {
                Debug.LogWarning("Path is empty", this);
            }
            StopAllCoroutines();
            StartCoroutine(FollowPathCoroutine(path, speed, findNewPathIfBlocked, targetReachedCallback, pathBlockedCallback));
        }

        private IEnumerator FollowPathCoroutine(List<Tile> path, float speed, bool findNewPathIfBlocked = true,
            Action targetReachedCallback = null, Action pathBlockedCallback = null)
        {
            if (path.Count == 0)
            {
                Debug.LogWarning("Path is empty", this);
                yield break;
            }
            
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

                    //Reached destination
                    if (currentIndex == path.Count)
                    {
                        targetReachedCallback?.Invoke();
                        yield break;
                    }
                    
                    //Path blocked
                    if (!path[currentIndex] || !path[currentIndex].IsWalkable)
                    {
                        if (findNewPathIfBlocked)
                        {
                            GoTo(path.LastOrDefault(), speed, findNewPathIfBlocked, targetReachedCallback, pathBlockedCallback);
                            yield break;
                        }
                        
                        Debug.Log("Path blocked", this);
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
