using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class StateManager : MonoBehaviour
{
    [SerializeField] protected GameObject gameUI;
    [SerializeField] protected GameObject prepUI;
    [SerializeField] protected GameObject pauseUI;
    [SerializeField] protected GameObject gameOverUI;
    [SerializeField] protected GameObject mapUI;
    [SerializeField] protected GameObject resultsUI;

    private bool DidConfirm;

    //[SerializeField] private Player player;


    public GameState currentState;
    public enum GameState
    {
        PrepState,
        PauseState,
        PlayState,
        GameOverState,
        WinState,
        MapState,
    }

    // Start is called before the first frame update
    private void Start()
    {
        ChangeState(GameState.PrepState);
    }
    public void ChangeState(GameState newState)
    {
        ExitState(currentState);

        currentState = newState;

        EnterState(currentState);
    }
    private void EnterState(GameState state)
    {
        switch (state)
        {
            case GameState.PrepState:
                //Handle Entering State
                Debug.Log("Entering Main Menu");
                break;
            case GameState.PlayState:
                Debug.Log("Entering Play State");
                break;
            case GameState.PauseState:
                Debug.Log("Entering Pause Menu");
                break;
            case GameState.GameOverState:
                Debug.Log("Entering Game Over");
                break;
            case GameState.MapState:
                Debug.Log("Entering Map Menu");
                break;
            case GameState.WinState:
                Debug.Log("You Win!");
                break;

        }
    }
    private void ExitState(GameState state)
    {
        switch (state)
        {
            case GameState.PrepState:
                //Handle Exiting State

                break;
            case GameState.PlayState:

                break;
            case GameState.PauseState:

                break;
            case GameState.GameOverState:

                break;
            case GameState.MapState:

                break;
            case GameState.WinState:

                break;

        }
    }

    public void StartGame()
    {
        currentState = GameState.MapState;
    }
    public void PauseGame()
    {
        // Transition from Play to Pause state
        currentState = GameState.PauseState;
    }


    public void ResumeGame()
    {
        // Transition from Pause to Play state
        currentState = GameState.PlayState;
    }

    public void EndGame()
    {
        // Transition to Game Over state
        currentState = GameState.GameOverState;
        gameOverUI.SetActive(true);
        mapUI.SetActive(false);

    }

    public void ReturnToMenu()
    {
        // Transition to Menu state
        currentState = GameState.PrepState;
    }

    public void OpenMap()
    {
        // Transition to Map state
        currentState = GameState.MapState;
        mapUI.SetActive(true);
        pauseUI.SetActive(false);
        gameUI.SetActive(false);
        //Shows UI button to go back
    }

    public void EnterBattle()
    {
        //transition to battle 
        currentState = GameState.PlayState;
        Debug.Log("Entering Battle");

        //add code to see if boss or not
    }

    public void Victory()
    {
        currentState = GameState.WinState;
        mapUI.SetActive(false);
        pauseUI.SetActive(false);
        gameUI.SetActive(false);
        resultsUI.SetActive(true);

    }

    public void SetState(GameState newState)
    {
        currentState = newState;
    }

    public GameState GetState()
    {
        return currentState;
    }
}
