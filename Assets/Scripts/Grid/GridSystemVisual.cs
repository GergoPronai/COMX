using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSystemVisual : MonoBehaviour
{
    [Serializable]
    public struct GridVisualTypeMaterial
    {
        public GridVisualType gridVisualType;
        public Material material;
    }

    public enum GridVisualType
    {
        White,
        Blue,
        Red,
        RedSoft,
        Yellow
    }

    public static GridSystemVisual Instance { get; private set; }

    [SerializeField] Transform gridSystemVisualSinglePrefab;
    [SerializeField] private List <GridVisualTypeMaterial> gridVisualTypeMaterials;

    private GridSystemVisualSingle[,] gridSystemVisuals;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There is more than one Unit Action System!" + transform + " - " + Instance);
            Destroy(Instance);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        gridSystemVisuals = new GridSystemVisualSingle[
            LevelGrid.Instance.GetWidth(),
            LevelGrid.Instance.GetHeight()
        ];

        for (int x = 0; x < LevelGrid.Instance.GetWidth(); ++x)
        {
            for (int z = 0; z < LevelGrid.Instance.GetHeight(); ++z)
            {
                GridPosition gridPosition = new GridPosition(x, z);

                Transform gridSystemVisualSingleTransform = Instantiate(gridSystemVisualSinglePrefab, LevelGrid.Instance.GetWorldPosition(gridPosition), Quaternion.identity);
                gridSystemVisuals[x, z] = gridSystemVisualSingleTransform.GetComponent<GridSystemVisualSingle>();
            }
        }

        UnitActionSystem.Instance.OnSelectedActionChange += UnitActionSystem_OnSelectedActionChange;
        LevelGrid.Instance.OnAnyUnitMovePosition += LevelGrid_OnAnyUnitMovePosition;

        UpdateGridVisual();
    }

    public void HideAllGridPosition()
    {
        foreach (var gridSystemVisual in gridSystemVisuals)
        {
            gridSystemVisual.Hide();
        }
    }

    private void ShowGridPositionRange(GridPosition gridPosition, int range, GridVisualType gridVisualType)
    {
        List<GridPosition> gridPositions = new List<GridPosition>();
        for (int x = -range; x <= range; ++x)
        {
            for (int z = -range; z <= range; ++z)
            {
                GridPosition testGridPosition = gridPosition + new GridPosition(x, z);

                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
                {
                    continue;
                }

                int testDistance = Mathf.Abs(x) + Mathf.Abs(z);
                if (testDistance > range)
                {
                    continue;
                }

                gridPositions.Add(testGridPosition);
            }
        }

        ShowGridPositionList(gridPositions, gridVisualType);
    }

    private void ShowGridPositionRangeSquare(GridPosition gridPosition, int range, GridVisualType gridVisualType)
    {
        List<GridPosition> gridPositions = new List<GridPosition>();
        for (int x = -range; x <= range; ++x)
        {
            for (int z = -range; z <= range; ++z)
            {
                GridPosition testGridPosition = gridPosition + new GridPosition(x, z);

                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
                {
                    continue;
                }

                gridPositions.Add(testGridPosition);
            }
        }

        ShowGridPositionList(gridPositions, gridVisualType);
    }

    public void ShowGridPositionList(List<GridPosition> gridPositions, GridVisualType gridVisualType)
    {
        foreach (var gridPosition in gridPositions)
        {
            gridSystemVisuals[gridPosition.x, gridPosition.z].Show(GetGridVisualTypeMaterial(gridVisualType));
        }
    }

    private void UpdateGridVisual()
    {
        HideAllGridPosition();

        Unit selectedUnit = UnitActionSystem.Instance.GetSelectedUnit();
        BaseAction selectedAction = UnitActionSystem.Instance.GetSelectedAction();

        GridVisualType gridVisualType;

        switch (selectedAction)
        {
            case MoveAction moveAction:
            {
                gridVisualType = GridVisualType.White;
                break;
            }
            case SpinAction spinAction:
            {
                gridVisualType = GridVisualType.Blue;
                break;
            }
            case ShootAction shootAction:
            {
                gridVisualType = GridVisualType.Red;
                ShowGridPositionRange(selectedUnit.GetGridPosition(), shootAction.GetMaxShootDistance(), GridVisualType.RedSoft);
                break;
            }
            case GrenadeAction grenadeAction:
            {
                gridVisualType = GridVisualType.Red;
                break;
            }    
            case SwardAction swardAction:
            {
                gridVisualType = GridVisualType.Red;
                ShowGridPositionRangeSquare(selectedUnit.GetGridPosition(), swardAction.GetMaxSwardDistance(), GridVisualType.RedSoft);
                break;
            }      
            case InteractAction interactAction:
            {
                gridVisualType = GridVisualType.Blue;
                break;
            }    
            default:
            {
                gridVisualType = GridVisualType.White;
                break;
            }

        }
        ShowGridPositionList(selectedAction.GetValidActionGridPositions(), gridVisualType);
    }

    private void UnitActionSystem_OnSelectedActionChange(object sender, EventArgs e)
    {
        UpdateGridVisual();
    }

    private void LevelGrid_OnAnyUnitMovePosition(object sender, EventArgs e)
    {
        UpdateGridVisual();
    }

    private Material GetGridVisualTypeMaterial(GridVisualType gridVisualType)
    {
        foreach (GridVisualTypeMaterial gridVisualTypeMaterial in gridVisualTypeMaterials)
        {
            if (gridVisualTypeMaterial.gridVisualType == gridVisualType)
            {
                return gridVisualTypeMaterial.material;
            }
        }
        Debug.LogError($"Could not find GridVisualTypeMaterial for GridVisualType + {gridVisualType}");
        return null;
    }
}
