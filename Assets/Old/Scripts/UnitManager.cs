using System.Collections.Generic;
using NPC;
using UnityEngine;

public class UnitManager : Singleton<UnitManager>
{
    public Unit SelectedUnit;

    public List<Unit> Units = new List<Unit>();

    public void AddUnit(Unit unit)
    {
        if(!Units.Contains(unit))
            Units.Add(unit);
    }
    public void RemoveUnit(Unit unit)
    {
        Units.Remove(unit);
    }
}
