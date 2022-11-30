using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class ShootAction : BaseAction
{
    private enum State
    {
        Aiming,
        Shooting,
        Cooloff
    }

    public class OnShootEventArgs : EventArgs
    {
        public Unit targetUnit;
        public Unit shootingUnit;
    }

    [SerializeField] private LayerMask obstacleLayerMask;

    public static event EventHandler<OnShootEventArgs> OnAnyShoot;
    public event EventHandler<OnShootEventArgs> OnShoot;

    private State state;
    private int maxShootDistance = 7;
    private float stateTimer;
    private bool canShootBullet;

    private Unit targetUnit;


    private void Update()
    {
        if (!isActive)
        {
            return;
        }

        stateTimer -= Time.deltaTime;

        switch (state)
        {
            case State.Aiming:
            {
                TurnTowardsTarget();
                break;
            }

            case State.Shooting:
            {
                if (canShootBullet)
                {
                    Shoot();
                    canShootBullet = false;
                }
                break;
            }

            case State.Cooloff:
            {
                break;
            }
        }

        if (stateTimer <= 0)
        {
            NextState();
        }
    }

    private void Shoot()
    {
        OnAnyShoot?.Invoke(this, new OnShootEventArgs
        {
            targetUnit = targetUnit,
            shootingUnit = unit
        });


        OnShoot?.Invoke(this, new OnShootEventArgs
        {
            targetUnit = targetUnit,
            shootingUnit = unit
        });

        targetUnit.Damage(40);
    }

    private void NextState()
    {
        switch (state)
        {
            case State.Aiming:
            {
                state = State.Shooting;
                float shootingStateTime = 0.1f;
                stateTimer = shootingStateTime;
                break;
            }

            case State.Shooting:
            {
                state = State.Cooloff;
                float cooloffStateTimer = 0.5f;
                stateTimer = cooloffStateTimer;
                break;
            }

            case State.Cooloff:
            {
                ActionComplete();
                break;
            }
        }
    }

    public override string GetActionName()
    {
        return "Shoot";
    }

    public override List<GridPosition> GetValidActionGridPositions()
    {
        GridPosition unitGridPosition = unit.GetGridPosition();
        return GetValidActionGridPositions(unitGridPosition);
    }

    public List<GridPosition> GetValidActionGridPositions(GridPosition unitGridPosition)
    {
        List<GridPosition> validGridPositions = new List<GridPosition>();

        for (int x = -maxShootDistance; x <= maxShootDistance; ++x)
        {
            for (int z = -maxShootDistance; z <= maxShootDistance; ++z)
            {
                GridPosition offset = new GridPosition(x, z);
                GridPosition testGridPosition = unitGridPosition + offset;

                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
                {
                    // Invalid Moves
                    continue;
                }
                int testDistance = Mathf.Abs(x) + Mathf.Abs(z);
                if (testDistance > maxShootDistance)
                {
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
                Vector3 unitWorldPosition = LevelGrid.Instance.GetWorldPosition(unitGridPosition);
                Vector3 shootDirection = (targetUnit.GetWorldPosition() - unitWorldPosition).normalized;

                float unitShoulderHeight = 1.7f;
                if(Physics.Raycast(unitWorldPosition + Vector3.up * unitShoulderHeight, shootDirection, Vector3.Distance(unitWorldPosition, targetUnit.GetWorldPosition()), obstacleLayerMask))
                {
                    // Blocked by a wall
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

        state = State.Aiming;
        float aimingStateTimer = 1.0f;
        stateTimer = aimingStateTimer;

        canShootBullet = true;

        ActionStart(onActionComplete);
    }

    private void TurnTowardsTarget()
    {
        float rotateSpeed = 2f;
        Vector3 aimDirection = (targetUnit.GetWorldPosition() - unit.GetWorldPosition()).normalized;
        transform.forward = Vector3.Lerp(transform.forward, aimDirection, Time.deltaTime * rotateSpeed);
    }

    public Unit GetTargetUnit()
    {
        return targetUnit;
    }

    public int GetMaxShootDistance()
    {
        return maxShootDistance;
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        Unit targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(gridPosition);

        return new EnemyAIAction
        {
            gridPosition = gridPosition,
            actionValue = 100 + Mathf.RoundToInt((1 - targetUnit.GetHealtNormalized()) * 100f)
        };
    }

    public int GetTargetCountAtPosition(GridPosition gridPosition)
    {
        return GetValidActionGridPositions(gridPosition).Count;
    }
}
