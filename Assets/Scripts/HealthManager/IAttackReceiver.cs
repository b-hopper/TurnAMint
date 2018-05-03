using UnityEngine;
using UnityEngine.EventSystems;

public interface IAttackReceiver : IEventSystemHandler
{
    void ReceiveAttack(Attack attack);
}

