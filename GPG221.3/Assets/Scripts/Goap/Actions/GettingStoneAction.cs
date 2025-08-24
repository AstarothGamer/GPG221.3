using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Resource;

namespace Goap
{
    public class GettingStoneAction : BaseGatherAction
    {
        public override ResourceType TargetType => ResourceType.Stone;

        protected override void Awake()
        {
            base.Awake();
            if (effects == null || effects.Count == 0)
                effects = new List<Effect>
                {
                    new Effect { kind = EffectKind.ResourceDelta,
                        resourceType = ResourceType.Stone, amount = amountPerTrip }
                };
        }
    }
}
