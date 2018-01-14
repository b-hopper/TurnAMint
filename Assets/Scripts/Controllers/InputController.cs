﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour {
    
    [HideInInspector] public float Vertical, Horizontal;    
    [HideInInspector] public Vector2 MouseInput;
    [HideInInspector] public bool Fire1, Fire2;
    [HideInInspector] public bool Reload;
    [HideInInspector] public bool IsWalking, IsSprinting, IsCrouched;
    [HideInInspector] public bool MouseWheelUp, MouseWheelDown;

    public event System.Action OnCameraViewChanged;

    private void Update()
    {
        Vertical = Input.GetAxis("Vertical");
        Horizontal = Input.GetAxis("Horizontal");
        MouseInput = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
        Fire1 = Input.GetButton("Fire1");
        Fire2 = Input.GetButton("Fire2");
        Reload = Input.GetKey(KeyCode.R);
        IsWalking = Input.GetKey(KeyCode.LeftControl);
        IsSprinting = Input.GetKey(KeyCode.LeftShift);
        IsCrouched = Input.GetKey(KeyCode.C);
        MouseWheelUp = Input.GetAxis("Mouse ScrollWheel") > 0;
        MouseWheelDown = Input.GetAxis("Mouse ScrollWheel") < 0;
        
        /*if (Input.GetKeyDown(KeyCode.V))
        {
            if (OnCameraViewChanged != null)
            {
                OnCameraViewChanged();
            }
        }*/
    }
}
