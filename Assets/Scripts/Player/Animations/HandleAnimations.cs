using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using TurnAMint.Items;

namespace TurnAMint.Player.Animation
{
    public class HandleAnimations : MonoBehaviour
    {
        Animator anim;
        IKHandler ikHandler;
        NetworkAnimator netAnim;

        StateManager state;
        Vector3 lookDirection;

        Weapon weaponToEquip;

        public delegate void AnimationEventHandler();

        public AnimationEventHandler OnReloadFinished;

        const float degreeToMagnitudeConversion = 0.0055555f;

        private void Awake()
        {
            state = GetComponent<StateManager>();
            anim = GetComponent<Animator>();
            ikHandler = GetComponent<IKHandler>();
            netAnim = GetComponent<NetworkAnimator>();
        }
        private void Start()
        {
            state.OnJumpLanded += EndJumpAnim;
            state.OnJumpStarted += DoJumpAnim;
            state.playerHealth.OnHealthRemoved += DoDamagedAnim;
            state.OnHardFallLanded += HandleHardLanding;
            state.OnSprintChanged += HandleSprintChange;

            //ChangeWeapon((states.currentWeapon != null) ? (int)states.currentWeapon.stats.weaponType : 0, (states.currentWeapon != null), false);
        }

        private void SetupAnimator()
        {

            Animator[] anims = GetComponentsInChildren<Animator>();

            for (int i = 0; i < anims.Length; i++)
            {
                if (anims[i] != anim)
                {
                    anim.avatar = anims[i].avatar;
                    break;
                }
            }

        }

        private void FixedUpdate()
        {
            state.reloading = anim.GetBool("Reloading");
            anim.SetBool("Aim", state.aiming);

            anim.SetBool("Falling", state.isFalling);

            anim.SetFloat("Forward", GetVerticalSpeed(), 0.1f, Time.deltaTime);
            anim.SetFloat("Sideways", state.horizontal * (state.walk || state.aiming || state.reloading ? 0.5f : 1), 0.1f, Time.deltaTime);

            anim.SetBool("Crouching", state.crouching);

            anim.SetFloat("TurnAngle", (state.legsAngle * degreeToMagnitudeConversion) + 0.5f);
        }

        private float GetVerticalSpeed()
        {
            float tmp = state.vertical;

            if (state.walk || state.aiming)
            {
                tmp *= 0.5f;
            }
            else if (state.sprint)
            {
                tmp *= 2;
            }

            return tmp;
        }

        public void OnWeaponHolstered()
        {
            state.inventoryHandler.HolsterWeapon(state.currentWeapon);
        }

        public void OnWeaponEquipped()
        {
            if (weaponToEquip != null)
            {
                state.inventoryHandler.UnholsterWeapon(weaponToEquip);
                state.ForceEquipWeapon(weaponToEquip);
                weaponToEquip = null;

                ikHandler.BlendAimIK(state.currentWeapon.transform, 1, 1f);
                ikHandler.BlendLeftHandIK(state.currentWeapon.leftHandIKTarget, state.currentWeapon.leftHandWeight, 1f);
                ikHandler.BlendRightHandIK(state.currentWeapon.rightHandIKTarget, state.currentWeapon.rightHandWeight, 1f);
            }
        }

        private void HandleSprintChange(bool val)
        {
            if (state.currentWeapon != null)
            {
                if (val) ikHandler.BlendAimIK(null, 0, 1f);
                else ikHandler.BlendAimIK(state.currentWeapon.transform, 1, 1f);
            }
        }

        internal bool isReloading()
        {
            return anim.GetBool("Reloading");
        }

        public void FinishReload()
        {
            if (OnReloadFinished != null)
            {
                OnReloadFinished();
            }

            ikHandler.BlendLeftHandIK(state.currentWeapon.leftHandIKTarget, 1, 1);
        }

        public void StartReload()
        {
            if (!state.reloading && state.currentWeapon != null)
            {
                anim.SetTrigger("Reload_Trigger");

                netAnim.SetTrigger("Reload_Trigger");

                ikHandler.BlendLeftHandIK(null, 0, 3f);
            }
        }

        public void ChangeWeapon(Weapon wpn, bool currentlyEquipped, bool shouldTrigger = true)
        {
            weaponToEquip = wpn;

            UI.UIManager.instance.EnableAmmoCountText(false);

            anim.SetInteger("WeaponType", wpn != null ? 1 : 0);
            if (shouldTrigger)
            {
                anim.SetBool("CurrentlyEquipped", currentlyEquipped);
                anim.SetTrigger("ChangeWeapon_Trigger");

                netAnim.SetTrigger("ChangeWeapon_Trigger");
            }

            ikHandler.BlendAimIK(null, 0, 3f, true);
            ikHandler.BlendLeftHandIK(null, 0, 3f);
            ikHandler.BlendRightHandIK(null, 0, 3f, true);
        }

        public void ForceChangeWeapon(Weapon wpn)
        {
            ikHandler.BlendAimIK(wpn.transform, 1, 2f);
            ikHandler.BlendLeftHandIK(null, 0, 5f, true, () =>
            {
                if (state.currentWeapon.leftHandWeight != 0)
                { ikHandler.BlendLeftHandIK(state.currentWeapon.leftHandIKTarget, state.currentWeapon.leftHandWeight, 3f, true); }
            });
            ikHandler.BlendRightHandIK(null, 0, 5f, true, () =>
            {
                ikHandler.BlendRightHandIK(state.currentWeapon.rightHandIKTarget, state.currentWeapon.rightHandWeight, 3f, true, () =>
                {
                    anim.SetInteger("WeaponType", state.currentWeaponType);
                });
            });
        }

        public void ForceUnequipWeapon()
        {
            ikHandler.BlendAimIK(null, 0, 2f);
            ikHandler.BlendLeftHandIK(null, 0, 2f);
            ikHandler.BlendRightHandIK(null, 0, 2f);
            state.currentWeaponType = 0;
            anim.SetInteger("WeaponType", state.currentWeaponType);
        }

        private void DoJumpAnim()
        {
            anim.SetTrigger("Jump_Trigger");
            netAnim.SetTrigger("Jump_Trigger");

            anim.SetBool("Jumping", true);
        }

        private void EndJumpAnim()
        {
            anim.SetBool("Jumping", false);
        }

        private void DoDamagedAnim(int amount)
        {
            anim.SetTrigger("PlayerDamaged_Trigger");
            netAnim.SetTrigger("PlayerDamaged_Trigger");
        }

        public void TurnLegs(float angle)
        {
            anim.SetTrigger("TurnLegs_Trigger");
            netAnim.SetTrigger("TurnLegs_Trigger");
        }

        private void HandleHardLanding(float val)
        {
            anim.SetTrigger("HardLanding_Trigger");
            netAnim.SetTrigger("HardLanding_Trigger");
        }

        private void FinishHardLanding()
        {
            ikHandler.EnableAllIK(true);
            if (state.currentWeapon != null)
            {
                ikHandler.BlendAimIK(null, 1, 1, false);
            }
            state.canMove = true;
        }

        public void StartFall()
        {
            ikHandler.EnableAllIK(false);
            anim.SetTrigger("Fall_Trigger");
            netAnim.SetTrigger("Fall_Trigger");
            if (state.currentWeapon != null)
            {
                ikHandler.BlendAimIK(null, 0, 5f, false);
            }
        }
    }
}