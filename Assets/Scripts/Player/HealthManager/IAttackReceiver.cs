using UnityEngine;
using UnityEngine.EventSystems;

namespace TurnAMint.Player.Health
{
    public interface IAttackReceiver : IEventSystemHandler
    {
        void ReceiveAttack(Attack attack);
    }

}