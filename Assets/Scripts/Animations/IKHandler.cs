using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.FinalIK;

public class IKHandler : MonoBehaviour, I_IKHandler
{
    IKValues m_IKValues;
    public IKValues IKValues
    {
        get
        {
            if (m_IKValues == null)
            {
                m_IKValues = new IKValues(
                    new IKValues.LookIK(bipedIK.solvers.lookAt.target, bipedIK.solvers.lookAt.IKPositionWeight),
                    new IKValues.AimIK(bipedIK.solvers.aim.target, bipedIK.solvers.aim.IKPositionWeight), 
                    new IKValues.HandIK(bipedIK.solvers.leftHand.target, bipedIK.solvers.leftHand.IKPositionWeight, bipedIK.solvers.leftHand.IKRotationWeight),
                    new IKValues.HandIK(bipedIK.solvers.rightHand.target, bipedIK.solvers.rightHand.IKPositionWeight, bipedIK.solvers.rightHand.IKRotationWeight));
            }
            else
            {
                m_IKValues.lookIK.target = bipedIK.solvers.lookAt.target;
                m_IKValues.lookIK.weight = bipedIK.solvers.lookAt.IKPositionWeight;
                
                m_IKValues.aimIK.target = bipedIK.solvers.aim.target;
                m_IKValues.aimIK.weight = bipedIK.solvers.aim.IKPositionWeight;
                
                m_IKValues.leftHandIK.target = bipedIK.solvers.leftHand.target;
                m_IKValues.leftHandIK.positionWeight = bipedIK.solvers.leftHand.IKPositionWeight;
                m_IKValues.leftHandIK.rotationWeight = bipedIK.solvers.leftHand.IKRotationWeight;
                
                m_IKValues.rightHandIK.target = bipedIK.solvers.rightHand.target;
                m_IKValues.rightHandIK.positionWeight = bipedIK.solvers.rightHand.IKPositionWeight;
                m_IKValues.rightHandIK.rotationWeight = bipedIK.solvers.rightHand.IKRotationWeight;
            }

            return m_IKValues;
        }
        set { m_IKValues = value; }
    }

    StateManager state;

    BipedIK bipedIK;                // TODO: Create new transforms for temp IK targets, and when target set to null, move temp IK target to current target, and blend 1->0 weight from that temp target to keep movements smooth
    FullBodyBipedIK FBBIK;

    InteractionSystem interactionSystem;

    Transform rightHandSmoother, leftHandSmoother, lookSmoother, aimSmoother;

    AnimationCurve rightHandPickUpCurve;
    Keyframe[] keys;

    public delegate void IKBlendTargetReachedEvent();

    public InteractionObject defaultPickup, rightHandLocationOnPickup;

    bool needsToBlend, removedEffectorThisFrame;

    float storedLHWeight, storedRHWeight, storedAimWeight, storedLookWeight;

    List<Effector> effectorsToBlend;

    private void Awake()
    {
        m_IKValues = new IKValues();
        bipedIK = GetComponent<BipedIK>();
        FBBIK = GetComponent<FullBodyBipedIK>();
        interactionSystem = GetComponent<InteractionSystem>();
        state = GetComponent<StateManager>();
        effectorsToBlend = new List<Effector>();
    }

    private void Start()
    {
        keys = new Keyframe[3];
        keys[0] = new Keyframe(0.0f, 0.0f);
        keys[2] = new Keyframe(0.74f, 0.0f);

        leftHandSmoother = new GameObject("LeftHandSmoother").transform;
        leftHandSmoother.parent = bipedIK.references.root;
        leftHandSmoother.position = Vector3.zero;

        rightHandSmoother = new GameObject("RightHandSmoother").transform;
        rightHandSmoother.parent = bipedIK.references.root;
        rightHandSmoother.position = Vector3.zero;

        aimSmoother = new GameObject("AimSmoother").transform;
        aimSmoother.parent = bipedIK.references.rightHand;
        aimSmoother.position = Vector3.zero;

        lookSmoother = new GameObject("LookSmoother").transform;
        lookSmoother.parent = bipedIK.references.root;
        lookSmoother.position = Vector3.zero;
    }

    private void FixedUpdate()
    {
        if (needsToBlend)
        {
            BlendIK();
        }
    }

