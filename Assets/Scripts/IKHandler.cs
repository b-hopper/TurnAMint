using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class IKHandler : NetworkBehaviour {

    Animator anim;
    NetworkAnimator netAnim;
    StateManager states;

    public float lookWeight = 1;
    public float bodyWeight = 0.8f;
    public float headWeight = 1;
    public float clampWeight = 1;

    float targetWeight;

    [SyncVar] float netLookWeight;
    [SyncVar] float netTargetWeight;
    [SyncVar] float netRhikWeight;
    [SyncVar] float netLhikWeight;

    public Transform weaponHolder;
    public Transform rightShoulder;

    public Transform overrideLookTarget;

    public Transform rightHandIkTarget;
    public float rightHandIkWeight;

    public Transform leftHandIkTarget;
    public float leftHandIkWeight;

    Vector3 aimHelper;
    [SyncVar] Vector3 netAimHelper;

    [HideInInspector] public bool LHIK_dis_notAiming;

    private void Start()
    {
        anim = GetComponent<Animator>();
        netAnim = GetComponent<NetworkAnimator>();
        states = GetComponent<StateManager>();
        //GetComponent<NetworkIdentity>().AssignClientAuthority(this.GetComponent<NetworkIdentity>().connectionToClient);

    }

    private void FixedUpdate()
    {
        if (rightShoulder == null)
        {
            rightShoulder = anim.GetBoneTransform(HumanBodyBones.RightShoulder);
        }
        else
        {
            weaponHolder.position = rightShoulder.position;
        }

        if (isLocalPlayer)
        {
            if (states.aiming && !states.reloading)                                 // ONLY ALLOWS SHOOTING WHILE AIMING, WILL NEED TO CHANGE
            {
                Vector3 directionTowardsTarget = aimHelper - transform.position;
                float angle = Vector3.Angle(transform.forward, directionTowardsTarget);

                if (angle < 90)
                {
                    targetWeight = 1;
                }
                else
                {
                    targetWeight = 0;
                }
            }
            else
            {
                targetWeight = 0;
            }

            float multiplier = states.aiming ? 5 : 30;

            lookWeight = Mathf.Lerp(lookWeight, targetWeight, Time.deltaTime * multiplier);

            rightHandIkWeight = lookWeight;

            if (!LHIK_dis_notAiming)
            {
                leftHandIkWeight = 1 - anim.GetFloat("LeftHandIKWeightOverride");
            }
            else
            {
                leftHandIkWeight = (states.aiming) ? 1 - anim.GetFloat("LeftHandIKWeightOverride") : 0;
            }

            HandleShoulderRotation();

            if (isServer)
            {
                RpcSetVars();
            }
            else
            {
                CmdSetVars();
            }
        }
        else
        {
            lookWeight = netLookWeight;
            targetWeight = netTargetWeight;
            leftHandIkWeight = netLhikWeight;
            rightHandIkWeight = netRhikWeight;
            aimHelper = netAimHelper;

            HandleShoulderRotation();
        }
    }

    [Command]
    void CmdSetVars()
    {
        netLookWeight = lookWeight;
        netTargetWeight = targetWeight;
        netLhikWeight = leftHandIkWeight;
        netRhikWeight = rightHandIkWeight;
        netAimHelper = aimHelper;
        
    }
    [ClientRpc]
    void RpcSetVars()
    {
        netLookWeight = lookWeight;
        netTargetWeight = targetWeight;
        netLhikWeight = leftHandIkWeight;
        netRhikWeight = rightHandIkWeight;
        netAimHelper = aimHelper;
    }


    private void HandleShoulderRotation()
    {
        aimHelper = Vector3.Lerp(aimHelper, states.lookPosition, Time.deltaTime * 5);
        weaponHolder.LookAt(aimHelper);
        rightHandIkTarget.parent.transform.LookAt(aimHelper);
    }

    private void OnAnimatorIK(int layerIndex)
    {
        anim.SetLookAtWeight(lookWeight, bodyWeight, headWeight, headWeight, clampWeight);
        //netAnim.SetLookAtWeight(lookWeight, bodyWeight, headWeight, headWeight, clampWeight);
        //netAnim.

        Vector3 filterDirection = states.lookPosition;
        //filterDirection.y = offsetY; //if needed
        anim.SetLookAtPosition(
            (overrideLookTarget != null) ? 
            overrideLookTarget.position : filterDirection);

        if (leftHandIkTarget)
        {
            anim.SetIKPositionWeight(AvatarIKGoal.LeftHand, leftHandIkWeight);
            anim.SetIKPosition(AvatarIKGoal.LeftHand, leftHandIkTarget.position);
            anim.SetIKRotationWeight(AvatarIKGoal.LeftHand, leftHandIkWeight);
            anim.SetIKRotation(AvatarIKGoal.LeftHand, leftHandIkTarget.rotation);
        }

        if (rightHandIkTarget)
        {
            anim.SetIKPositionWeight(AvatarIKGoal.RightHand, rightHandIkWeight);
            anim.SetIKPosition(AvatarIKGoal.RightHand, rightHandIkTarget.position);
            anim.SetIKRotationWeight(AvatarIKGoal.RightHand, rightHandIkWeight);
            anim.SetIKRotation(AvatarIKGoal.RightHand, rightHandIkTarget.rotation);
        }

    }
}
