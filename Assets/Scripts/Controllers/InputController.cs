using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour {
    
    [HideInInspector]
    public float Vertical, Horizontal;
    
    [HideInInspector]
    public Vector2 MouseInput;

    [HideInInspector]
    public bool Fire1;

    [HideInInspector]
    public bool Reload;

    private void Update()
    {
        Vertical = Input.GetAxis("Vertical");
        Horizontal = Input.GetAxis("Horizontal");
        MouseInput = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
        Fire1 = Input.GetButton("Fire1");
        Reload = Input.GetKey(KeyCode.R);
    }
}
