using UnityEngine;
using UnityEngine.EventSystems;

public interface IAttackReceiver : IEventSystemHandler
{
    void RpcReceiveAttack(Attack attack);
    void CmdReceiveAttack(Attack attack);
}

