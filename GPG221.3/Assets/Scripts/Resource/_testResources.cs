using UnityEngine;
using System.Collections;

namespace Resource
{
    public class _testResources : MonoBehaviour
    {
        //Test for finding nearest resource
        [SerializeField] private GameObject player;

        void Update()
        {
            // if (Input.GetKeyDown(KeyCode.R))
            // {
            //     // NearestResourceTest();
            //     // StartCoroutine(ConsumeStockTest());
            //     // HighlightAllResourcesOfTypeTest(ResourceType.Food);
            //     // HighlightAllResourcesTest();
            // }
        }


        // test for finding nearest resource
        void NearestResourceTest()
        {
            GameObject nearest =
                ResourceManager.Instance.GetNearestResource(ResourceType.Wood, player.transform.position);
            if (nearest)
            {
                nearest.GetComponent<SpriteRenderer>().color = Color.red;
                Debug.Log("Nearest Wood Resource found at: " + nearest.transform.position);
            }
            else
                Debug.Log("No Wood Resource found");
        }
        // result: nearest tree found and highlighted in red

        //consume stock test
        IEnumerator ConsumeStockTest()
        {
            Debug.Log("Starting Consume Stock Test");
            GameObject nearest =
                ResourceManager.Instance.GetNearestResource(ResourceType.Wood, player.transform.position);
            nearest.GetComponent<SpriteRenderer>().color = Color.red;
            var wood = nearest.GetComponent<Wood>();
            Debug.Log("Wood Stock at start: " + wood.StockPile);
            while (wood.StockPile > 0)
            {
                wood.ConsumeStock(1);
                Debug.Log("Wood Stock left: " + wood.StockPile);
                yield return new WaitForSeconds(0.5f);
            }
        }
        // result: initial stock 30, depletes to 0, tree object destroyed

        // highlight all resources of a type
        void HighlightAllResourcesOfTypeTest(ResourceType type)
        {
            var resources = ResourceManager.Instance.GetResourcesFromType(type);
            foreach (var resource in resources)
            {
                resource.GetComponent<SpriteRenderer>().color = Color.red;
            }
        }
        // result: all food resources highlighted in red
        
        //highlight all resources
        void HighlightAllResourcesTest()
        {
            var allResources = ResourceManager.Instance.GetAllResources();
            foreach (var resource in allResources)
            {
                resource.GetComponent<SpriteRenderer>().color = Color.red;
            }
        }
        // result: all resources highlighted in red
    }
}
