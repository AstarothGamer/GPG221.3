using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Resource;

namespace Goap
{
    public class DepositFoodAction : BaseDepositAction
    {
        public override ResourceType TargetType => ResourceType.Food;
    }
}
