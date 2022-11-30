using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public static event EventHandler OnAnyActionPointsChange;
    public static event EventHandler OnAnyUnitSpawned;
    public static event EventHandler OnAnyUnitDead;

    [SerializeField] private int maximumActionPoints;
    [SerializeField] private bool isEnemy;

    private int actionPoints;

    private BaseAction[] actions;

    private GridPosition gridPosition;

    private HealthSystem healthSystem;

    private void Awake()
    {
        actions = GetComponents<BaseAction>();

        healthSystem = GetComponent<HealthSystem>();

        actionPoints = maximumActionPoints;
    }

    private void Start()
    {
        gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        LevelGrid.Instance.AddUnitAtGridPosition(gridPosition, this);
        TurnSystem.Instance.OnTurnChange += TurnSystem_OnTurnChange;

        healthSystem.OnDead += HealthSystem_OnDead;

        OnAnyUnitSpawned?.Invoke(this, EventArgs.Empty);
    }

    private void Update()
    {
        GridPosition newGridPosition = LevelGrid.Instance.GetGridPosition(transform.position);

        if(newGridPosition != gridPosition)
        {
            // Unit position changed
            GridPosition oldGridPosition = gridPosition;
            gridPosition = newGridPosition;
            LevelGrid.Instance.UnitMovedGridPosition(this, oldGridPosition, newGridPosition);
        }
    }
    
    public T GetAction<T>() where T : BaseAction
    {
        foreach(BaseAction action in actions)
        {
            if(action is T)
            {
                return (T)action;
            }
        }
        return null;
    }

    public BaseAction[] GetActions()
    {
        return actions;
    }

    public GridPosition GetGridPosition()
    {
        return gridPosition;
    }

    public bool TrySpendActionPoints(BaseAction action)
    {
        if(CanSpendActionPoints(action))
        {
            SpendActionPoints(action.GetActionPointsCost());
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool CanSpendActionPoints(BaseAction action)
    {
        return actionPoints >= action.GetActionPointsCost();
    }

    private void SpendActionPoints(int amount)
    {
        actionPoints -= amount;
        OnAnyActionPointsChange?.Invoke(this, EventArgs.Empty);
    }

    public int GetActionPoints()
    {
        return actionPoints;
    }

    private void TurnSystem_OnTurnChange(object sender, EventArgs e)
    {
        if((IsEnemy() && !TurnSystem.Instance.IsPlayerTurn()) ||
            !IsEnemy() && TurnSystem.Instance.IsPlayerTurn())
        actionPoints = maximumActionPoints;

        OnAnyActionPointsChange?.Invoke(this, EventArgs.Empty);
    }

    public bool IsEnemy()
    {
        return isEnemy;
    }

    public void Damage(int damageAmount)
    {
        healthSystem.Damage(damageAmount);
    }

    public Vector3 GetWorldPosition()
    {
        return transform.position;
    }

    private void HealthSystem_OnDead(object sender, EventArgs e)
    {
        LevelGrid.Instance.RemoveUnitAtGridPosition(gridPosition, this);
        OnAnyUnitDead?.Invoke(this, EventArgs.Empty);

        Destroy(gameObject);
    }

    public float GetHealtNormalized()
    {
        return healthSystem.GetHealthNormolized();
    }
}