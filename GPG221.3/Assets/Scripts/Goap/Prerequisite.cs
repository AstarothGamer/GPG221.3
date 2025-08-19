using UnityEngine;
using Resource;

namespace Goap
{
    public class Prerequisite : MonoBehaviour
    {
        public enum Kind { Named, ResourceAmount }
        public Kind kind = Kind.Named;

        public string effectName;

        public ResourceType resourceType;
        public int minAmount = 0;
    }
}

