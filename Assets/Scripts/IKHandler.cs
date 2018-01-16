﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKHandler : MonoBehaviour {

    Animator anim;
    StateManager states;

    public float lookWeight = 1;
    public float bodyWeight = 0.8f;
    public float headWeight = 1;
    public float clampWeight = 1;

    float targetWeight;

    public Transform weaponHolder;
    public Transform rightShoulder;

    public Transform overrideLookTarget;

    public Transform rightHandIkTarget;
    public float rightHandIkWeight;

    public Transform leftHandIkTarget;
    public float leftHandIkWeight;

    Transform aimHelper;

    [HideInInspector] public bool LHIK_dis_notAiming;

    private void Start()
    {
        aimHelper = new GameObject("_aimHelper").transform;
        anim = GetComponent<Animator>();
        states = GetComponent<StateManager>();
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

        if (states.aiming && !states.reloading)                                 // ONLY ALLOWS SHOOTING WHILE AIMING, WILL NEED TO CHANGE
        {
            Vector3 directionTowardsTarget = aimHelper.position - transform.position;
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
    }

    private void HandleShoulderRotation()
    {
        aimHelper.position = Vector3.Lerp(aimHelper.position, states.lookPosition, Time.deltaTime * 5);
        weaponHolder.LookAt(aimHelper.position);
        rightHandIkTarget.parent.transform.LookAt(aimHelper.position);
    }

    private void OnAnimatorIK(int layerIndex)
    {
        anim.SetLookAtWeight(lookWeight, bodyWeight, headWeight, headWeight, clampWeight);

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
