using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerState))]
public class Player : MonoBehaviour {
    [System.Serializable]
    public class MouseInput
    {
        public Vector2 Damping;
        public Vector2 Sensitivity;
        public bool LockMouse;
    }

    [SerializeField] float runSpeed;
    [SerializeField] float walkSpeedMultiplier;
    [SerializeField] float crouchSpeedMultiplier;
    [SerializeField] float sprintSpeedMultiplier;
    [SerializeField] MouseInput mouseControl;
    [SerializeField] AudioController footsteps;
    [SerializeField] float minimumFootstepDistance;

    public PlayerAim playerAim;

    [SerializeField, Tooltip("Object to mask out when switching to first-person mode.")] public GameObject playerHead;

    Vector3 previousPosition;

    Vector2 direction;
    
    PlayerState m_playerState;
    public PlayerState PlayerState
    {
        get
        {
            if (m_playerState == null)
            {
                m_playerState = GetComponentInChildren<PlayerState>();
            }
            return m_playerState;
        }
    }

    private CharacterController m_MoveController;
    public CharacterController MoveController
    {
        get
        {
            if (m_MoveController == null)
            {
                m_MoveController = GetComponent<CharacterController>();
            }
            return m_MoveController;
        }
    }

    private PlayerShoot m_PlayerShoot;
    public PlayerShoot PlayerShoot
    {
        get
        {
            if (m_PlayerShoot == null)
                m_PlayerShoot = GetComponent<PlayerShoot>();
            return m_PlayerShoot;
        }
    }

    InputController playerInput;
    Vector2 mouseInput;

    private void Awake()
    {
        playerInput = GameManager.Instance.InputController;
        GameManager.Instance.LocalPlayer = this;

        GameManager.Instance.InputController.OnCameraViewChanged += ChangeCameraPOV;

        if (mouseControl.LockMouse)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    private void Update()
    {
        Move();

        LookControl();
    }

    private void LookControl()
    {
        mouseInput.x = Mathf.Lerp(mouseInput.x, playerInput.MouseInput.x, 1f / mouseControl.Damping.x);
        mouseInput.y = Mathf.Lerp(mouseInput.y, playerInput.MouseInput.y, 1f / mouseControl.Damping.y);

        transform.Rotate(Vector3.up * mouseInput.x * mouseControl.Sensitivity.x);
        
        playerAim.SetRotation(mouseInput.y * mouseControl.Sensitivity.y);
    }

    void Move()
    {
        float moveSpeed = runSpeed;

        if (playerInput.IsCrouched)
        {
            moveSpeed *= crouchSpeedMultiplier;
        }
        else if (playerInput.IsSprinting)
        {
            moveSpeed *= sprintSpeedMultiplier;
        }
        else if (playerInput.IsWalking)
        {
            moveSpeed *= walkSpeedMultiplier;
        }

        direction = new Vector2(playerInput.Vertical * moveSpeed, playerInput.Horizontal * moveSpeed);
        MoveController.Move(transform.forward * direction.x * 0.02f + transform.right * direction.y * 0.02f);

        if (Vector3.Distance(transform.position, previousPosition) > minimumFootstepDistance)
        {
            footsteps.Play();
        }

        previousPosition = transform.position;
    }

    void ChangeCameraPOV()
    {
        if (PlayerState.CameraState == PlayerState.ECameraState.THIRDPERSON)
        {
            GameManager.Instance.MainCameraController.SetFirstPersonCameraParent(playerHead.transform);
            PlayerState.SetCameraState(true);
        }
        else
        {
            GameManager.Instance.MainCameraController.SetThirdPersonCameraParent();
            PlayerState.SetCameraState(false);
        }
    }
}
