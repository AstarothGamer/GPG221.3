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
    }
}
