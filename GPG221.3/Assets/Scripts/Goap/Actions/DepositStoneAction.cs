using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Resource;

namespace Goap
{
    public class DepositStoneAction : BaseDepositAction
    {
        public override ResourceType TargetType => ResourceType.Stone;
    }
}
