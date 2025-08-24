using UnityEngine;
using Resource;

namespace Goap
{
    public enum PrereqKind { Named, ResourceAmount }

    [System.Serializable]
    public class Prerequisite
    {
        public PrereqKind kind = PrereqKind.Named;

        public string name;

        public ResourceType resourceType;
        public int minAmount;
    }
}
