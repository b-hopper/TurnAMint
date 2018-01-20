using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

public class AttackReceiver : NetworkBehaviour, IAttackReceiver {
	public UnityEvent OnAttackReceived;

    [ClientRpc]
    public void RpcReceiveAttack(Attack attack)
    {
        OnAttackReceived.Invoke();
    }
    [Command]
    public void CmdReceiveAttack(Attack attack)
    {
        RpcReceiveAttack(attack);
    }
}
