using UnityEngine;
using System.Collections;

public class StateController : MonoBehaviour {

    public GameObject wc;
    public UIController uc;

    private State state;
    public enum State { BeginMarker, MainMenu, Running, Finished, Paused, EndMarker };

    // Use this for initialization
    void Start() {
        state = State.MainMenu;
    }

    public State GetState() {
        return state;
    }

    public void Transition(int newState) {
        if (newState <= (int)State.BeginMarker || newState >= (int)State.EndMarker) {
            throw new System.Exception("Invalid state change.");
        }

        Transition((State)newState);
    }

    public void Transition(State newState) {
        if (newState == state) return;

        switch (state) {
            case State.MainMenu:
                switch (newState) {
                    case State.Running:
                        StartSim();
                        break;
                    default: throw new System.Exception("Invalid state transition from " + state + " to " + newState + ".");
                }
                break;
            case State.Running:
                switch (newState) {
                    case State.Paused:
                        Pause();
                        break;
                    case State.Finished:
                        EndSim();
                        break;
                    default: throw new System.Exception("Invalid state transition from " + state + " to " + newState + ".");
                }
                break;
            case State.Finished:
                switch (newState) {
                    case State.MainMenu:
                        Restart();
                        break;
                    default: throw new System.Exception("Invalid state transition from " + state + " to " + newState + ".");
                }
                break;
            case State.Paused:
                switch (newState) {
                    case State.Running:
                        UnPause();
                        break;
                    case State.MainMenu:
                        Restart();
                        break;
                    default: throw new System.Exception("Invalid state transition from " + state + " to " + newState + ".");
                }
                break;
        };

        state = newState;
    }

    void StartSim() {
        wc.SetActive(true);
        uc.SetMainMenu(false);
    }

    void EndSim() {

    }

    void Restart() {

    }

    void Pause() {
        Time.timeScale = 0;
        uc.SetPauseMenu(true);
        Debug.Log("Paused.");
    }

    void UnPause() {
        Time.timeScale = 1;
        uc.SetPauseMenu(false);
        Debug.Log("Unpaused.");
    }
}
