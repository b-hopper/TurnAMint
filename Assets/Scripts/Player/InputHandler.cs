using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TurnAMint.Items;

namespace TurnAMint.Player
{
    public class InputHandler : NetworkBehaviour
    {

        #region Declaring Variables
        public float horizontal;
        public float vertical;
        public float mouse2;
        public float walk;
        public float scrollWheel;
        public float mouseX;
        public float mouseY;
        public bool crouching;
        public bool freelook;
        public bool sprint;

        public delegate void InputEvent();
        public InputEvent OnJumpPressed;
        public InputEvent OnViewModeChanged;
        public InputEvent OnInteractPressed;
        public InputEvent OnFireButtonPressed;
        public InputEvent OnFireButtonUnPressed;
        public InputEvent OnReloadButtonPressed;
        public InputEvent OnFireModeSwitchButtonPressed;

        public delegate void InputIntegerEvent(int val);
        public InputIntegerEvent OnWeaponSelected;
        public InputIntegerEvent OnWeaponChangeWithDirection;

        public Transform cameraActualTrans;
        public Transform camRotation;

        Transform lookingAtObj;
        ItemBase lookingAtItem;
        bool isCurrentlyInRangeOfItem, wasInRangeLastUpdate;

        UI.UIManager crosshairManager;
        CameraManager camMan;
        StateManager state;

        public float minimumRaycastDistance, maximumPlayerInteractDistance;

        public LayerMask rayLayerMask, interactLayerMask;

        public float shakeRecoil = 0.5f;
        public float shakeMovement = 0.3f;
        public float shakeMin = 0.1f;
        float targetShake;
        float curShake;

        public bool fpsMode;
        bool canSwitch;

        #endregion

        #region Awake, Start, Update
        private void Awake()
        {
            state = GetComponent<StateManager>();
            camMan = GetComponent<CameraManager>();
        }

        private void Start()
        {
            rayLayerMask = ~(1 << gameObject.layer) & rayLayerMask;
            interactLayerMask = (1 << LayerMask.NameToLayer("Pickups"));
            state.layerMask = rayLayerMask;

            crosshairManager = UI.UIManager.GetInstance();

            if (isLocalPlayer)
                camMan.SetupCameras();
        }

        private void Update()
        {
            HandleInput();
            UpdateStates();
        }

        private void FixedUpdate()
        {
            if (!state.freelook)
            {
                // Find where the camera is looking
                Ray ray = new Ray(cameraActualTrans.position, cameraActualTrans.forward);

                DetectLookPosition(ray);
                DetectInteractions(ray);
            }
        }

        private void UpdateStates()
        {
            state.aiming = state.onGround && (mouse2 > 0) && !freelook;
            state.canRun = !state.aiming;
            state.walk = (walk > 0);

            state.crouching = (state.onGround && crouching);

            state.horizontal = horizontal;
            state.vertical = vertical;

            state.freelook = freelook;

            state.sprint = sprint;
        }

        #endregion

        private void DetectLookPosition(Ray ray)
        {
            state.lookPosition = ray.GetPoint(20);

            RaycastHit hit;

            Vector3 lookHitPosition;

            if (Physics.Raycast(ray.origin + (cameraActualTrans.forward * minimumRaycastDistance), ray.direction, out hit, 500, rayLayerMask))
            {
                lookHitPosition = hit.point;
            }
            else
            {
                lookHitPosition = state.lookPosition;
            }
            state.lookHitPosition = lookHitPosition;
        }

        private void DetectInteractions(Ray ray)
        {
            RaycastHit hit;

            isCurrentlyInRangeOfItem = false;

            if (Physics.Raycast(ray.origin + (cameraActualTrans.forward), ray.direction, out hit, 10, interactLayerMask))      // Detect if player is looking at something interactable
            {
                if (hit.transform != lookingAtObj)                                  // Detect if the item being looked at has changed
                {
                    wasInRangeLastUpdate = false;                                   // Needed for multiple objects in close range
                    lookingAtItem = hit.transform.GetComponent<ItemBase>();         // If we're looking at something new, get the ItemBase component
                }
            }
            else
            {
                lookingAtItem = null;
            }

            lookingAtObj = hit.transform;

            if (lookingAtItem != null)
            {
                isCurrentlyInRangeOfItem = (Vector3.Distance(transform.position, lookingAtItem.transform.position) <= maximumPlayerInteractDistance);
            }
            else
            {
                isCurrentlyInRangeOfItem = false;
            }
            // Now we know if we have an item, and if it's in range
            if (wasInRangeLastUpdate != isCurrentlyInRangeOfItem)                   // If the item wasn't in range, but now is, or vice versa, set the state accordingly
            {
                state.PrepareInteract(isCurrentlyInRangeOfItem, lookingAtItem);    // Prepare the interaction. 
            }

            wasInRangeLastUpdate = isCurrentlyInRangeOfItem;
        }

