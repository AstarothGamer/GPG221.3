using System.Collections.Generic;
using UnityEngine;

namespace Goap
{
    public class LocalState : MonoBehaviour
    {
        public List<Effect> receivedEffects = new();

        public int wood;
        public int stone;
        public int steel;
        public int food;

        public int woodMax = 20;
        public int stoneMax = 20;
        public int steelMax = 20;
        public int foodMax = 20;
    }
}

