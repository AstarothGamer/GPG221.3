using UnityEngine;
using Resource;

namespace Goap
{
    public class Effect : MonoBehaviour
    {
        public enum Kind { Named, ResourceDelta }
        public Kind kind = Kind.Named;

        public string effectName;

        public ResourceType resourceType;
        public int amount = 0;
    }
}

