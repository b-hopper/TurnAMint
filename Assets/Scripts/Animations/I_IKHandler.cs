using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.FinalIK;

public interface I_IKHandler {

    IKValues IKValues { get; set; }
    
    void PickUp(InteractionObject itemToPickup);    
}

public class IKValues
{
    public LookIK lookIK;
    public AimIK aimIK;
    public HandIK leftHandIK;
    public HandIK rightHandIK;

    public IKValues()
    {
        lookIK = new LookIK(null, 0);
        aimIK = new AimIK(null, 0);
        leftHandIK = new HandIK(null, 0, 0);
        rightHandIK = new HandIK(null, 0, 0);
    }

    public IKValues(LookIK newLookIK, AimIK newAimIK, HandIK newLeftHandIK, HandIK newRightHandIK)
    {
        lookIK = newLookIK;
        aimIK = newAimIK;
        leftHandIK = newLeftHandIK;
        rightHandIK = newRightHandIK;
    }
    
    public class LookIK
    {
        public LookIK(Transform newTarget, float newWeight)
        {
            target = newTarget;
            weight = newWeight;
        }

        public Transform target;
        public float weight;
    }

    public class AimIK
    {
        public AimIK(Transform newTarget, float newWeight)
        {
            target = newTarget;
            weight = newWeight;
        }

        public Transform target;
        public float weight;
    }

    public class HandIK
    {
        public HandIK(Transform newTarget, float newPositionWeight, float newRotationWeight)
        {
            target = newTarget;
            positionWeight = newPositionWeight;
            rotationWeight = newRotationWeight;
        }

        public Transform target;
        public float positionWeight;
        public float rotationWeight;
    }
}