    public void EnableAllIK(bool val)
    {
        FBBIK.enabled = val;
        bipedIK.enabled = val;
        /*
        if (!val)
        {
            storedAimWeight = bipedIK.solvers.aim.IKPositionWeight;
            bipedIK.solvers.aim.IKPositionWeight = 0;

            storedLHWeight = bipedIK.solvers.leftHand.IKPositionWeight;
            bipedIK.solvers.leftHand.IKPositionWeight = 0;
            bipedIK.solvers.leftHand.IKRotationWeight = 0;

            storedRHWeight = bipedIK.solvers.rightHand.IKPositionWeight;
            bipedIK.solvers.rightHand.IKPositionWeight = 0;
            bipedIK.solvers.rightHand.IKRotationWeight = 0;

            storedLookWeight = bipedIK.solvers.lookAt.IKPositionWeight;
            bipedIK.solvers.lookAt.IKPositionWeight = 0;
        }
        else
        {
            bipedIK.solvers.aim.IKPositionWeight = storedAimWeight;

            bipedIK.solvers.leftHand.IKPositionWeight = storedLHWeight;
            bipedIK.solvers.leftHand.IKRotationWeight = storedLHWeight;

            bipedIK.solvers.rightHand.IKPositionWeight = storedRHWeight;
            bipedIK.solvers.rightHand.IKRotationWeight = storedRHWeight;

            bipedIK.solvers.lookAt.IKPositionWeight = storedLookWeight;
        }*/
    }
    
    public void BlendAimIK(Transform target, float targetWeight, float blendSpeed, bool changeTarget = true, IKBlendTargetReachedEvent OnIKTargetReachedMethod = null)
    {
        Effector newEffector = new Effector();
        newEffector.aim = bipedIK.solvers.aim;
        newEffector.targetWeight = targetWeight;
        newEffector.blendSpeed = blendSpeed;
        newEffector.startWeight = bipedIK.solvers.aim.IKPositionWeight;
        newEffector.newTarget = target ?? aimSmoother;
        if (bipedIK.solvers.aim.transform != null)
        {
            aimSmoother.position = bipedIK.solvers.aim.transform.position;
            aimSmoother.rotation = bipedIK.solvers.aim.transform.rotation;
        }
        
        newEffector.shouldChangeTarget = changeTarget;
        newEffector.OnIKTargetReached = OnIKTargetReachedMethod;

        for (int i = 0; i < effectorsToBlend.Count; i++)
        {
            if (effectorsToBlend[i].aim != null)
            {
                removedEffectorThisFrame = true;
                effectorsToBlend.Remove(effectorsToBlend[i]);
            }
        }

        needsToBlend = true;

        effectorsToBlend.Add(newEffector);
    }
    
    public void BlendLookAtIK(Transform target, float targetWeight, float blendSpeed, bool changeTarget = true, IKBlendTargetReachedEvent OnIKTargetReachedMethod = null)
    {
        Effector newEffector = new Effector();
        newEffector.look = bipedIK.solvers.lookAt;
        newEffector.targetWeight = targetWeight;
        newEffector.blendSpeed = blendSpeed;
        newEffector.startWeight = bipedIK.solvers.lookAt.IKPositionWeight;
        newEffector.newTarget = target ?? lookSmoother;
        if (bipedIK.solvers.lookAt.target != null) lookSmoother.position = bipedIK.solvers.lookAt.target.position;
        newEffector.shouldChangeTarget = changeTarget;
        newEffector.OnIKTargetReached = OnIKTargetReachedMethod;

        for (int i = 0; i < effectorsToBlend.Count; i++)
        {
            if (effectorsToBlend[i].look != null)
            {
                removedEffectorThisFrame = true;
                effectorsToBlend.Remove(effectorsToBlend[i]);
            }
        }

        needsToBlend = true;

        effectorsToBlend.Add(newEffector);
    }
    
    public void BlendLeftHandIK(Transform target, float targetWeight, float blendSpeed, bool changeTarget = true, IKBlendTargetReachedEvent OnIKTargetReachedMethod = null)
    {
        Effector newEffector = new Effector();
        newEffector.limb = bipedIK.solvers.leftHand;
        newEffector.targetWeight = targetWeight;
        newEffector.blendSpeed = blendSpeed;
        newEffector.startWeight = bipedIK.solvers.leftHand.IKPositionWeight;
        newEffector.newTarget = target ?? leftHandSmoother;//== null ? leftHandSmoother : target;
        if (bipedIK.solvers.leftHand.target != null)
        {
            leftHandSmoother.position = bipedIK.solvers.leftHand.IKPosition;
            leftHandSmoother.rotation = bipedIK.solvers.leftHand.IKRotation;
        }
        newEffector.shouldChangeTarget = changeTarget;
        newEffector.OnIKTargetReached = OnIKTargetReachedMethod;

        for (int i = 0; i < effectorsToBlend.Count; i++)
        {
            if (effectorsToBlend[i].limb == newEffector.limb)
            {
                removedEffectorThisFrame = true;
                effectorsToBlend.Remove(effectorsToBlend[i]);
            }
        }

        needsToBlend = true;

        effectorsToBlend.Add(newEffector);
    }

