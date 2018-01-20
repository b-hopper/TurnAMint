using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
<<<<<<< HEAD
using UnityEngine.Networking;

public class InputHandler : NetworkBehaviour {
=======

public class InputHandler : MonoBehaviour {
>>>>>>> refs/remotes/origin/master

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

    public bool fpsMode;
    bool canSwitch;
    ControllerSwitcher conSwitcher;

    private void Start()
    {
<<<<<<< HEAD
        states = GetComponent<StateManager>();

        layerMask = ~(1 << gameObject.layer);
        states.layerMask = layerMask;

        if (!isLocalPlayer)
        {
            return;
        }

        crosshairManager = CrosshairManager.GetInstance();
        camProperties = FreeCameraLook.GetInstance();

        camProperties.target = transform;

=======
        crosshairManager = CrosshairManager.GetInstance();
        camProperties = FreeCameraLook.GetInstance();

>>>>>>> refs/remotes/origin/master
        camPivot = camProperties.transform.GetChild(0);
        camTrans = camPivot.GetChild(0);
        shakeCam = camPivot.GetComponentInChildren<ShakeCamera>();

<<<<<<< HEAD

=======
        states = GetComponent<StateManager>();

        layerMask = ~(1 << gameObject.layer);
        states.layerMask = layerMask;
>>>>>>> refs/remotes/origin/master

        conSwitcher = ControllerSwitcher.GetInstance();
        if (conSwitcher != null)
        {
            canSwitch = true;
        }
    }

    private void Update()
    {
<<<<<<< HEAD
        if (isLocalPlayer)
        {
            HandleInput();
        }
        UpdateStates();
        if (!isLocalPlayer)
        {
            return;
        }
        HandleShake();
=======
        HandleInput();
        UpdateStates();
        HandleShake();

>>>>>>> refs/remotes/origin/master
        // Find where the camera is looking
        Ray ray = new Ray(camTrans.position, camTrans.forward);
        states.lookPosition = ray.GetPoint(20);
        RaycastHit hit;

        Debug.DrawRay(ray.origin, ray.direction);

        if (Physics.Raycast(ray.origin, ray.direction, out hit, 100, layerMask))
        {
            states.lookHitPosition = hit.point;
        }
        else
        {
            states.lookHitPosition = states.lookPosition;
        }

<<<<<<< HEAD
        if (!fpsMode && isLocalPlayer)
=======
        if (!fpsMode)
>>>>>>> refs/remotes/origin/master
        {

            // Check for obstacles in front of the camera
            CameraCollision(layerMask);

            // Update camera's position
            curZ = Mathf.Lerp(curZ, actualZ, Time.deltaTime * 15);
            camTrans.localPosition = new Vector3(0, 0, curZ);
        }
    }

    private void HandleShake()
    {
        if (states.actualShooting && states.handleShooting.curBullets > 0)
        {
            targetShake = shakeRecoil;
            camProperties.WiggleCrosshairAndCamera(0.2f);
            targetFov += 5;
        }
        else
        {
            if (states.vertical != 0)
            {
                targetShake = shakeMovement;
            }
            else
            {
                if (states.horizontal != 0)
                {
                    targetShake = shakeMovement;
                }
                else
                {
                    targetShake = shakeMin;
                }
            }
        }

        curShake = Mathf.Lerp(curShake, targetShake, Time.deltaTime * 10);
        shakeCam.positionShakeSpeed = curShake;

        curFov = Mathf.Lerp(curFov, targetFov, Time.deltaTime * 5);
        Camera.main.fieldOfView = curFov;
    }

    private void HandleInput()
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");
        mouse1 = Input.GetAxis("Fire1");
        mouse2 = Input.GetAxis("Fire2");
        middleMouse = Input.GetAxis("Mouse ScrollWheel");
        mouseX = Input.GetAxis("Mouse X");
        mouseY = Input.GetAxis("Mouse Y");
        fire3 = Input.GetAxis("Fire3");

        if (canSwitch)
        {
            if (Input.GetKeyUp(KeyCode.V))
            {
                Ray ray = new Ray(camTrans.position, camTrans.forward);
                Vector3 lookPos = ray.GetPoint(20);

                if (!fpsMode)
                {
                    conSwitcher.SwitchToFPS(lookPos);
                }
                else
                {
                    conSwitcher.SwitchToTPS(lookPos);
                }
            }
        }
<<<<<<< HEAD
        if (mouse1 > 0)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
    
=======
    }

>>>>>>> refs/remotes/origin/master
    private void UpdateStates()
    {
        states.aiming = states.onGround && (mouse2 > 0);
        states.canRun = !states.aiming;
        states.walk = (fire3 > 0);

        states.horizontal = horizontal;
        states.vertical = vertical;

        if (states.aiming)
        {
            targetZ = cameraAimingZ; // update target Z position of the camera
            targetFov = aimingFov;

            if (mouse1 > 0.5f && !states.reloading)
            {
                states.shoot = true;
            }
            else
            {
                states.shoot = false;
            }
        }
        else
        {
            states.shoot = false;
            targetZ = cameraNormalZ;
            targetFov = normalFov;
        }
    }

    void CameraCollision(LayerMask layerMask)
    {
        // Do a raycast from the pivot of the camera to the camera
        Vector3 origin = camPivot.TransformPoint(Vector3.zero);
        Vector3 direction = camTrans.TransformPoint(Vector3.zero) - camPivot.TransformPoint(Vector3.zero);
        RaycastHit hit;

        // the distance of the raycast is controlled by if we are aiming or not
        actualZ = targetZ;

        // if an obstacle is found
        if (Physics.Raycast(origin, direction, out hit, Mathf.Abs(targetZ), layerMask))
        {
            // If we hit something, find that distance
            float dis = Vector3.Distance(camPivot.position, hit.point);
            actualZ = -dis; // And the opposite of that is where we want to place the camera.
        }
    }
}
