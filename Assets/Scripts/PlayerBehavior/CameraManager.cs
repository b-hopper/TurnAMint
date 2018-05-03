using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour {

    public bool fpsMode;
    public bool canChangeViewmode;

    CameraReferences camRefs;
    public Cinemachine.CinemachineVirtualCamera fpsVirtualCam, tpsVirtualCam, deathCam;
    public Cinemachine.CinemachineBlendDefinition.Style transitionCutStyle;
    public float transitionCutTime;
    public Cinemachine.CinemachineBlendDefinition.Style deathBlendStyle;
    public float deathCamBlendTime;    

    Cinemachine.CinemachineBrain camBrain;

    [HideInInspector] public Vector3 bufferedCamEulerAngles;
    Transform bufferedCamTransform;

    public delegate void BoolViewChangeEvent(bool val);
    public BoolViewChangeEvent OnFreeLook;


    bool wasFreeLooking = true;

    InputHandler input;
    StateManager state;
    HealthManager health;
    IKHandler ikHandler;

    private void Awake()
    {
        camBrain = FindObjectOfType<Cinemachine.CinemachineBrain>();
        camRefs = camBrain.GetComponent<CameraReferences>();
        input = GetComponent<InputHandler>();
        state = GetComponent<StateManager>();
        health = GetComponent<HealthManager>();
        ikHandler = GetComponent<IKHandler>();
    }

    private void Start()
    {
        input.OnViewModeChanged += HandleViewModeChange;

        health.OnKilled += HandleDeathViewMode;
        health.OnReset += ResetCam;

        bufferedCamTransform = input.camRotation;               
    }

    public void SetupCameras()
    {
        if (fpsVirtualCam == null)
        {
            fpsVirtualCam = camRefs.fpsCam;
        }
        if (tpsVirtualCam == null)
        {
            tpsVirtualCam = camRefs.tpsCam;
        }
        if (deathCam == null)
        {
            deathCam = camRefs.deathCam;
        }
        if (input.cameraActualTrans == null)
        {
            input.cameraActualTrans = camRefs.mainCam.transform;
        }

        if (!canChangeViewmode)
        {
            fpsVirtualCam.Priority = 11;
        }
        else
        {
            fpsVirtualCam.Priority = 9;
        }
    }

    private void Update()
    {
        if (state.freelook != wasFreeLooking)
        {
            TurnOnFreeLook(state.freelook);
        }
        wasFreeLooking = state.freelook;
    }

    private void HandleFreeLook(bool val)
    {
        fpsVirtualCam.LookAt = (val) ? state.freeLookCamTarget : state.camLookTarget;
        tpsVirtualCam.LookAt = (val) ? state.freeLookCamTarget : state.camLookTarget;
        tpsVirtualCam.Follow = (val) ? state.freeLookCamPivot : state.tpsCamPivot;
    }

    private void ResetCam()
    {
        camBrain.m_DefaultBlend.m_Style = transitionCutStyle;
        camBrain.m_DefaultBlend.m_Time = transitionCutTime;
        deathCam.Priority = 0;        
    }

    private void HandleDeathViewMode()
    {
        camBrain.m_DefaultBlend.m_Time = deathCamBlendTime;

        camBrain.m_DefaultBlend.m_Style = deathBlendStyle;
        deathCam.Priority = 12;
    }

    private void HandleViewModeChange()
    {
        camBrain.m_DefaultBlend.m_Style = transitionCutStyle;
        if (!fpsMode && canChangeViewmode)
        {
            fpsMode = true;
            fpsVirtualCam.Priority = 11;
        }
        else if (canChangeViewmode)
        {
            fpsMode = false;
            fpsVirtualCam.Priority = 9;
        }
    }

    public void ForceViewModeChange(bool setToFPSMode)
    {
        if (setToFPSMode)
        {
            fpsMode = true;
            fpsVirtualCam.Priority = 11;
        }
        else
        {
            fpsMode = false;
            fpsVirtualCam.Priority = 9;
        }
    }

    public void TurnOnFreeLook(bool val)
    {
        state.freeLookRotation.localEulerAngles = input.camRotation.localEulerAngles;

        if (val)
        {
            bufferedCamEulerAngles = input.camRotation.eulerAngles;
        }
        else
        {
            bufferedCamTransform.eulerAngles = bufferedCamEulerAngles;
        }        

        input.camRotation = val ? state.freeLookRotation : bufferedCamTransform;        

        if (OnFreeLook != null)
        {
            OnFreeLook(val);
        }
        HandleFreeLook(val);
    }
}