    public void BlendRightHandIK(Transform target, float targetWeight, float blendSpeed, bool changeTarget = true, IKBlendTargetReachedEvent OnIKTargetReachedMethod = null)
    {
        Effector newEffector = new Effector();
        newEffector.limb = bipedIK.solvers.rightHand;
        newEffector.targetWeight = targetWeight;
        newEffector.blendSpeed = blendSpeed;
        newEffector.startWeight = bipedIK.solvers.rightHand.IKPositionWeight;
        newEffector.newTarget = target ?? rightHandSmoother;
        if (bipedIK.solvers.rightHand.target != null)
        {
            rightHandSmoother.position = bipedIK.solvers.rightHand.IKPosition;
            rightHandSmoother.rotation = bipedIK.solvers.rightHand.IKRotation;
        }
        newEffector.shouldChangeTarget = changeTarget;
        newEffector.OnIKTargetReached = OnIKTargetReachedMethod;
        

        for (int i = 0; i < effectorsToBlend.Count; i++)
        {
            if (effectorsToBlend[i].limb == newEffector.limb)
            {
                removedEffectorThisFrame = true;
                effectorsToBlend.Remove(effectorsToBlend[i]);
            }
        }

        needsToBlend = true;

        effectorsToBlend.Add(newEffector);
    }

    void BlendIK()                                                  // Holy hell this is a lot
    {                                                               // Doesn't -seem- to be too expensive, but may need to revisit
        if (effectorsToBlend.Count == 0)
        {
            needsToBlend = false;
            return;
        }
        else                                                        // Keeps a list of IK effectors waiting to blend, then iterates through them
        {
            for (int i = 0; i < effectorsToBlend.Count; i++)
            {
                if (i >= effectorsToBlend.Count)                    // Safeguard against removing effectors during loop
                {
                    break;
                }
                else
                {
                    if (effectorsToBlend[i].limb != null)           // Checks if there are any effectors needing blending for limbs...
                    {
                        if (effectorsToBlend[i].shouldChangeTarget)
                        {
                            effectorsToBlend[i].limb.target = effectorsToBlend[i].newTarget;
                            effectorsToBlend[i].shouldChangeTarget = false;
                        }
                        effectorsToBlend[i].limb.IKPositionWeight = Mathf.Lerp(effectorsToBlend[i].startWeight,                 // Actual lerps here
                                                                               effectorsToBlend[i].targetWeight,
                                                                               effectorsToBlend[i].currentLerpVal);  

                        effectorsToBlend[i].limb.IKRotationWeight = Mathf.Lerp(effectorsToBlend[i].startWeight,
                                                                               effectorsToBlend[i].targetWeight,
                                                                               effectorsToBlend[i].currentLerpVal);

                        effectorsToBlend[i].currentLerpVal += effectorsToBlend[i].blendSpeed * Time.deltaTime;                  // Increase lerp value

                        if (effectorsToBlend[i].startWeight > effectorsToBlend[i].targetWeight && effectorsToBlend[i].limb.IKPositionWeight < effectorsToBlend[i].targetWeight)
                        {
                            effectorsToBlend[i].limb.IKPositionWeight = effectorsToBlend[i].targetWeight;                       // If effector has gone past target weight,
                            effectorsToBlend[i].limb.IKRotationWeight = effectorsToBlend[i].targetWeight;                       // Set them to target weight.
                        }
                        if (effectorsToBlend[i].limb.IKPositionWeight == effectorsToBlend[i].targetWeight)
                        {
                            if (effectorsToBlend[i].OnIKTargetReached != null)                                                  // Remove blend from list, and fire an event
                            {
                                effectorsToBlend[i].OnIKTargetReached();
                            }
                            if (!removedEffectorThisFrame)
                            {
                                removedEffectorThisFrame = true;
                                effectorsToBlend.Remove(effectorsToBlend[i]);
                            }                            
                            break;
                        }
                    }
                    if (effectorsToBlend[i].aim != null)            // Do the same for weapon aiming effectors...
                    {
                        if (effectorsToBlend[i].shouldChangeTarget)
                        {
                            effectorsToBlend[i].aim.transform = effectorsToBlend[i].newTarget;
                            effectorsToBlend[i].shouldChangeTarget = false;
                        }
                        effectorsToBlend[i].aim.IKPositionWeight = Mathf.Lerp(effectorsToBlend[i].startWeight,
                                                                              effectorsToBlend[i].targetWeight,
                                                                              effectorsToBlend[i].currentLerpVal);

                        effectorsToBlend[i].currentLerpVal += effectorsToBlend[i].blendSpeed * Time.deltaTime;

                        if (effectorsToBlend[i].startWeight > effectorsToBlend[i].targetWeight && effectorsToBlend[i].aim.IKPositionWeight < effectorsToBlend[i].targetWeight)
                        {
                            effectorsToBlend[i].aim.IKPositionWeight = effectorsToBlend[i].targetWeight;
                        }
                        if (effectorsToBlend[i].aim.IKPositionWeight == effectorsToBlend[i].targetWeight)
                        {
                            if (effectorsToBlend[i].OnIKTargetReached != null)
                            {
                                effectorsToBlend[i].OnIKTargetReached();
                            }
                            if (!removedEffectorThisFrame)
                            {
                                removedEffectorThisFrame = true;
                                effectorsToBlend.Remove(effectorsToBlend[i]);
                            }
                            break;
                        }
                    }
                    if (effectorsToBlend[i].look != null)           // And the same for look position effectors.
                    {
                        if (effectorsToBlend[i].shouldChangeTarget)
                        {
                            effectorsToBlend[i].look.target = effectorsToBlend[i].newTarget;
                            effectorsToBlend[i].shouldChangeTarget = false;
                        }
                        effectorsToBlend[i].look.IKPositionWeight = Mathf.Lerp(effectorsToBlend[i].startWeight,
                                                                               effectorsToBlend[i].targetWeight,
                                                                               effectorsToBlend[i].currentLerpVal);

                        effectorsToBlend[i].currentLerpVal += effectorsToBlend[i].blendSpeed * Time.deltaTime;

                        if (effectorsToBlend[i].startWeight > effectorsToBlend[i].targetWeight && effectorsToBlend[i].limb.IKPositionWeight < effectorsToBlend[i].targetWeight)
                        {
                            effectorsToBlend[i].look.IKPositionWeight = effectorsToBlend[i].targetWeight;
                        }
                        if (effectorsToBlend[i].look.IKPositionWeight == effectorsToBlend[i].targetWeight)
                        {
                            if (effectorsToBlend[i].OnIKTargetReached != null)
                            {
                                effectorsToBlend[i].OnIKTargetReached();
                            }
                            if (!removedEffectorThisFrame)
                            {
                                removedEffectorThisFrame = true;
                                effectorsToBlend.Remove(effectorsToBlend[i]);
                            }
                            break;
                        }
                    }
                }                   
            }
            removedEffectorThisFrame = false;
        }
    }

