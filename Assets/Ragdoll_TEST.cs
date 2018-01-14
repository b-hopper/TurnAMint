using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ragdoll_TEST : MonoBehaviour {

    Rigidbody[] bodyParts;
    Animator animator;
    HealthManagement.HealthManager hm;

    MoveController move;

    private void Awake()
    {
        hm = GetComponent<HealthManagement.HealthManager>();
        move = GetComponent<MoveController>();
        animator = GetComponent<Animator>();
        bodyParts = GetComponentsInChildren<Rigidbody>();
        
        EnableRagdoll(false);
    }
        
    void EnableRagdoll(bool value)
    {

        for (int i = 0; i < bodyParts.Length; i++)
        {
            bodyParts[i].isKinematic = !value;
        }
        animator.enabled = !value;
    }

    private void Update()
    {
        if (!hm.IsAlive)
        {
            return;
        }
        //animator.SetBool("IsWalking", true);
        animator.SetFloat("Vertical", 1);

        move.Move(new Vector2(5, 0));
    }

    public void EnableRagdoll()
    {
        if (bodyParts != null)
        {
            EnableRagdoll(true);
        }
    }

    public void DisableRagdoll()
    {
        if (bodyParts != null)
        {
            EnableRagdoll(false);
        }
    }


}
