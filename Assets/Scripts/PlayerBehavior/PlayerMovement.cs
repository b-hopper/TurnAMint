using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

    InputHandler ih;
    StateManager state;
    Rigidbody rb;
    CameraManager view;
    
    public float runSpeed = 3;
    public float sprintSpeed = 4;
    public float walkSpeed = 1.5f;
    public float crouchSpeedMultiplier = 0.7f;
    public float aimSpeed = 1;
    public float speedMultiplier = 10;
    public float rotateSpeedY = 100;
    public float rotateSpeedX = 100;
    public float legStraightenSpeed;

    Vector3 lastForward, lastRight;

    public float minY, maxY, mouseXFreeLookLimit;

    float horizontal;
    float vertical;
    public float mouseY, mouseX, bufferedMouseX, bufferedMouseY, mouseXClampValMax, mouseXClampValMin;

    Vector3 rot;

    bool straighteningLegs;
    
    PhysicMaterial zFriction;
    PhysicMaterial mFriction;
    CapsuleCollider col;

    private void Awake()
    {
        ih = GetComponent<InputHandler>();
        rb = GetComponent<Rigidbody>();
        state = GetComponent<StateManager>();
        col = GetComponent<CapsuleCollider>();
        view = GetComponent<CameraManager>();
    }

    private void Start()
    {
        zFriction = new PhysicMaterial();
        zFriction.dynamicFriction = 0;
        zFriction.staticFriction = 0;

        mFriction = new PhysicMaterial();
        mFriction.dynamicFriction = 1;
        mFriction.staticFriction = 1;

        if (view != null)
        {
            view.OnFreeLook += HandleOnFreeLook;
        }
    }
    
    private void FixedUpdate()
    {        
        // Handle movement
        horizontal = state.horizontal;
        vertical = state.vertical;

        bool onGround = state.onGround;

        if (horizontal != 0 || vertical != 0 || !onGround)
        {
            col.material = zFriction;
        }
        else
        {
            col.material = mFriction;
        }

        Vector3 v = (state.freelook) ? lastForward * vertical : ih.cameraActualTrans.forward * vertical;
        Vector3 h = (state.freelook) ? lastRight * horizontal : ih.cameraActualTrans.right * horizontal;

        h.y = 0;
        v.y = 0;

        mouseY += ih.mouseY * Time.deltaTime * rotateSpeedY;
        mouseY = Mathf.Clamp(mouseY, minY, maxY);

        mouseX += ih.mouseX * Time.deltaTime * rotateSpeedX;
        mouseX = (state.freelook ? Mathf.Clamp(mouseX, mouseXClampValMin, mouseXClampValMax) : mouseX) % 360;
        
        HandleMovement(h, v, onGround);
        HandleRotation(h, v, onGround);

        if (onGround)
        {
            rb.drag = 4;
        }
        else
        {
            rb.drag = 0;
        }
        if (straighteningLegs)
        {
            StraightenLegsALittle(legStraightenSpeed);
        }

        if (!state.freelook)
        {
            lastForward = ih.cameraActualTrans.forward;
            lastRight = ih.cameraActualTrans.right;
        }

    }

    void StraightenLegs(float angle)
    {
        straighteningLegs = true;
        if (state.handleAnim != null)
        {
            state.handleAnim.TurnLegs(angle);
        }
    }

    void StraightenLegsALittle(float speed)
    {
        rot = new Vector3(0, transform.eulerAngles.y, 0);
        Vector3 targetRot = new Vector3(ih.camRotation.eulerAngles.x, ih.camRotation.eulerAngles.y, 0);

        // Move the player a little...
        transform.rotation = Quaternion.Lerp(Quaternion.Euler(rot), Quaternion.Euler(new Vector3(0, targetRot.y, 0)), Time.deltaTime * speed);
        // Then move the camera target opposite that to keep crosshairs on target.
        ih.camRotation.rotation = Quaternion.Lerp(Quaternion.Euler(targetRot), Quaternion.Euler(ih.camRotation.eulerAngles), Time.deltaTime * speed);

        if (Mathf.Abs(ih.camRotation.localEulerAngles.y) < 10 || Mathf.Abs(ih.camRotation.localEulerAngles.y) > 350)
        {
            straighteningLegs = false;
        }
    }

    private void HandleRotation(Vector3 h, Vector3 v, bool onGround)
    {
        ih.camRotation.eulerAngles = new Vector3(-mouseY, mouseX, 0);
        if (horizontal == 0 && vertical == 0)
        {
            float angle = GetLookHorizAngle();
            if (Mathf.Abs(angle) > 50 && !state.freelook)
            {
                StraightenLegs(angle);
            }
        }
    }

    private void HandleOnFreeLook(bool val)
    {
        if (val)
        {
            float angle = GetLookHorizAngle();
            if (Mathf.Abs(angle) > 30)
            {
                StraightenLegs(angle);
            }
            mouseXClampValMax = (mouseX + mouseXFreeLookLimit) % 360;
            mouseXClampValMin = (mouseX - mouseXFreeLookLimit) % 360;
            bufferedMouseX = mouseX;
            bufferedMouseY = mouseY;  
        }
        else
        {
            mouseX = bufferedMouseX;
            mouseY = bufferedMouseY;
        }
    }

    private float GetLookHorizAngle()
    {
        float angle = ih.camRotation.transform.localEulerAngles.y - (ih.camRotation.transform.localEulerAngles.y > 180 ? 360 : 0);
        
        state.legsAngle = angle;
                
        return angle;
    }

    public void HandleJump(float jumpStrength)
    {
        rb.AddForce(Vector3.up * jumpStrength, ForceMode.Impulse);        
    }

    private void HandleMovement(Vector3 h, Vector3 v, bool onGround)
    {
        if (CanMove(onGround))
        {
            rb.AddForce((v + h).normalized * speed());
            if ((horizontal != 0 || vertical != 0) && Mathf.Abs(GetLookHorizAngle()) > 10 && !state.freelook)
            {
                rot = new Vector3(0, transform.eulerAngles.y, 0);
                StraightenLegsALittle(5);
            }
        }
    }

    private bool CanMove(bool onGround)
    {
        return (onGround && state.canMove);

    }

    private float speed()
    {
        float speed = 0;
        
        if (state.aiming)
        {
            speed = aimSpeed;
        }
        else
        {
            if (state.walk || state.reloading)
            {
                speed = walkSpeed;
            }
            else if (state.sprint && state.vertical > 0)
            {
                speed = sprintSpeed;
            }
            else
            {
                speed = runSpeed;
            }

            if (state.crouching)
            {
                speed *= crouchSpeedMultiplier;
            }
        }

        speed *= speedMultiplier;

        return speed;
    }
}
