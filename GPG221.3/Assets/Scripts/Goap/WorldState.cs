using System.Collections.Generic;
using UnityEngine;

namespace Goap
{
    public class WorldState : MonoBehaviour
    {
        public List<Effect> receivedEffects;

        void Start()
        {
            receivedEffects.Add(new Effect { name = "Sleeping" });
        }
        public void AddEffect(Effect effect)
        {
            if (!receivedEffects.Exists(e => e.name == effect.name))
            {
                receivedEffects.Add(effect);
            }
        }
        public void RemoveEffect(Effect effect)
        {
            receivedEffects.RemoveAll(e => e.name == effect.name);
        }
        public bool HasEffect(string effectName)
        {
            return receivedEffects.Exists(e => e.name == effectName);
        }
        public bool HasEnoughValue(Effect effect, int requiredValue)
        {
            if (effect == null) return false;

            Effect existingEffect = receivedEffects.Find(e => e.name == effect.name);
            return existingEffect != null && existingEffect.value >= requiredValue;
        }
    }
}
