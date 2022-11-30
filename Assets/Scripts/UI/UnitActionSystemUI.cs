using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UnitActionSystemUI : MonoBehaviour
{
    [SerializeField] private Transform actionButtonPrefab;
    [SerializeField] private Transform actionButtonContainerTransform;
    [SerializeField] private TextMeshProUGUI actionPointText;

    private List<ActionButtonUI> actionButtonUIs;

    private void Awake()
    {
        actionButtonUIs = new List<ActionButtonUI>();
    }

    private void Start()
    {
        UnitActionSystem.Instance.OnSelectedUnitChange += UnitActionSystem_OnSelectedUnitChanged;
        UnitActionSystem.Instance.OnSelectedActionChange += UnitActionSystem_OnSelectedActionChanged;
        UnitActionSystem.Instance.OnActionStart += UnitActionSystem_OnActionStart;
        TurnSystem.Instance.OnTurnChange += TurnSystem_OnTurnChange;
        Unit.OnAnyActionPointsChange += Unit_OnAnyActionPointsChange;

        CreateUnitActionButtons();
        UpdateVisual();
    }

    private void CreateUnitActionButtons()
    {
        foreach (Transform button in actionButtonContainerTransform)
        {
            Destroy(button.gameObject);
        }

        actionButtonUIs.Clear();

        Unit selectedUnit = UnitActionSystem.Instance.GetSelectedUnit();
        foreach (var action in selectedUnit.GetActions())
        {
            Transform actionButtonTransform = Instantiate(actionButtonPrefab, actionButtonContainerTransform);
            ActionButtonUI actionButtonUI = actionButtonTransform.GetComponent<ActionButtonUI>();
            actionButtonUI.SetBaseAction(action);
            actionButtonUIs.Add(actionButtonUI);
        }
    }

    private void UnitActionSystem_OnSelectedUnitChanged(object sender, EventArgs e)
    {
        CreateUnitActionButtons();
        UpdateVisual();
        UpdateActionPoints();
    }

    private void UnitActionSystem_OnSelectedActionChanged(object sender, EventArgs e)
    {
        UpdateVisual();
    }

    private void UpdateVisual()
    {
        foreach (ActionButtonUI actionButtonUI in actionButtonUIs)
        {
            actionButtonUI.UpdateSelectedVisual();
        }
    }


    private void UpdateActionPoints()
    {
        Unit selectedUnit = UnitActionSystem.Instance.GetSelectedUnit();
        int actionPoints = selectedUnit.GetActionPoints();
        actionPointText.text = $"Action Points: {actionPoints}";
    }

    private void UnitActionSystem_OnActionStart(object sender, EventArgs e)
    {
        UpdateActionPoints();
    }

    private void TurnSystem_OnTurnChange(object sender, EventArgs e)
    {
        UpdateActionPoints();
    }

    private void Unit_OnAnyActionPointsChange(object sender, EventArgs e)
    {
        UpdateActionPoints();
    }
}
