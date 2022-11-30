using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ActionButtonUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textMeshPro;
    [SerializeField] private Button button;
    [SerializeField] private GameObject selectedGameObject;

    private BaseAction action;
    public void SetBaseAction(BaseAction action)
    {
        this.action = action;

        textMeshPro.text = action.GetActionName().ToUpper();

        button.onClick.AddListener(() => 
        { 
            UnitActionSystem.Instance.SetSelectedAction(action);
        });
    }

    public void UpdateSelectedVisual()
    {
        BaseAction selectedAction = UnitActionSystem.Instance.GetSelectedAction();
        selectedGameObject.SetActive(selectedAction == action);
    }
}
