using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerMovement : NetworkBehaviour {

    InputHandler ih;
    StateManager states;
    Rigidbody rb;

    Vector3 lookPosition;
    Vector3 storeDirection;

    public float runSpeed = 3;
    public float walkSpeed = 1.5f;
    public float aimSpeed = 1;
    public float speedMultiplier = 10;
    public float rotateSpeed = 2;
    public float turnSpeed = 5;

    float horizontal;
    float vertical;

    Vector3 lookDirection;

    PhysicMaterial zFriction;
    PhysicMaterial mFriction;
    Collider col;

    private void Start()
    {
        ih = GetComponent<InputHandler>();
        rb = GetComponent<Rigidbody>();
        states = GetComponent<StateManager>();
        col = GetComponent<Collider>();

        zFriction = new PhysicMaterial();
        zFriction.dynamicFriction = 0;
        zFriction.staticFriction = 0;

        mFriction = new PhysicMaterial();
        mFriction.dynamicFriction = 1;
        mFriction.staticFriction = 1;
    }

    private void FixedUpdate()
    {
        if (!isLocalPlayer)
        {
            return;
        }
        lookPosition = states.lookPosition;
        lookDirection = lookPosition - transform.position;

        // Handle movement
        horizontal = states.horizontal;
        vertical = states.vertical;

        bool onGround = states.onGround;

        if (horizontal != 0 || vertical != 0 || !onGround)
        {
            col.material = zFriction;
        }
        else
        {
            col.material = mFriction;
        }

        Vector3 v = ih.camTrans.forward * vertical;
        Vector3 h = ih.camTrans.right * horizontal;

        h.y = 0;
        v.y = 0;

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
    }

    private void HandleRotation(Vector3 h, Vector3 v, bool onGround)
    {
        //if (states.aiming)
        //{
            lookDirection.y = 0;

            Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
            transform.rotation = Quaternion.Slerp(rb.rotation, targetRotation, Time.deltaTime * rotateSpeed);
        //}
        /*else              // Allows for rotation away from look position. Not needed for TurnAMint
        {
            storeDirection = transform.position + h + v;

            Vector3 dir = storeDirection - transform.position;
            dir.y = 0;

            if (horizontal != 0 || vertical != 0)
            {
                float angl = Vector3.Angle(transform.forward, dir);

                if (angl != 0)
                {
                    float angle = Quaternion.Angle(transform.rotation, Quaternion.LookRotation(dir));
                    if (angle != 0)
                    {
                        rb.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), turnSpeed * Time.deltaTime);
                    }
                }
            }
        }*/
    }

    public void HandleJump(float jumpStrength)
    {
        rb.AddForce(Vector3.up * jumpStrength, ForceMode.Impulse);
        
    }

    private void HandleMovement(Vector3 h, Vector3 v, bool onGround)
    {
        if (onGround)
        {
            rb.AddForce((v + h).normalized * speed());
        }
    }

    private float speed()
    {
        float speed = 0;
        
        if (states.aiming)
        {
            speed = aimSpeed;
        }
        else
        {
            if (states.walk || states.reloading)
            {
                speed = walkSpeed;
            }
            else
            {
                speed = runSpeed;
            }
        }

        speed *= speedMultiplier;

        return speed;
    }
}
