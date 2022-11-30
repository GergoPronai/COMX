using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridObject
{
    private GridSystem<GridObject> gridSystem;
    private GridPosition gridPosition;
    private List<Unit> units;
    private IInteractable interactable;
    
    public GridObject(GridSystem<GridObject> gridSystem, GridPosition gridPosition)
    {
        this.gridSystem = gridSystem;
        this.gridPosition = gridPosition;
        units = new List<Unit>();
    }

    public void AddUnit(Unit unit)
    {
        units.Add(unit);
    }

    public List<Unit> GetUnits()
    {
        return units;
    }

    public void RemoveUnit(Unit unit)
    {
        units.Remove(unit);
    }

    public override string ToString()
    {
        string unitString = "";
        foreach(Unit unit in units)
        {
            unitString += unit + "\n";
        }
        return gridPosition.ToString() + "\n" + unitString;
    }

    public bool HasAnyUnit()
    {
        return units.Count > 0;
    }

    public Unit GetUnit()
    {
        if(HasAnyUnit())
        {
            return units[0];
        }
        else
        {
            return null;
        }
    }

    public IInteractable GetInteractable()
    {
        return interactable;
    }

    public void SetInteractable(IInteractable interactable)
    {
        this.interactable = interactable;
    }
}
