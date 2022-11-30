using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwardAction : BaseAction
{
    public static event EventHandler OnAnySwardHit;

    public event EventHandler OnSwardActionStart;
    public event EventHandler OnSwardActionEnd;

    private enum State
    {
        SwingingSwardBeforeHit,
        SwingingSwardAfterHit,
    }

    private int maxSwardDistance = 1;
    private State state;
    private float stateTimer;

    private Unit targetUnit;

    private void Update()
    {
        if(!isActive)
        {
            return;
        }

        stateTimer -= Time.deltaTime;

        switch (state)
        {
            case State.SwingingSwardBeforeHit:
            {
                TurnTowardsTarget();
                break;
            }

            case State.SwingingSwardAfterHit:
            {
                
                break;
            }
        }

        if (stateTimer <= 0)
        {
            NextState();
        }
    }

    private void NextState()
    {
        switch (state)
        {
            case State.SwingingSwardBeforeHit:
            {
                state = State.SwingingSwardAfterHit;
                float afterHitStateTime = 0.1f;
                stateTimer = afterHitStateTime;
                targetUnit.Damage(100);
                OnAnySwardHit?.Invoke(this, EventArgs.Empty);
                break;
            }

            case State.SwingingSwardAfterHit:
            {
                OnSwardActionEnd?.Invoke(this, EventArgs.Empty);
                ActionComplete();
                break;
            }
        }
    }

    private void TurnTowardsTarget()
    {
        float rotateSpeed = 2f;
        Vector3 aimDirection = (targetUnit.GetWorldPosition() - unit.GetWorldPosition()).normalized;
        transform.forward = Vector3.Lerp(transform.forward, aimDirection, Time.deltaTime * rotateSpeed);
    }

    public override string GetActionName()
    {
        return "Sward";
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        return new EnemyAIAction
        {
            gridPosition = gridPosition,
            actionValue = 0
        };
    }

    public override List<GridPosition> GetValidActionGridPositions()
    {
        List<GridPosition> validGridPositions = new List<GridPosition>();
        GridPosition unitGridPosition = unit.GetGridPosition();

        for (int x = -maxSwardDistance; x <= maxSwardDistance; ++x)
        {
            for (int z = -maxSwardDistance; z <= maxSwardDistance; ++z)
            {
                GridPosition offset = new GridPosition(x, z);
                GridPosition testGridPosition = unitGridPosition + offset;

                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
                {
                    // Invalid Moves
                    continue;
                }

                if (!LevelGrid.Instance.HasAnyUnitOnGridPosition(testGridPosition))
                {
                    continue;
                }

                Unit targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(testGridPosition);
                if (targetUnit.IsEnemy() == unit.IsEnemy())
                {
                    continue;
                }

                validGridPositions.Add(testGridPosition);
            }
        }

        return validGridPositions;
    }

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(gridPosition);

        state = State.SwingingSwardBeforeHit;
        float beforeHitStateTime = 0.7f;
        stateTimer = beforeHitStateTime;

        OnSwardActionStart?.Invoke(this, EventArgs.Empty);

        ActionStart(onActionComplete);
    }

    public int GetMaxSwardDistance()
    {
        return maxSwardDistance;
    }
}
