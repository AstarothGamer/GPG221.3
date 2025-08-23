using System.Collections.Generic;
using UnityEngine;

namespace Goap
{
    public class WorldState : MonoBehaviour
    {
        public List<Effect> receivedEffects = new();

        public int wood;
        public int stone;
        public int steel;
        public int food;
    }
}
