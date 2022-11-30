using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractAction : BaseAction
{
    private int maxInteractDistance = 1;

    private void Update()
    {
        if(!isActive)
        {
            return;
        }
    }

    public override string GetActionName()
    {
        return "Interact";
    }

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        IInteractable interactable = LevelGrid.Instance.GetInteractableAtGridPosition(gridPosition);
        interactable.Interact(OnInteractComplete);
        ActionStart(onActionComplete);
    }

    public override List<GridPosition> GetValidActionGridPositions()
    {
        List<GridPosition> validGridPositions = new List<GridPosition>();
        GridPosition unitGridPosition = unit.GetGridPosition();

        for (int x = -maxInteractDistance; x <= maxInteractDistance; ++x)
        {
            for (int z = -maxInteractDistance; z <= maxInteractDistance; ++z)
            {
                GridPosition offset = new GridPosition(x, z);
                GridPosition testGridPosition = unitGridPosition + offset;

                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
                {
                    // Invalid Moves
                    continue;
                }

                IInteractable interactable = LevelGrid.Instance.GetInteractableAtGridPosition(testGridPosition);
                if (interactable == null)
                {
                    // No door at the test position
                    continue;
                }
                validGridPositions.Add(testGridPosition);
            }
        }

        return validGridPositions;
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        return new EnemyAIAction()
        {
            gridPosition = gridPosition,
            actionValue = 0
        };
    }

    private void OnInteractComplete()
    {
        ActionComplete();
    }
}
