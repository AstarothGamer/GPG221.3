using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Resource;

namespace Goap
{
    public class GettingWoodAction : BaseGatherAction
    {
        public override ResourceType TargetType => ResourceType.Wood;
    }
}
