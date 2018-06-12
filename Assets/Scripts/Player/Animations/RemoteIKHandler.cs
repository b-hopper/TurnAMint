using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.FinalIK;

namespace TurnAMint.Player.Animation
{
    public class RemoteIKHandler : MonoBehaviour, I_IKHandler
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

                return m_IKValues;
            }
            set
            {
                bipedIK.solvers.lookAt.target = value.lookIK.target;
                bipedIK.solvers.lookAt.IKPositionWeight = value.lookIK.weight;
                m_IKValues.lookIK.target = value.lookIK.target;
                m_IKValues.lookIK.weight = value.lookIK.weight;

                bipedIK.solvers.aim.target = value.aimIK.target;
                bipedIK.solvers.aim.IKPositionWeight = value.aimIK.weight;
                m_IKValues.aimIK.target = value.aimIK.target;
                m_IKValues.aimIK.weight = value.aimIK.weight;

                bipedIK.solvers.leftHand.target = value.leftHandIK.target;
                bipedIK.solvers.leftHand.IKPositionWeight = value.leftHandIK.positionWeight;
                bipedIK.solvers.leftHand.IKRotationWeight = value.leftHandIK.rotationWeight;
                m_IKValues.leftHandIK.target = value.leftHandIK.target;
                m_IKValues.leftHandIK.positionWeight = value.leftHandIK.positionWeight;
                m_IKValues.leftHandIK.rotationWeight = value.leftHandIK.rotationWeight;

                bipedIK.solvers.rightHand.target = value.rightHandIK.target;
                bipedIK.solvers.rightHand.IKPositionWeight = value.rightHandIK.positionWeight;
                bipedIK.solvers.rightHand.IKRotationWeight = value.rightHandIK.rotationWeight;
                m_IKValues.rightHandIK.target = value.rightHandIK.target;
                m_IKValues.rightHandIK.positionWeight = value.rightHandIK.positionWeight;
                m_IKValues.rightHandIK.rotationWeight = value.rightHandIK.rotationWeight;
            }
        }

        BipedIK bipedIK;
        InteractionSystem interactionSystem;

        public InteractionObject defaultPickup;

        private void Awake()
        {
            m_IKValues = new IKValues();
            bipedIK = GetComponent<BipedIK>();
            interactionSystem = GetComponent<InteractionSystem>();
        }

        public void PickUp(InteractionObject itemToPickup)
        {
            interactionSystem.StartInteraction(FullBodyBipedEffector.LeftHand, defaultPickup, true);
        }
    }

}