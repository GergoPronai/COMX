using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnSystem : MonoBehaviour
{
    private bool isPlayerTurn = true;
    
    public event EventHandler OnTurnChange;

    public static TurnSystem Instance { get; private set; }
    private int turnNumber = 1;

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

    public void NextTurn()
    {
        turnNumber++;
        isPlayerTurn = !isPlayerTurn;

        OnTurnChange?.Invoke(this, EventArgs.Empty);
    }

    public int GetTurnNumber()
    {
        return turnNumber;
    }

    public bool IsPlayerTurn()
    {
        return isPlayerTurn;
    }
}
