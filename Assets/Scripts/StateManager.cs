using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class StateManager : NetworkBehaviour {


    public bool aiming;
    public bool canRun;
    public bool walk;
    public bool shoot;
    public bool actualShooting;
    public bool reloading;
    public bool onGround;
    public bool crouching;
    public bool prone;
    public bool canJump = true;
    public bool isJumping;
    bool canStopJumping;

    [HideInInspector] public HealthManager playerHealth;

    Animator[] animators;
    Animator mainAnimator;
    NetworkAnimator networkAnimator;
    Rigidbody[] bodyParts;
    Rigidbody mainRB;


    public float horizontal;
    public float vertical;
    [SyncVar]
    public Vector3 lookPosition;
    public Vector3 lookHitPosition;
    public LayerMask layerMask;

    InputHandler input;

    public CharacterAudioManager audioManager;

    public delegate void JumpEvent();

    public JumpEvent OnJumpLanded;
    public JumpEvent OnJumpStarted;

    [HideInInspector]
    public HandleShooting handleShooting;
    [HideInInspector]
    public HandleAnimations handleAnim;
    PlayerMovement handleMovement;


    public float jumpStrength, jumpTimer;

    PlayerRespawn respawn;

    private void Awake()
    {
        playerHealth = GetComponent<HealthManager>();
        networkAnimator = GetComponent<NetworkAnimator>();
        mainAnimator = GetComponent<Animator>();
        mainRB = GetComponent<Rigidbody>();
        animators = GetComponentsInChildren<Animator>();
        bodyParts = GetComponentsInChildren<Rigidbody>();
        respawn = GetComponent<PlayerRespawn>();
        input = GetComponent<InputHandler>();
        handleMovement = GetComponent<PlayerMovement>();
    }

    private void Start()
    {
        audioManager = GetComponent<CharacterAudioManager>();
        handleShooting = GetComponent<HandleShooting>();
        handleAnim = GetComponent<HandleAnimations>();
        input.OnJumpPressed += HandleJump;
        playerHealth.OnHealthDepleted += PlayerDeath;
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
            Debug.Log("Jump");
        }
    }

    public override void OnStartLocalPlayer()
    {
        input.enabled = true;
        respawn.RespawnAtSpawnPoint(this, 0);

        UIManager.instance.healthBar.hm = playerHealth;
        UIManager.instance.healthBar.gameObject.SetActive(true);

        base.OnStartLocalPlayer();
    }

    private void ClientSideEnableRagdoll(bool val)
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
        }
    }

    private void FixedUpdate()
    {
        if (!isLocalPlayer)
        {
            return;
        }
        onGround = IsOnGround();
    }

    bool IsOnGround()
    {
        bool retVal = false;

        Vector3 origin = transform.position + new Vector3(0, 0.05f, 0);
        RaycastHit hit;

        if (Physics.Raycast(origin, -Vector3.up, out hit, 0.06f, layerMask))
        {
            retVal = true;
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

        return retVal;
    }

    [ClientRpc]
    public void RpcPlayerDeath()
    {
        if (isServer)
        {
            RpcEnableRagdoll(true);
        }
        else
        {
            CmdEnableRagdoll(true);
        }
        networkAnimator.enabled = false;
        input.enabled = false;
        respawn.RespawnAtSpawnPoint(this, 3f);
    }

    [Command]
    public void CmdPlayerDeath()
    {
        RpcPlayerDeath();
    }

    public void PlayerDeath(Attack attack)
    {
        if (isServer)
        {
            RpcPlayerDeath();
        }
        else
        {
            CmdPlayerDeath();
        }
    }

    [ClientRpc]
    public void RpcPlayerRespawn()
    {
        if (isServer)
        {
            RpcEnableRagdoll(false);
        }
        else
        {
            CmdEnableRagdoll(false);
        }
        networkAnimator.enabled = true;
        input.enabled = true;
    }

    [Command]
    public void CmdPlayerRespawn()
    {
        RpcPlayerRespawn();
    }

    [ClientRpc]
    void RpcEnableRagdoll(bool val)
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

    [Command]
    void CmdEnableRagdoll(bool val)
    {
        RpcEnableRagdoll(val);
    }
}
