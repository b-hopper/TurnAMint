using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TurnAMint.Management;
using TurnAMint.Networking;
using TurnAMint.UI;
using TurnAMint.Player.Animation;
using TurnAMint.Player.Inventory;
using TurnAMint.Items;
using TurnAMint.Player.Health;


namespace TurnAMint.Player
{
    public class StateManager : NetworkBehaviour
    {

        #region Declaring Variables
        uint playerFlags;

        public float blendSpeedTEMP;
        public string PlayerName;
        public bool aiming;
        public bool canRun;
        public bool canMove = true;
        public bool walk;
        public bool sprint;
        public bool shoot;
        public bool actualShooting;
        public bool reloading;
        public bool onGround;
        public bool crouching;
        public bool prone;
        public bool canJump = true;
        public bool isJumping;
        public bool isFalling;
        public bool distanceFromGround;
        public float fallInjuryThreshold;
        public float fallDamageMultiplier;      // Replace with velocity? Opens up to weird bugs, though
        public float legsAngle;
        public bool freelook;
        public bool isHolstering;
        public bool isPickingUp;
        bool canStopJumping, initialHeightSet, wasSprinting, isBlendingLook;

        public MenuState menuState;

        [HideInInspector] public Transform leftHandIKTarget, rightHandIKTarget;

        [HideInInspector] public HealthManager playerHealth;

        [HideInInspector] public Networking.ClientBehavior cBehavior;

        public Weapon currentWeapon;
        [SyncVar] public int currentWeaponType;

        public Transform inHandWeaponLocation;

        Animator[] animators;       // For ragdoll enable/disable    
        Rigidbody[] bodyParts;
        Rigidbody mainRB;

        public ItemBase itemToPickUpBase;

        public float horizontal;
        public float vertical;
        public Vector3 lookPosition;
        public Vector3 lookHitPosition;
        Quaternion camPivotTargetRot;
        public LayerMask layerMask;

        [HideInInspector]
        public HandleAnimations handleAnim;
        PlayerMovement handleMovement;
        InputHandler input;
        IKHandler ikHandler;
        [HideInInspector] public HandleInventory inventoryHandler;

        public CharacterAudioManager audioManager;

        public delegate void StateChangeEvent();
        public delegate void float_StateChangeEvent(float val);
        public delegate void bool_StateChangeEvent(bool val);

        public StateChangeEvent OnJumpLanded;
        public StateChangeEvent OnJumpStarted;
        public StateChangeEvent OnWeaponSwitch;
        public StateChangeEvent OnFireButtonPressed;
        public StateChangeEvent OnFireButtonUnpressed;
        public float_StateChangeEvent OnHardFallLanded;
        public bool_StateChangeEvent OnSprintChanged;

        //public StateChangeEvent TEMP_OnItemAbleToPickUp;

        public Transform freeLookRotation, freeLookCamTarget, camLookTarget, tpsCamPivot, freeLookCamPivot;

        [HideInInspector] public Transform camPivotBase { get { return input.camRotation; } }


        float fallDistance;

        public float jumpStrength, jumpTimer;

        Management.PlayerRespawn respawn;

        #endregion

        #region Awake, Start, Update

        private void Awake()
        {
            input = GetComponent<InputHandler>();
            ikHandler = GetComponent<IKHandler>();
            playerHealth = GetComponent<HealthManager>();
            mainRB = GetComponent<Rigidbody>();
            animators = GetComponentsInChildren<Animator>();
            bodyParts = GetComponentsInChildren<Rigidbody>();
            handleMovement = GetComponent<PlayerMovement>();
            respawn = GetComponent<PlayerRespawn>();
            audioManager = GetComponent<CharacterAudioManager>();
            handleAnim = GetComponent<HandleAnimations>();
            inventoryHandler = GetComponent<HandleInventory>();
            cBehavior = GetComponent<ClientBehavior>();
        }

        private void Start()
        {
            input.OnJumpPressed += HandleJump;
            input.OnWeaponChangeWithDirection += EquipWeaponWithDirection;
            input.OnWeaponSelected += EquipNewWeapon;
            input.OnInteractPressed += HandlePickup;
            input.OnReloadButtonPressed += HandleReload;
            input.OnFireModeSwitchButtonPressed += HandleFireModeSwitch;
            playerHealth.OnKilled += PlayerDeath;
            playerHealth.OnKnockedDown += PlayerKnockedDown;
            OnHardFallLanded += HandleHardFall;
        }

