using System.Collections.Generic;
using UnityEngine;

using Vector3 = UnityEngine.Vector3;

namespace Resource
{
    public class ResourceManager : Singleton<ResourceManager>
    {
        public Dictionary<ResourceType , List<GameObject>> Resources = new();

        public GameObject GetNearestResource(ResourceType resourceType, Vector3 fromPosition)
        {
            GameObject nearestResource = null;
            
            if (Resources == null || !Resources.ContainsKey(resourceType) || Resources[resourceType].Count == 0)
            {
                Debug.LogWarning($"No resources of type {resourceType} found.");
                return null;
            }
            for (int i = 0; i< Resources[resourceType].Count; i++)
            {
                if (nearestResource == null)
                {
                    nearestResource = Resources[resourceType][i];
                    continue;
                }
                float distanceFromLastResource = Vector3.Distance(fromPosition, nearestResource.transform.position);
                float distanceFromCurrentResource = Vector3.Distance(fromPosition, Resources[resourceType][i].transform.position);
                
                if (distanceFromLastResource > distanceFromCurrentResource)
                {
                    nearestResource = Resources[resourceType][i];
                }
            }
            return nearestResource;
        }
    }
    public enum ResourceType
    {
        Tree,
        Stone,
        Water,
        Food
    }
}
