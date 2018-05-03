using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ClientBehavior : NetworkBehaviour {

    #region Declaring variables
    public string playerName;

    int frameCounter, lookUpdateCounter = 0;

    InputHandler input;
    CameraManager camMan;
    PlayerMovement movement;
    NetworkAnimator netAnim;
    HandleAnimations animHandler;
    [HideInInspector] public StateManager state { get; private set; }

    NetManager netM;
    #endregion

    #region Awake, Start, Update

    private void Awake()
    {
        input = GetComponent<InputHandler>();
        camMan = GetComponent<CameraManager>();
        movement = GetComponent<PlayerMovement>();
        state = GetComponent<StateManager>();
        netM = FindObjectOfType<NetManager>();
        netAnim = GetComponent<NetworkAnimator>();
        animHandler = GetComponent<HandleAnimations>();
    }

    private void Start()
    {
        if (!isLocalPlayer)
        {
            input.enabled = false;
            camMan.enabled = false;
            movement.enabled = false;
            animHandler.enabled = false;
        }
        else
        {            
            UIManager.instance.AssignPlayerState(state);
            UIManager.instance.healthBar.gameObject.SetActive(true);
            GameManager.Instance.LocalPlayerReference = state;
        }        
    }

    private void FixedUpdate()
    {
        if (!isLocalPlayer) return;

        frameCounter++;
        if (frameCounter >= 50)
        {
            frameCounter = 0;
            lookUpdateCounter = 0;
        }
        if (frameCounter >= lookUpdateCounter)
        {
            lookUpdateCounter += 10;
            CmdUpdateLookPosition(state.camPivotBase.localRotation);
        }

    }

    #endregion

    #region On Connected To Server
    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();

        GameManager.Instance.LocalPlayerReference = state;
        Cursor.lockState = CursorLockMode.Locked;

        CmdRegisterPlayer();
    }
    
    [Command]
    void CmdRegisterPlayer()
    {
        if (!isServer)
            return;
        
        netM.RegisterPlayer(this);
    }
    #endregion

    #region Syncing

    [Command]
    void CmdUpdateLookPosition(Quaternion rot)
    {        
        if (!isServer) return;
                
        RpcUpdateLookPosition(rot);
    }

    [ClientRpc]
    public void RpcUpdateLookPosition(Quaternion rot)
    {
        if (isLocalPlayer) return;
                
        state.SmoothMoveLookTarget(rot);
    }

    #endregion

    #region CmdPlayerDeath()
    [Command]
    public void CmdPlayerDeath()
    {
        if (!isServer) return;
        RpcPlayerDeath();
    }

    [ClientRpc]
    void RpcPlayerDeath()
    {
        if (isLocalPlayer) return;  // Already called client-side, no need to do it again
       
        state.RemoteDeath();
    }
    #endregion

    #region CmdPlayerRespawn()
    [Command]
    public void CmdPlayerRespawn()
    {
        if (!isServer) return;

        RpcPlayerRespawn();
    }

    [ClientRpc]
    void RpcPlayerRespawn()
    {
        if (isLocalPlayer) return;  // Already called client-side

        state.RemoteRespawn();
    }
    #endregion

    #region RequestPlayerHit(Attack newAttack, GameObject receivingPlayer)
    
    /// <summary>
    /// When a client-side shot is registered on a player, this lets the server know
    /// </summary>
    /// <param name="newAttack">Attack info</param>
    /// <param name="receivingPlayer">Player receiving the attack</param>
    internal void RequestPlayerHit(Attack newAttack, GameObject receivingPlayer)
    {
        CmdRequestHitFromServer(newAttack, receivingPlayer);
    }

    [Command]
    void CmdRequestHitFromServer(Attack newAttack, GameObject receivingPlayer)
    {
        if (!isServer || receivingPlayer == null) return;

        Debug.LogWarning("receivingPlayer " + receivingPlayer + " took " + newAttack.damage + " damage");
        
        ClientBehavior target = netM.GetClientFromGO(receivingPlayer);

        if (target != null)
        {
            target.RpcReceiveHitFromServer(newAttack);
        }
    }

    [ClientRpc]
    public void RpcReceiveHitFromServer(Attack newAttack)
    {
        if (!isLocalPlayer) return;

        state.playerHealth.TakeDamage(newAttack);
    }
    #endregion

    #region RequestRemoteShoot()

    /// <summary>
    /// Shot fired client-side, let the server know
    /// </summary>
    internal void RequestRemoteShoot()
    {
        CmdRequestShootFromServer();
    }

    [Command]
    void CmdRequestShootFromServer()
    {
        if (!isServer) return;

        RpcRemotePlayerShoot();
    }

    [ClientRpc]
    void RpcRemotePlayerShoot()
    {
        if (isLocalPlayer) return;

        state.RemoteFire();
    }

    #endregion

    #region RequestRemoteReload()
    internal void RequestRemoteReload()
    {
        CmdRemoteReload();
    }

    [Command]
    void CmdRemoteReload()
    {
        if (!isServer) return;

        RpcRemoteReload();
    }

    [ClientRpc]
    void RpcRemoteReload()
    {
        if (isLocalPlayer) return;

        state.RemoteReload();
    }


    #endregion

    #region CmdRemotePickup(GameObject obj)
    /// <summary>
    /// Local player picked up an item. Let server know
    /// </summary>
    /// <param name="obj">Object being picked up</param>
    [Command]
    public void CmdRemotePickup(GameObject obj)
    {
        if (!isServer) return;
        RpcRemotePickup(obj);
    }

    [ClientRpc]
    void RpcRemotePickup(GameObject obj)
    {
        if (!isLocalPlayer)         // Local player already picked up, only need if this is NOT local player
        {
            state.RemotePickup(obj);
        }
    }
    #endregion

    #region CmdRemoteDrop(GameObject obj)
    public void RemoteDrop(GameObject obj, Vector3 pos)
    {
        CmdRemoteDrop(obj, pos);
    }

    [Command]
    void CmdRemoteDrop(GameObject obj, Vector3 pos)
    {
        if (!isServer) return;
        RpcRemoteDrop(obj, pos);
    }

    [ClientRpc]
    void RpcRemoteDrop(GameObject obj, Vector3 pos)
    {
        if (!isLocalPlayer)
        {
            state.RemoteDrop(obj, pos);
        }
    }
    #endregion

    #region Miscellaneous
    [ClientRpc]
    public void RpcSetRandomSeed(int seed)
    {
        if (!isLocalPlayer) return;
        GameManager.Instance.Respawner.SetRandomSeed(seed);
        
    }
    #endregion
}
