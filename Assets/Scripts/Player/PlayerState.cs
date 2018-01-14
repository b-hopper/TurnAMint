using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState : MonoBehaviour {
    public enum EMoveState
    {
        WALKING,
        RUNNING,
        CROUCHING,
        SPRINTING
    }

    public enum EWeaponState
    {
        IDLE,
        FIRING,
        AIMING,
        AIMEDFIRING
    }

    public enum ECameraState
    {
        THIRDPERSON,
        FIRSTPERSON
    }

    public EMoveState MoveState;
    public EWeaponState WeaponState;
    public ECameraState CameraState;

    InputController m_inputController;
    public InputController InputController
    {
        get
        {
            if (m_inputController == null)
            {
                m_inputController = GameManager.Instance.InputController;
            }
            return m_inputController;
        }
    }

    private void Update()
    {
        SetMoveState();
        SetWeaponState();
    }

    void SetMoveState()
    {
        MoveState = EMoveState.RUNNING;

        if (InputController.IsSprinting)
        {
            MoveState = EMoveState.SPRINTING;
        }
        if (InputController.IsWalking)
        {
            MoveState = EMoveState.WALKING;
        }
        if (InputController.IsCrouched)
        {
            MoveState = EMoveState.CROUCHING;
        }
    }

    void SetWeaponState()
    {
        WeaponState = EWeaponState.IDLE;

        if (InputController.Fire1)
        {
            WeaponState = EWeaponState.FIRING;
        }
        if (InputController.Fire2)
        {
            WeaponState = EWeaponState.AIMING;
        }
        if (InputController.Fire1 && InputController.Fire2)
        {
            WeaponState = EWeaponState.AIMEDFIRING;
        }
    }

    public void SetCameraState(bool isFirstPerson)
    {
        if (isFirstPerson)
        {
            CameraState = ECameraState.FIRSTPERSON;
        }
        else
        {
            CameraState = ECameraState.THIRDPERSON;
        }
    }
}
