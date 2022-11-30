using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MoveAction : BaseAction
{
    public event EventHandler OnStartMoving;
    public event EventHandler OnStopMoving;
    
    [SerializeField] private float moveSpeed;
    [SerializeField] private float rotateSpeed;
    [SerializeField] private int maxMoveDistance;

    private List<Vector3> positions;

    private int currentPositionList;


    private void Update()
    {
        if (!isActive)
        {
            return;
        }

        Vector3 targetPosition = positions[currentPositionList];
        Vector3 moveDirection = (targetPosition - transform.position).normalized;

        transform.forward = Vector3.Lerp(transform.forward, moveDirection, Time.deltaTime * rotateSpeed);

        float stoppingDistance = 0.01f;
        if (Vector3.Distance(targetPosition, transform.position) < stoppingDistance)
        {
            currentPositionList++;
            if(currentPositionList >= positions.Count)
            {
                OnStopMoving?.Invoke(this, EventArgs.Empty);
                ActionComplete();
            }          
        }
        else
        {
            transform.position += moveDirection * Time.deltaTime * moveSpeed;
        }

    }

    public override void TakeAction(GridPosition targetPosition, Action onActionComplete)
    {
        List<GridPosition> pathGridPositions = Pathfinding.Instance.FindPath(unit.GetGridPosition(), targetPosition, out int pathLength);

        currentPositionList = 0;
        positions = new List<Vector3>();

        foreach(GridPosition pathGridPosition in pathGridPositions)
        {
            positions.Add(LevelGrid.Instance.GetWorldPosition(pathGridPosition));
        }

        OnStartMoving?.Invoke(this, EventArgs.Empty);

        ActionStart(onActionComplete);
    }

    public bool IsValidActionGridPosition(GridPosition gridPosition)
    {
        List<GridPosition> validGridPositions = GetValidActionGridPositions();
        return validGridPositions.Contains(gridPosition);
    }

    public override List<GridPosition> GetValidActionGridPositions()
    {
        List<GridPosition> validGridPositions = new List<GridPosition>();

        GridPosition unitGridPosition = unit.GetGridPosition();

        for(int x = -maxMoveDistance; x <= maxMoveDistance; ++x)
        {
            for (int z = -maxMoveDistance; z <= maxMoveDistance; ++z)
            {
                GridPosition offset = new GridPosition(x, z);
                GridPosition testGridPosition = unitGridPosition + offset;

                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition) ||
                    unitGridPosition == testGridPosition ||
                    LevelGrid.Instance.HasAnyUnitOnGridPosition(testGridPosition))
                {
                    // Invalid Moves
                    continue;
                }

                if(!Pathfinding.Instance.IsWalkableGridPosition(testGridPosition))
                {
                    continue;
                }

                if (!Pathfinding.Instance.HasPath(unitGridPosition, testGridPosition))
                {
                    continue;
                }
                int pathfindingDistanceMultiplier = 10;
                if(Pathfinding.Instance.GetPathLength(unitGridPosition, testGridPosition) > maxMoveDistance * pathfindingDistanceMultiplier)
                {
                    continue;
                }

                validGridPositions.Add(testGridPosition);
            }
        }

        return validGridPositions;
    }

    public override string GetActionName()
    {
        return "Move";
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        int targetCountAtGridPosition = unit.GetAction<ShootAction>().GetTargetCountAtPosition(gridPosition);

        return new EnemyAIAction
        {
            gridPosition = gridPosition,
            actionValue = targetCountAtGridPosition * 10
        };
    }
}