        private void OnDrawGizmos()
        {
            if (state != null)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(transform.position + (Vector3.up * 1.5f), state.lookHitPosition);
                Gizmos.color = new Vector4(1, 0.5f, 0, 1);
                Gizmos.DrawWireCube(state.lookHitPosition, Vector3.one * 0.15f);
            }
            if (lookingAtObj != null)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawLine(transform.position + (Vector3.up * 1.5f), lookingAtObj.position);
            }
        }

        private void HandleInput()
        {
            horizontal = Input.GetAxis("Horizontal");
            vertical = Input.GetAxis("Vertical");
            mouse2 = Input.GetAxis("Fire2");
            scrollWheel = Input.GetAxis("Mouse ScrollWheel");
            walk = Input.GetAxis("Walk");
            crouching = Input.GetAxis("Crouch") > 0;
            freelook = Input.GetAxis("Freelook") > 0;
            sprint = Input.GetAxis("Sprint") > 0;

            #region Weapons
            if (state.menuState == MenuState.NONE)
            {
                if (Input.GetButtonDown("Fire1"))
                {
                    state.FireButtonPressed();
                }

                if (Input.GetButtonUp("Fire1"))
                {
                    state.FireButtonUnpressed();
                }

                mouseX = Input.GetAxis("Mouse X");
                mouseY = Input.GetAxis("Mouse Y");
            }
            else
            {
                mouseX = 0;
                mouseY = 0;
            }

            if (Input.GetButtonDown("Reload"))
            {
                if (OnReloadButtonPressed != null)
                {
                    OnReloadButtonPressed();
                }
            }

            if (Input.GetButtonDown("FireModeSwitch"))
            {
                if (OnFireModeSwitchButtonPressed != null)
                {
                    OnFireModeSwitchButtonPressed();
                }
            }
            #endregion

            #region Inventory

            if (Input.GetButtonDown("InvSwitch1"))
            {
                if (OnWeaponSelected != null)
                {
                    OnWeaponSelected(0);
                }
            }
            if (Input.GetButtonDown("InvSwitch2"))
            {
                if (OnWeaponSelected != null)
                {
                    OnWeaponSelected(1);
                }
            }
            if (Input.GetButtonDown("InvSwitch3"))
            {
                if (OnWeaponSelected != null)
                {
                    OnWeaponSelected(2);
                }
            }

            if (scrollWheel > 0)
            {
                if (OnWeaponChangeWithDirection != null)
                {
                    OnWeaponChangeWithDirection(1);
                }
            }
            if (scrollWheel < 0)
            {
                if (OnWeaponChangeWithDirection != null)
                {
                    OnWeaponChangeWithDirection(-1);
                }
            }

            if (Input.GetKeyDown(KeyCode.Tab))
            {
                bool tmp = Management.GameManager.Instance.UIManager.invUI.ToggleInventory();
                state.menuState = tmp ? MenuState.INVENTORY : MenuState.NONE;

                Cursor.lockState = tmp ? CursorLockMode.None : CursorLockMode.Locked;
            }
            #endregion

            if (Input.GetButtonDown("Interact"))
            {
                if (OnInteractPressed != null)
                {
                    OnInteractPressed();
                }
            }

            if (Input.GetButtonDown("Viewmode"))
            {
                if (OnViewModeChanged != null)
                {
                    OnViewModeChanged();
                }
            }

            if (Input.GetAxis("Jump") > 0)
            {
                if (OnJumpPressed != null)
                {
                    OnJumpPressed();
                }
            }
            else
            {
                state.canJump = true;
            }
        }
    }
}