    public void PickUp(GameObject itemToPickup)
    {
        float horizontalAngle = Vector2.SignedAngle(new Vector2(transform.forward.x, transform.forward.z), new Vector2(itemToPickup.transform.position.x - transform.position.x, itemToPickup.transform.position.z - transform.position.z).normalized);
        float verticalDiff = itemToPickup.transform.position.y - transform.position.y;
        float curveHeight = 1;

        if (horizontalAngle < 50 && horizontalAngle > -50)
        {
            defaultPickup.transform.position = transform.position + ((itemToPickup.transform.position - transform.position).normalized);
            defaultPickup.transform.position = new Vector3(defaultPickup.transform.position.x, itemToPickup.transform.position.y, defaultPickup.transform.position.z);
            curveHeight = ((horizontalAngle < 0 ? Mathf.InverseLerp(0, -50, horizontalAngle) : 0) + (verticalDiff < 1 ? Mathf.InverseLerp(1, 0, verticalDiff) : 0)) * 0.5f;         // Adjusting the right hand to make sure it doesn't flip out or go through the body
        }                                                                                                                                                                           // Not perfect, especially when looking up
                                                                                                                                                                                    // But, once items are fully done, no one should be looking up and grabbing things anyway
        keys[1] = new Keyframe(0.37f, curveHeight);

        rightHandPickUpCurve = new AnimationCurve(keys);

        rightHandLocationOnPickup.weightCurves[0].curve = rightHandPickUpCurve;
        interactionSystem.StartInteraction(FullBodyBipedEffector.RightHand, rightHandLocationOnPickup, true);
        interactionSystem.StartInteraction(FullBodyBipedEffector.LeftHand, defaultPickup, true);

        Debug.Log("Pickup from IKHandler");
    }

    public void PickUp(InteractionObject obj)
    {
        PickUp(obj.gameObject);
    }

    class Effector
    {
        public IKSolverAim aim;
        public IKSolverLimb limb;
        public IKSolverLookAt look;
        public Transform newTarget;
        public float startWeight;
        public float targetWeight;
        public float blendSpeed;
        public float currentLerpVal;
        public bool shouldChangeTarget;
        public IKBlendTargetReachedEvent OnIKTargetReached;
    }
}