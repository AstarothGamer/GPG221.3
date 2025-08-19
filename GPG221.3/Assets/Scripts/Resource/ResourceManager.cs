using System.Collections.Generic;
using UnityEngine;

using Vector3 = UnityEngine.Vector3;

namespace Resource
{
    /// <summary>
    /// ResourceManager is a singleton class that manages resources in the game.
    /// It gets the nearest resource of a specified type from a given position,
    /// retrieve all resources of a specific type, or get all resources available in the game.
    /// </summary>
    /// <remarks>
    ///  Uses a dictionary to store resources categorized by their type.
    ///  It provides methods to find the nearest resource based on distance, ensuring that the
    /// </remarks>
    /// <example>
    /// <code>
    /// var nearestTree = ResourceManager.Instance.GetNearestResource(ResourceType.Tree, playerPosition);
    /// var allStones = ResourceManager.Instance.GetResourcesFromType(ResourceType.Stone);
    /// var allResources = ResourceManager.Instance.GetAllResources();
    /// </code>
    /// </example>
    public class ResourceManager : Singleton<ResourceManager>
    {
        public Dictionary<ResourceType , List<GameObject>> Resources = new();
        /// <summary>
        /// Gets the nearest resource of the specified type from a given Vector3 position using Distance.
        /// ResourceType(enum) examples: ResourceType.Tree, ResourceType.Stone, ResourceType.Water, ResourceType.Food.
        /// </summary>
        /// <param name="resourceType">Type of nearest resource you want to find.</param>
        /// <param name="fromPosition">The position you want to look for the resource from.</param>
        /// <returns>GameObject of the resource</returns>
        public GameObject GetNearestResource(ResourceType resourceType, Vector3 fromPosition)
        {
            GameObject nearestResource = null;
            
            // Check if Resource is not null or empty and contains the specified key
            if (Resources == null || !Resources.ContainsKey(resourceType) || Resources[resourceType].Count == 0)
            {
                Debug.LogWarning($"No resources of type {resourceType} found.");
                return null;
            }
            for (int i = 0; i< Resources[resourceType].Count; i++)
            {
                if (nearestResource == null)
                {
                    // Assign the first resource as the nearest one to avoid null exception
                    nearestResource = Resources[resourceType][i];
                    continue;
                }
                // Finds distance between the last nearest resource and the fromPosition
                float distanceFromLastResource = Vector3.Distance(fromPosition, nearestResource.transform.position);
                // Finds distance between the current resource and the fromPosition
                float distanceFromCurrentResource = Vector3.Distance(fromPosition, Resources[resourceType][i].transform.position);
                
                // If the distance from the last nearest resource is greater than the current resource, update the nearest resource
                if (distanceFromLastResource > distanceFromCurrentResource)
                {
                    nearestResource = Resources[resourceType][i];
                }
            }
            return nearestResource;
        }
        /// <summary>
        /// Gets all resources of the specified type.
        /// </summary>
        /// <param name="resourceType">Type of resource you want to get.</param>
        /// <returns>List of GameObject of the resource</returns>
        public List<GameObject> GetResourcesFromType(ResourceType resourceType)
        {
            if (Resources == null || !Resources.ContainsKey(resourceType))
            {
                Debug.LogWarning($"No resources of type {resourceType} found.");
                return new List<GameObject>();
            }
            return Resources[resourceType];
        }
        /// <summary>
        /// Gets all resources of every type.
        /// </summary>
        /// <returns>List of GameObject of all the resource</returns>
        public List<GameObject> GetAllResources()
        {
            List<GameObject> allResources = new();
            
            //iterates through all the resources types in the dictionary
            foreach (List<GameObject> resourceList in Resources.Values)
            {
                // AddRange unpacks the list and adds all the GameObjects to the allResources list
                allResources.AddRange(resourceList);
            }
            return allResources;
        }
        /// <summary>
        /// Adds a resource of a specified type to the ResourceManager.
        /// </summary>
        /// <param name="resourceType"> Type to resource you want to add</param>
        /// <param name="resourceObject"> gameObject you're adding to the list</param>
        public void AddResource(ResourceType resourceType, GameObject resourceObject)
        {
            if (!Resources.ContainsKey(resourceType))
            {
                Resources[resourceType] = new();
            }

            Resources[resourceType].Add(resourceObject);
        }
    }

    /// <summary>
    /// Types of Resources available.
    /// </summary>
    public enum ResourceType
    {
        Wood,
        Stone,
        Steel,
        Food
    }
}
