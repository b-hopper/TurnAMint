using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler : MonoBehaviour {

    public float horizontal;
    public float vertical;
    public float mouse1;
    public float mouse2;
    public float fire3;
    public float middleMouse;
    public float mouseX;
    public float mouseY;

    [HideInInspector]
    public FreeCameraLook camProperties;
    [HideInInspector]
    public Transform camPivot;
    [HideInInspector]
    public Transform camTrans;

    CrosshairManager crosshairManager;
    ShakeCamera shakeCam;
    StateManager states;

    public float normalFov = 60;
    public float aimingFov = 40;
    float targetFov;
    float curFov;
    public float cameraNormalZ = -2;
    public float cameraAimingZ = -0.86f;
    float targetZ;
    float actualZ;
    float curZ;
    LayerMask layerMask;

    public float shakeRecoil = 0.5f;
    public float shakeMovement = 0.3f;
    public float shakeMin = 0.1f;
    float targetShake;
    float curShake;

    private void Start()
    {
        crosshairManager = CrosshairManager.GetInstance();
        camProperties = FreeCameraLook.GetInstance();

        camPivot = camProperties.transform.GetChild(0);
        camTrans = camPivot.GetChild(0);
        shakeCam = camPivot.GetComponentInChildren<ShakeCamera>();

        states = GetComponent<StateManager>();

        layerMask = ~(1 << gameObject.layer);
        states.layerMask = layerMask;
    }

    private void FixedUpdate()
    {
        HandleInput();
        UpdateStates();
        HandleShake();

        // Find where the camera is looking
        Ray ray = new Ray(camTrans.position, camTrans.forward);
        states.lookPosition = ray.GetPoint(20);
    }

    private void HandleShake()
    {
        throw new NotImplementedException();
    }

    private void UpdateStates()
    {
        throw new NotImplementedException();
    }

    private void HandleInput()
    {
        throw new NotImplementedException();
    }
}
