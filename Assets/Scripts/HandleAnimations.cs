using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

public class HandleAnimations : NetworkBehaviour {
    Animator anim;
    
    StateManager states;
    Vector3 lookDirection;
    
    private void Start()
    {        
        states = GetComponent<StateManager>();
        states.OnJumpLanded += EndJumpAnim;
        states.OnJumpStarted += DoJumpAnim;
        states.playerHealth.OnDamaged += DoDamagedAnim;
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
        
        anim.SetFloat("Forward", states.vertical * (states.walk || states.aiming ? 0.5f : 1), 0.1f, Time.deltaTime);
        anim.SetFloat("Sideways", states.horizontal * (states.walk || states.aiming ? 0.5f : 1), 0.1f, Time.deltaTime);

        anim.SetBool("Crouching", states.crouching);

    }

    public void StartReload()
    {
        if (!states.reloading)
        {
            anim.SetTrigger("Reload");
        }
    }

    private void DoJumpAnim()
    {
        anim.SetTrigger("Jump");
        anim.SetBool("Jumping", true);
    }

    private void EndJumpAnim()
    {
        anim.SetBool("Jumping", false);
    }

    private void DoDamagedAnim(Attack attack)
    {
        anim.SetTrigger("PlayerDamaged");
    }
}
