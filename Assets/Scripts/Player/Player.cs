using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MoveController))]

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

    Vector2 direction;

    Weapons.Crosshair m_Crosshair;

    Weapons.Crosshair Crosshair
    {
        get
        {
            if (m_Crosshair == null)
            {
                m_Crosshair = GetComponentInChildren<Weapons.Crosshair>();
            }
            return m_Crosshair;
        }
    }

    private MoveController m_MoveController;
    public MoveController MoveController
    {
        get
        {
            if (m_MoveController == null)
            {
                m_MoveController = GetComponent<MoveController>();
            }
            return m_MoveController;
        }
    }

    InputController playerInput;
    Vector2 mouseInput;

    private void Awake()
    {
        playerInput = GameManager.Instance.InputController;
        GameManager.Instance.LocalPlayer = this;

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

        Crosshair.LookHeight(mouseInput.y * mouseControl.Sensitivity.y);
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
        MoveController.Move(direction);
    }
}