        private void FixedUpdate()
        {
            onGround = IsOnGround();

            if (!isLocalPlayer) { LerpLookTarget(); return; }

            if (wasSprinting != sprint)             // Check to see if sprint status changed, and if it did, fire an event
            {                                       // Listeners: HandleAnimations
                if (OnSprintChanged != null)
                {
                    OnSprintChanged(sprint);
                }
            }
            wasSprinting = sprint;
        }

        internal void SetCurrentWeapon(Weapon wpn)
        {
            currentWeapon = wpn;
        }

        private void LerpLookTarget()
        {
            camPivotBase.localRotation = Quaternion.Lerp(camPivotBase.localRotation, camPivotTargetRot, Time.deltaTime * blendSpeedTEMP);
        }

        #endregion

        #region Interaction With Environment/Items
        private void HandlePickup()
        {
            if (itemToPickUpBase != null)
            {
                isPickingUp = true;
                cBehavior.CmdRemotePickup(itemToPickUpBase.gameObject);
                ikHandler.PickUp(itemToPickUpBase.gameObject);
            }
        }

        public void PrepareInteract(bool enableContextText, ItemBase item = null)
        {
            if (item != null && inventoryHandler.GetRemainingInventoryWeight() > item.totalWeight)
            {
                itemToPickUpBase = item;
                itemToPickUpBase.state = this;
                GameManager.Instance.UIManager.SetContextText("Press [F] to pick up " + item.itemName);
            }
            else if (item != null)
            {
                GameManager.Instance.UIManager.SetContextText("Inventory Full!", false);
            }
            else
            {
                if (itemToPickUpBase != null && !isPickingUp)
                {
                    itemToPickUpBase.state = null;
                }
                itemToPickUpBase = null;
            }

            GameManager.Instance.UIManager.EnableContextText(enableContextText);
        }

        internal void PickUp()
        {
            if (itemToPickUpBase != null)
            {
                inventoryHandler.PickUp(itemToPickUpBase);
            }
            isPickingUp = false;
        }
        #endregion

        #region Jumping and Falling
        bool IsOnGround()
        {
            bool retVal = false;

            Vector3 origin = transform.position + new Vector3(0, 0.05f, 0);
            RaycastHit hit;

            if (Physics.Raycast(origin, -Vector3.up, out hit, 0.06f, layerMask))
            {
                retVal = true;
                initialHeightSet = false;
            }

            if (retVal && canStopJumping)
            {
                isJumping = false;
                canStopJumping = false;
                if (OnJumpLanded != null)
                {
                    OnJumpLanded();
                }
            }

            if (!isFalling && !retVal)
            {
                if (!initialHeightSet)
                {
                    fallDistance = transform.position.y;
                    initialHeightSet = true;
                }
                if (fallDistance - transform.position.y > fallInjuryThreshold)
                {
                    isFalling = true;
                    canMove = false;
                    if (handleAnim != null)
                    {
                        handleAnim.StartFall();
                    }
                }
                canJump = false;
            }
            else if (isFalling && retVal)
            {
                isFalling = false;
                float tmpDist = fallDistance - transform.position.y;
                if (tmpDist > fallInjuryThreshold && OnHardFallLanded != null)
                {
                    OnHardFallLanded(tmpDist);
                }
            }
            return retVal;
        }

        private void HandleJump()
        {
            if (!isJumping && !crouching && !prone && canJump)
            {
                canJump = false;
                isJumping = true;

                GameManager.Instance.Timer.Add(() =>
                {
                    canStopJumping = true;
                }, 0.1f);

                if (OnJumpStarted != null)
                {
                    OnJumpStarted();
                }

                handleMovement.HandleJump(jumpStrength);
            }
        }
        #endregion

        #region Weapon Management
        public void FireButtonPressed()
        {
            if (OnFireButtonPressed != null)        // Listeners: None
            {
                OnFireButtonPressed();
            }
            if (currentWeapon != null)
            {
                currentWeapon.Fire();
            }
        }

        public void FireButtonUnpressed()
        {
            if (OnFireButtonUnpressed != null)          // Listeners: None
            {
                OnFireButtonUnpressed();
            }
            if (currentWeapon != null)
            {
                currentWeapon.StopFiring();
            }
        }

        void HandleFireModeSwitch()
        {
            if (currentWeapon != null)
            {
                currentWeapon.ChangeFireMode();
            }
        }

