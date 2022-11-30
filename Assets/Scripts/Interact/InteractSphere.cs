using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractSphere : MonoBehaviour, IInteractable
{
    [SerializeField] private Material greenMaterial;
    [SerializeField] private Material redMaterial;
    [SerializeField] private MeshRenderer meshRenderer;

    private GridPosition gridPosition;
    private Action onInteractComplete;
    private bool isActive;
    private float timer;


    private bool isGreen;

    private void Start()
    {
        gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        LevelGrid.Instance.SetInteractableAtGridPosition(gridPosition, this);

        SetColourGreen();
    }

    private void Update()
    {
        if (!isActive)
        {
            return;
        }

        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            isActive = false;
            onInteractComplete();
        }
    }

    private void SetColourGreen()
    {
        isGreen = true;
        meshRenderer.material = greenMaterial;
    }

    private void SetColourRed()
    {
        isGreen = false;
        meshRenderer.material = redMaterial;
    }

    public void Interact(Action onInteractionComplete)
    {
        this.onInteractComplete = onInteractionComplete;
        isActive = true;
        timer = .5f;

        if (isGreen)
            SetColourRed();
        else
            SetColourGreen();
    }
}