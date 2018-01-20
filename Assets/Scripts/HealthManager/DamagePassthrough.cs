using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class DamagePassthrough : NetworkBehaviour, IAttackReceiver {
    HealthManager parentHM;

    [SerializeField]
    float damageMultiplier = 1;

    private void Start()
    {
        parentHM = GetComponentInParent<HealthManager>();
    }

    [ClientRpc]
    public void RpcReceiveAttack(Attack attack)
    {
        attack.damage = (int)(attack.damage * damageMultiplier);
        parentHM.RpcReceiveAttack(attack);
    }

    [Command]
    public void CmdReceiveAttack(Attack attack)
    {
        RpcReceiveAttack(attack);
    }

}
