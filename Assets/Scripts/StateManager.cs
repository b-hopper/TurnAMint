<<<<<<< HEAD
<<<<<<< HEAD
﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class StateManager : NetworkBehaviour {

=======
=======
>>>>>>> refs/remotes/origin/master
﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateManager : MonoBehaviour {
<<<<<<< HEAD
>>>>>>> refs/remotes/origin/master
=======
>>>>>>> refs/remotes/origin/master

    public bool aiming;
    public bool canRun;
    public bool walk;
    public bool shoot;
    public bool actualShooting;
    public bool reloading;
    public bool onGround;

<<<<<<< HEAD
<<<<<<< HEAD
    [HideInInspector] public HealthManager playerHealth;

    Animator[] animators;
    Animator mainAnimator;
    NetworkAnimator networkAnimator;
    Rigidbody[] bodyParts;
    Rigidbody mainRB;


=======
>>>>>>> refs/remotes/origin/master
=======
>>>>>>> refs/remotes/origin/master
    public float horizontal;
    public float vertical;
    public Vector3 lookPosition;
    public Vector3 lookHitPosition;
    public LayerMask layerMask;

<<<<<<< HEAD
<<<<<<< HEAD
    InputHandler input;

=======
>>>>>>> refs/remotes/origin/master
=======
>>>>>>> refs/remotes/origin/master
    public CharacterAudioManager audioManager;

    [HideInInspector]
    public HandleShooting handleShooting;
    [HideInInspector]
    public HandleAnimations handleAnim;

<<<<<<< HEAD
<<<<<<< HEAD
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
    }

=======
>>>>>>> refs/remotes/origin/master
=======
>>>>>>> refs/remotes/origin/master
    private void Start()
    {
        audioManager = GetComponent<CharacterAudioManager>();
        handleShooting = GetComponent<HandleShooting>();
        handleAnim = GetComponent<HandleAnimations>();
    }

<<<<<<< HEAD
<<<<<<< HEAD
    public override void OnStartLocalPlayer()
    {
        input.enabled = true;
        respawn.RespawnAtSpawnPoint(this);
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
            Debug.Log("Horizontal: " + horizontal, this);
            return;
        }
=======
    private void FixedUpdate()
    {
>>>>>>> refs/remotes/origin/master
=======
    private void FixedUpdate()
    {
>>>>>>> refs/remotes/origin/master
        onGround = IsOnGround();
    }

    bool IsOnGround()
    {
        bool retVal = false;

        Vector3 origin = transform.position + new Vector3(0, 0.05f, 0);
        RaycastHit hit;

        if (Physics.Raycast(origin, -Vector3.up, out hit, 0.5f, layerMask))
        {
            retVal = true;
        }

        return retVal;
    }
<<<<<<< HEAD
<<<<<<< HEAD

    [ClientRpc]
    public void RpcPlayerDeath()
    {
        Debug.Log("Death");
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
        respawn.RespawnAtSpawnPoint(this);
    }

    [Command]
    public void CmdPlayerDeath()
    {
        RpcPlayerDeath();
    }

    public void PlayerDeath()
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
        Debug.Log("Respawned", this);
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
=======
>>>>>>> refs/remotes/origin/master
=======
>>>>>>> refs/remotes/origin/master
}
