using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class HandleAnimations : NetworkBehaviour {
    Animator anim;

    StateManager states;
    Vector3 lookDirection;

    private void Start()
    {
        states = GetComponent<StateManager>();
        SetupAnimator();
    }

    private void SetupAnimator()
    {
        anim = GetComponent<Animator>();

        Animator[] anims = GetComponentsInChildren<Animator>();

        for (int i = 0; i < anims.Length; i++)
        {
            if (anims[i] != anim)
            {
                anim.avatar = anims[i].avatar;
                Destroy(anims[i]);
                break;
            }
        }
    }

    private void FixedUpdate()
    {
        if (!isLocalPlayer)
        {
            return;
        }
        states.reloading = anim.GetBool("Reloading");
        anim.SetBool("Aim", states.aiming);

        if (!states.canRun)
        {
            anim.SetFloat("Forward", states.vertical, 0.1f, Time.deltaTime);
            anim.SetFloat("Sideways", states.horizontal, 0.1f, Time.deltaTime);
        }
        else
        {
            float movement = Mathf.Abs(states.vertical) + Mathf.Abs(states.horizontal);

            bool walk = states.walk;

            movement = Mathf.Clamp(movement, 0, (walk || states.reloading) ? 0.5f : 1);

            anim.SetFloat("Forward", movement, 0.1f, Time.deltaTime);
        }
    }

    public void StartReload()
    {
        if (!states.reloading)
        {
            anim.SetTrigger("Reload");
        }
    }
}