        void HandleReload()
        {
            if (currentWeapon != null && !isHolstering)
            {
                if (currentWeapon.Reload())
                {
                    cBehavior.RequestRemoteReload();
                }
            }
        }

        void EquipWeaponWithDirection(int direction)
        {
            if (!reloading && !isHolstering)
            {
                inventoryHandler.HandleWeaponSwitchWithDirection(direction);
            }
        }

        void EquipNewWeapon(int index)
        {
            if (!reloading && !isHolstering)
            {
                inventoryHandler.HandleWeaponSwitchWithIndex(index);
            }
        }

        public void ForceEquipWeapon(Weapon wpn)
        {
            SetUpNewWeapon(wpn);
            currentWeaponType = 1;
        }

        void SetUpNewWeapon(Weapon wpn)
        {
            isHolstering = false;
            currentWeapon = wpn;
            currentWeapon.EquipWeapon(this);
            currentWeapon.currentHolster = null;
            currentWeapon.transform.parent = inHandWeaponLocation;
            currentWeapon.transform.localPosition = currentWeapon.alignedWithHandLocalPos;
            currentWeapon.transform.localEulerAngles = currentWeapon.alignedWithHandLocalRot;
            inventoryHandler.equippedWeaponClass = wpn.weaponClass;
            if (isLocalPlayer) UIManager.instance.EnableAmmoCountText(true);
        }
        #endregion

        #region Health Management
        private void HandleHardFall(float val)          // Linear damage for now, may revisit later
        {
            float damage = (val - fallInjuryThreshold) * fallDamageMultiplier;
            playerHealth.TakeEnvironmentalDamage((int)damage);
        }

        public void PlayerDeath()
        {
            EnableRagdoll(true);
            respawn.RespawnAtSpawnPoint(this, 5f);
            ikHandler.EnableAllIK(false);
            cBehavior.CmdPlayerDeath();

            input.enabled = false;
        }

        public void PlayerRespawn()
        {
            EnableRagdoll(false);
            gameObject.SetActive(true);
            ikHandler.EnableAllIK(true);
            cBehavior.CmdPlayerRespawn();

            input.enabled = true;

        }

        void EnableRagdoll(bool val)
        {
            if (animators != null)
            {
                for (int i = 0; i < animators.Length; i++)
                {
                    if (animators[i] != null)
                    {
                        animators[i].enabled = !val;
                    }
                }
            }

            if (bodyParts != null)
            {
                for (int i = 0; i < bodyParts.Length; i++)
                {
                    bodyParts[i].isKinematic = !val;
                }
                mainRB.isKinematic = val;
            }

        }

        private void PlayerKnockedDown()
        {
            print("Knocked Down Not Implemented");
        }

        #endregion

        #region Remote Functions

        public void RemoteFire()        // This is called when a networked player fires. Bullet trajectories are client-side, so 
        {                               // only shots and hits are sent over the network.
            if (currentWeapon != null)
            {
                currentWeapon.RemotePlayerFire();
            }
        }

        public void RemotePickup(GameObject obj)    // Called when a networked player picks up an item
        {
            itemToPickUpBase = obj.GetComponent<ItemBase>();
            ikHandler.PickUp(obj);
        }

        public void RemoteDeath()
        {
            EnableRagdoll(true);
            ikHandler.EnableAllIK(false);
        }

        public void RemoteRespawn()
        {
            EnableRagdoll(false);
            ikHandler.EnableAllIK(true);
        }

        public void RemoteReload()
        {
            handleAnim.StartReload();
        }

        internal void SmoothMoveLookTarget(Quaternion rot)
        {
            camPivotTargetRot = rot;
        }

        internal void RemoteDrop(GameObject obj, Vector3 pos)
        {
            Transform t;
            t = inventoryHandler.DropItem(obj.GetComponent<ItemBase>());
            t.position = pos;
        }

        #endregion

        public bool CheckFlag(PlayerFlags flag) { return (playerFlags & (uint)flag) != 0; }

        public void SetFlag(PlayerFlags flag) { playerFlags = playerFlags | (uint)flag; }

        public void ClearFlag(PlayerFlags flag) { playerFlags = playerFlags & ~(uint)flag; }
    }

    public enum MenuState
    {
        NONE,
        SYSTEM,
        INVENTORY
    }

    public enum PlayerFlags
    {
        ALIVE = 1 << 0,
        ACCEPTING_MOUSE_INPUT = 1 << 1,
        ACCEPTING_KEYBOARD_INPUT = 1 << 2
    }
}