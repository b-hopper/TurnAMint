using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerSwitcher : MonoBehaviour {
    public bool fps;
    bool switchController;

    public StateManager fpsController;
    public StateManager tpsController;
    public Transform tpsCamera;
    public Transform fpsCamera;

    public static ControllerSwitcher instance;
    public static ControllerSwitcher GetInstance()
    {
        return instance;
    }

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        if (fps)
        {
            tpsController.gameObject.SetActive(false);
            tpsCamera.gameObject.SetActive(false);
            fpsController.transform.gameObject.SetActive(true);
        }
        else
        {
            tpsController.gameObject.SetActive(true);
            tpsCamera.gameObject.SetActive(true);
            fpsController.gameObject.SetActive(false);
            fpsCamera.gameObject.SetActive(false);
        }
    }

    private void FixedUpdate()
    {
        if (fps)
        {
            tpsController.transform.position = fpsController.transform.position;
        }
        else
        {
            fpsController.transform.position = tpsController.transform.position;
        }
    }

    public void SwitchToFPS(Vector3 lookPos)
    {
        fpsController.transform.position = tpsController.transform.position;
        fpsController.transform.rotation = tpsController.transform.rotation;
        fpsController.lookPosition = lookPos;

        fpsController.gameObject.SetActive(true);
        fpsCamera.transform.gameObject.SetActive(true);

        tpsCamera.gameObject.SetActive(false);
        tpsController.gameObject.SetActive(false);

        fps = true;
    }

    public void SwitchToTPS(Vector3 lookPos)
    {
        tpsController.transform.position = fpsController.transform.position;
        tpsCamera.transform.parent.position = tpsController.transform.position;

        tpsController.lookPosition = lookPos;
        tpsController.transform.rotation = fpsController.transform.rotation;
        tpsCamera.transform.rotation = tpsController.transform.rotation;

        tpsController.gameObject.SetActive(true);
        tpsCamera.gameObject.SetActive(true);

        fpsController.gameObject.SetActive(false);
        fpsCamera.transform.gameObject.SetActive(false);

        fps = false;
    }
}
