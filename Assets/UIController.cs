using UnityEngine;
using System.Collections;

public class UIController : MonoBehaviour {

    private StateController sc;
    private GameObject menuMain, menuPause;

    public int zoomSensitivity = 5;

    // Use this for initialization
	void Start () {
        sc = GameObject.Find("StateController").GetComponent<StateController>();
        menuMain = GameObject.Find("Canvas/MainMenu");
        menuPause = GameObject.Find("Canvas/PauseMenu");
	}

    public void SetMainMenu(bool set) {
        menuMain.SetActive(set);
    }

    public void SetPauseMenu(bool set) {
        menuPause.SetActive(set);
    }

    void CameraMove() {
        float hValue = Input.GetAxis("Horizontal");
        float vValue = Input.GetAxis("Vertical");
        Camera.main.transform.Translate(new Vector3(hValue, vValue, 0f));
    }

    void CameraZoom() {
        float zoomValue = Input.GetAxis("Mouse ScrollWheel");
        if (zoomValue < 0) {
            Camera.main.orthographicSize += zoomSensitivity;
        } else if (zoomValue > 0) {
            Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize - zoomSensitivity, zoomSensitivity, float.MaxValue);
        }
    }

    void TryPause() {
        if (Input.GetKeyDown(KeyCode.P)) {
            if (sc.GetState() == StateController.State.Paused) {
                sc.Transition(StateController.State.Running);
            } else if (sc.GetState() == StateController.State.Running) {
                sc.Transition(StateController.State.Paused);
            }
        }
    }

	// Update is called once per frame
	void Update () {
        CameraMove();
        CameraZoom();
        TryPause();
	}

    /*
    void OnGUI() {
        Event e = Event.current;
        if (e.isKey && e.keyCode == KeyCode.P) {
            TryPause();
        }
    }*/
}
