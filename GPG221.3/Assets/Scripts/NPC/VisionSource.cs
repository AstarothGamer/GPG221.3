using Sirenix.Utilities;
using UnityEngine;

public class VisionSource : MonoBehaviour
{
    [SerializeField]protected float visionRange = 2f;
    public float VisionRange => visionRange;
    [SerializeField] bool checkOnStart = true;

    private void Awake()
    {
        if(checkOnStart)
            CheckVision();
    }

    public void SetVisionRange(float range)
    {
        visionRange = range;
        CheckVision();
    }

    public void CheckVision()
    {
        GridManager.Instance.GetTilesInCircle(transform.position, visionRange).ForEach(x =>
        {
            if (!x.Discovered)
                x.Discover();
        });
    } 
}
