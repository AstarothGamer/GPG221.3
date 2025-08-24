using UnityEngine;
using Resource;

namespace Goap
{
    public enum EffectKind
    {
        Named,
        ResourceDelta
    }

    [System.Serializable]
    public class Effect
    {
        public EffectKind kind = EffectKind.Named;

        public string name;

        public ResourceType resourceType;
        public int amount;
    }
}

